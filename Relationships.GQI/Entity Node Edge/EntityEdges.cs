namespace Skyline.DataMiner.Solutions.Relationships.GQI.Entity_Node_Edge
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.Solutions.Relationships.GQI.Shared;

	[GQIMetaData(Name = "Relationships.GQI.Entity.Edges")]
	public class EntityEdges : IGQIDataSource, IGQIOnInit, IGQIInputArguments, IGQIOnPrepareFetch, IGQIOnDestroy
	{
		private readonly IList<GQIColumn> _columns = new List<GQIColumn>
		{
			new GQIStringColumn("From ID"),
			new GQIStringColumn("To ID"),
			new GQIIntColumn("Status"),
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

				// Traverse the relationships starting from the entity to get all relations up to the specified depth
				var query = _manager.From(entity);
				if (!_inputs.IncludeInActive)
				{
					query = query.Where(EntityDescriptorExposers.Status.Equal(EntityStatus.Active));
				}

				var results = query.Traverse(_inputs.Depth);
				_pageEnumerator = new GQIPageEnumerator(
					results
					.SelectMany(r => r.Relations)
					.Select(CreateGQIRow));
			}
			catch (Exception ex)
			{
				_logger.Error(ex, ex.Message);
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

		private static GQIRow CreateGQIRow(Relation relation)
		{
			var state = relation.SourceState == EntityStatus.Inactive || relation.TargetState == EntityStatus.Inactive
				? NodeStatus.Inactive
				: NodeStatus.Active;

			return new GQIRow(relation.Identifier, new GQICell[]
			{
				new GQICell { Value = Convert.ToString(relation.SourceKey) },
				new GQICell { Value = Convert.ToString(relation.TargetKey) },
				new GQICell { Value = (int)state, DisplayValue = state.ToString() },
			});
		}
	}
}
