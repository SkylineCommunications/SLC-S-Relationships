namespace Skyline.DataMiner.Solutions.Relationships.GQI.SearchEntity
{
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.Solutions.Relationships.GQI.Shared;

	using SLDataGateway.API.Querying;

	[GQIMetaData(Name = "Relationships.GQI.SearchEntity")]
	public class SearchEntity : IGQIOnInit, IGQIInputArguments, IGQIOnPrepareFetch, IGQIDataSource, IGQIOnDestroy
	{
		private readonly IReadOnlyDictionary<GQIColumn, FieldExposer> _columnMap = new Dictionary<GQIColumn, FieldExposer>
		{
			[new GQIStringColumn("ID")] = EntityDescriptorExposers.ID,
			[new GQIIntColumn("Status")] = EntityDescriptorExposers.Status,
			[new GQIStringColumn("Display Name")] = EntityDescriptorExposers.DisplayName,
			[new GQIStringColumn("Model Name")] = EntityDescriptorExposers.ModelName,
			[new GQIStringColumn("Solution ID")] = EntityDescriptorExposers.SolutionID,
			[new GQIStringColumn("Solution Name")] = EntityDescriptorExposers.SolutionName,
			[new GQIStringColumn("Parent ID")] = EntityDescriptorExposers.ParentID,
			[new GQIStringColumn("Parent Model Name")] = EntityDescriptorExposers.ParentModelName,
			[new GQIStringColumn("Dropdown")] = EntityDescriptorExposers.SolutionName,
		};

		private readonly SearchInputs _inputs = new SearchInputs();
		private HashSet<string> _seen = new HashSet<string>();
		private GQIPageEnumerator _pageEnumerator;
		private IGQILogger _logger;
		private IRelationshipManager _manager;

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			_logger = args.Logger;
			_manager = args.GetRelationshipManager();
			return default;
		}

		public GQIArgument[] GetInputArguments()
		{
			return _inputs.GetInputArguments();
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			return _inputs.OnArgumentsProcessed(args);
		}

		public GQIColumn[] GetColumns() => _columnMap.Keys.ToArray();

		public OnPrepareFetchOutputArgs OnPrepareFetch(OnPrepareFetchInputArgs args)
		{
			var filter = _inputs.GetFilter()
				.OrderBy(EntityDescriptorExposers.SolutionID)
				.OrderBy(EntityDescriptorExposers.ModelName)
				.OrderBy(EntityDescriptorExposers.DisplayName);

			var postFilter = _inputs.GetPostFilter();
			var postSorting = _inputs.GetPostSorting();

			_pageEnumerator = new GQIPageEnumerator(_manager.Entities
				.ReadPaged(filter, 100)
				.SelectMany(page => page
					.Where(e => _seen.Add($"{e.ModelName}/{e.ID}")) // Ensure distinct entities based on ID (should not happen)
					.Where(postFilter)
					.OrderBy(entity => entity, Comparer<EntityDescriptor>.Create(postSorting))
					.Select(CreateGQIRow)));

			return default;
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			return _pageEnumerator.GetNextPage(100);
		}

		public OnDestroyOutputArgs OnDestroy(OnDestroyInputArgs args)
		{
			_pageEnumerator?.Dispose();
			return default;
		}

		private static GQIRow CreateGQIRow(EntityDescriptor entity)
		{
			return new GQIRow(new GQICell[]
			{
				new GQICell { Value = entity.ID },
				new GQICell { Value = (int)entity.Status, DisplayValue = entity.Status.ToString() },
				new GQICell { Value = entity.DisplayName },
				new GQICell { Value = entity.ModelName },
				new GQICell { Value = entity.SolutionID },
				new GQICell { Value = entity.SolutionName },
				new GQICell { Value = entity.ParentID },
				new GQICell { Value = entity.ParentModelName },
				new GQICell { Value = $"{entity.SolutionName} - {entity.ModelName}: {entity.DisplayName} (ID: {entity.ID})" }, // For the dropdown
			});
		}
	}
}
