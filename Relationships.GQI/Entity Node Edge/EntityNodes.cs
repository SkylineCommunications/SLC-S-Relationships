namespace Skyline.DataMiner.Solutions.Relationships.GQI.Entity_Node_Edge
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.Solutions.Relationships;
	using Skyline.DataMiner.Solutions.Relationships.GQI.Shared;

	[GQIMetaData(Name = "Relationships.GQI.Entity.Nodes")]
	public class EntityNodes : IGQIDataSource, IGQIOnInit, IGQIInputArguments, IGQIOnPrepareFetch, IGQIOnDestroy
	{
		private readonly IList<GQIColumn> _columns = new List<GQIColumn>
		{
			new GQIStringColumn("Key"),
			new GQIStringColumn("ID"),
			new GQIIntColumn("Status"),
			new GQIStringColumn("Display Name"),
			new GQIStringColumn("Model Name"),
			new GQIStringColumn("Solution ID"),
			new GQIStringColumn("Solution Name"),
			new GQIStringColumn("Parent ID"),
			new GQIStringColumn("Parent Model Name"),
		};

		private readonly HashSet<string> _seen = new HashSet<string>();
		private readonly Inputs _inputs = new Inputs();

		private GQIPageEnumerator _pageEnumerator;
		private IRelationshipManager _manager;
		private IGQILogger _logger;

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			try
			{
				_logger = args.Logger;
				_manager = args.GetRelationshipManager();
			}
			catch (Exception ex)
			{
				_logger.Error(ex, ex.Message);
			}

			return default;
		}

		public GQIArgument[] GetInputArguments() => _inputs.GetInputArguments();

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			try
			{
				return _inputs.OnArgumentsProcessed(args);
			}
			catch (Exception ex)
			{
				_logger.Error(ex, ex.Message);
			}

			return default;
		}

		public GQIColumn[] GetColumns() => _columns.ToArray();

		public OnPrepareFetchOutputArgs OnPrepareFetch(OnPrepareFetchInputArgs args)
		{
			try
			{
				// Read the entity for the provided entity key to ensure it exists and to get its details for filtering and display purposes
				var filter = new ANDFilterElement<EntityDescriptor>(
					EntityDescriptorExposers.SolutionID.Equal(_inputs.EntityKey.SolutionID),
					EntityDescriptorExposers.ModelName.Equal(_inputs.EntityKey.ModelName),
					EntityDescriptorExposers.ID.Equal(_inputs.EntityKey.ID));

				var entity = _manager.Entities.Read(filter).FirstOrDefault();
				if (entity is null)
				{
					throw new KeyNotFoundException($"No entity found for key: {_inputs.EntityKey}");
				}

				// Traverse the relationships starting from the entity to get all related entities up to the specified depth
				var query = _manager.From(entity);
				if (!_inputs.IncludeInActive)
				{
					query = query.Where(EntityDescriptorExposers.Status.Equal(EntityStatus.Active));
				}

				var results = query.Traverse(_inputs.Depth);
				_pageEnumerator = new GQIPageEnumerator(
					new GQIRow[] { CreateGQIRow(entity) },
					results.Select(r => CreateGQIRow(r.Entity)));
			}
			catch (Exception ex)
			{
				_logger.Error(ex, ex.Message);
				_pageEnumerator = null;
			}

			return default;
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			if (_pageEnumerator is null)
			{
				return new GQIPage(new GQIRow[0]);
			}

			return _pageEnumerator.GetNextPage(100);
		}

		public OnDestroyOutputArgs OnDestroy(OnDestroyInputArgs args)
		{
			_pageEnumerator?.Dispose();
			return default;
		}

		private static GQIRow CreateGQIRow(EntityDescriptor entity)
		{
			return new GQIRow(entity.ID, new GQICell[]
			{
				new GQICell { Value = entity.Key.ToString() },
				new GQICell { Value = entity.ID },
				new GQICell { Value = (int)entity.Status, DisplayValue = entity.Status.ToString() },
				new GQICell { Value = entity.DisplayName },
				new GQICell { Value = entity.ModelName },
				new GQICell { Value = entity.SolutionID },
				new GQICell { Value = entity.SolutionName },
				new GQICell { Value = entity.ParentID },
				new GQICell { Value = entity.ParentModelName },
			});
		}
	}
}
