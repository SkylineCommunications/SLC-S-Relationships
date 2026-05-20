namespace Skyline.DataMiner.Solutions.Relationships.GQI.SearchEntity
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	internal class SearchInputs
	{
		private readonly GQIStringArgument _idArg = new GQIStringArgument("ID") { IsRequired = false };
		private readonly GQIStringArgument _displayNameArg = new GQIStringArgument("Display Name") { IsRequired = false };
		private readonly GQIStringArgument _modelNameArg = new GQIStringArgument("Model Name") { IsRequired = false };
		private readonly GQIStringArgument _solutionIdArg = new GQIStringArgument("Solution ID") { IsRequired = false };
		private readonly GQIStringArgument _solutionNameArg = new GQIStringArgument("Solution Name") { IsRequired = false };
		private readonly GQIStringArgument _parentIdArg = new GQIStringArgument("Parent ID") { IsRequired = false };
		private readonly GQIStringArgument _parentModelNameArg = new GQIStringArgument("Parent Model Name") { IsRequired = false };

		public string ID { get; private set; } = String.Empty;

		public string DisplayName { get; private set; } = String.Empty;

		public string ModelName { get; private set; } = String.Empty;

		public string SolutionID { get; private set; } = String.Empty;

		public string SolutionName { get; private set; } = String.Empty;

		public string ParentID { get; private set; } = String.Empty;

		public string ParentModelName { get; private set; } = String.Empty;

		public GQIArgument[] GetInputArguments()
		{
			return new GQIArgument[]
			{
				_idArg,
				_displayNameArg,
				_modelNameArg,
				_solutionIdArg,
				_solutionNameArg,
				_parentIdArg,
				_parentModelNameArg,
			};
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			if (args.TryGetArgumentValue(_idArg, out var id))
			{
				ID = id;
			}

			if (args.TryGetArgumentValue(_displayNameArg, out var displayName))
			{
				DisplayName = displayName;
			}

			if (args.TryGetArgumentValue(_modelNameArg, out var modelName))
			{
				ModelName = modelName;
			}

			if (args.TryGetArgumentValue(_solutionIdArg, out var solutionId))
			{
				SolutionID = solutionId;
			}

			if (args.TryGetArgumentValue(_solutionNameArg, out var solutionName))
			{
				SolutionName = solutionName;
			}

			if (args.TryGetArgumentValue(_parentIdArg, out var parentId))
			{
				ParentID = parentId;
			}

			if (args.TryGetArgumentValue(_parentModelNameArg, out var parentModelName))
			{
				ParentModelName = parentModelName;
			}

			return new OnArgumentsProcessedOutputArgs();
		}

		public FilterElement<EntityDescriptor> GetFilter()
		{
			var filters = new List<FilterElement<EntityDescriptor>>();
			if (!String.IsNullOrEmpty(ID))
			{
				filters.Add(EntityDescriptorExposers.ID.Contains(ID));
			}

			if (!String.IsNullOrEmpty(DisplayName))
			{
				filters.Add(EntityDescriptorExposers.DisplayName.Contains(DisplayName));
			}

			if (!String.IsNullOrEmpty(ModelName))
			{
				filters.Add(EntityDescriptorExposers.ModelName.Contains(ModelName));
			}

			if (!String.IsNullOrEmpty(SolutionID))
			{
				filters.Add(EntityDescriptorExposers.SolutionID.Contains(SolutionID));
			}

			if (!String.IsNullOrEmpty(SolutionName))
			{
				filters.Add(EntityDescriptorExposers.SolutionName.Contains(SolutionName));
			}

			if (!String.IsNullOrEmpty(ParentID))
			{
				filters.Add(EntityDescriptorExposers.ParentID.Contains(ParentID));
			}

			if (!String.IsNullOrEmpty(ParentModelName))
			{
				filters.Add(EntityDescriptorExposers.ParentModelName.Contains(ParentModelName));
			}

			if (filters.Any())
			{
				return new ANDFilterElement<EntityDescriptor>(filters.ToArray());
			}

			return new TRUEFilterElement<EntityDescriptor>();
		}

		public Func<EntityDescriptor, bool> GetPostFilter()
		{
			var filters = new List<Expression<Func<EntityDescriptor, bool>>>();
			if (!String.IsNullOrEmpty(ID))
			{
				filters.Add((entity) => entity != null && !String.IsNullOrEmpty(entity.ID) && entity.ID.Contains(ID));
			}

			if (!String.IsNullOrEmpty(DisplayName))
			{
				filters.Add((entity) => entity != null && !String.IsNullOrEmpty(entity.DisplayName) && entity.DisplayName.Contains(DisplayName));
			}

			if (!String.IsNullOrEmpty(ModelName))
			{
				filters.Add((entity) => entity != null && !String.IsNullOrEmpty(entity.ModelName) && entity.ModelName.Contains(ModelName));
			}

			if (!String.IsNullOrEmpty(SolutionID))
			{
				filters.Add((entity) => entity != null && !String.IsNullOrEmpty(entity.SolutionID) && entity.SolutionID.Contains(SolutionID));
			}

			if (!String.IsNullOrEmpty(SolutionName))
			{
				filters.Add((entity) => entity != null && !String.IsNullOrEmpty(entity.SolutionName) && entity.SolutionName.Contains(SolutionName));
			}

			if (!String.IsNullOrEmpty(ParentID))
			{
				filters.Add((entity) => entity != null && !String.IsNullOrEmpty(entity.ParentID) && entity.ParentID.Contains(ParentID));
			}

			if (!String.IsNullOrEmpty(ParentModelName))
			{
				filters.Add((entity) => entity != null && !String.IsNullOrEmpty(entity.ParentModelName) && entity.ParentModelName.Contains(ParentModelName));
			}

			if (filters.Any())
			{
				var expression = filters.Aggregate((left, right) =>
				{
					var parameter = Expression.Parameter(typeof(EntityDescriptor), "entity");
					var body = Expression.AndAlso(
						Expression.Invoke(left, parameter),
						Expression.Invoke(right, parameter));
					return Expression.Lambda<Func<EntityDescriptor, bool>>(body, parameter);
				});

				return expression.Compile();
			}

			return (entity) => true;
		}

		public Comparison<EntityDescriptor> GetPostSorting()
		{
			return (entity1, entity2) =>
			{
				var score1 = CalculateMatchScore(entity1);
				var score2 = CalculateMatchScore(entity2);

				var scoreComparison = score2.CompareTo(score1);
				if (scoreComparison != 0)
				{
					return scoreComparison;
				}

				return String.Compare(entity1?.ID, entity2?.ID, StringComparison.Ordinal);
			};
		}

		private int CalculateMatchScore(EntityDescriptor entity)
		{
			if (entity == null)
			{
				return -1;
			}

			var score = 0;

			score += CalculateFieldScore(entity.ID, ID);
			score += CalculateFieldScore(entity.DisplayName, DisplayName);
			score += CalculateFieldScore(entity.ModelName, ModelName);
			score += CalculateFieldScore(entity.SolutionID, SolutionID);
			score += CalculateFieldScore(entity.SolutionName, SolutionName);
			score += CalculateFieldScore(entity.ParentID, ParentID);
			score += CalculateFieldScore(entity.ParentModelName, ParentModelName);

			return score;
		}

		private int CalculateFieldScore(string fieldValue, string searchValue)
		{
			if (String.IsNullOrEmpty(searchValue) || String.IsNullOrEmpty(fieldValue))
			{
				return 0;
			}

			if (fieldValue.Equals(searchValue, StringComparison.OrdinalIgnoreCase))
			{
				return 1000;
			}

			if (fieldValue.StartsWith(searchValue, StringComparison.OrdinalIgnoreCase))
			{
				return 100;
			}

			if (fieldValue.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return 10;
			}

			return 0;
		}
	}
}
