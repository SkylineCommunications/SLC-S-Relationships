namespace Skyline.DataMiner.Solutions.Relationships.GQI.Shared
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Analytics.GenericInterface.Operators;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;

	using SLDataGateway.API.Querying;
	using SLDataGateway.API.Types.Querying;

	internal class SortTranslator<T>
	{
		private readonly IReadOnlyDictionary<GQIColumn, FieldExposer> _map;

		public SortTranslator(IReadOnlyDictionary<GQIColumn, FieldExposer> map)
		{
			_map = map ?? throw new ArgumentNullException(nameof(map));
		}

		public IQuery<T> Translate(IGQISortOperator sortOperator)
		{
			return Translate(new TRUEFilterElement<T>().ToQuery(), sortOperator);
		}

		public IQuery<T> Translate(IQuery<T> baseQuery, IGQISortOperator sortOperator)
		{
			if (baseQuery == null)
			{
				throw new ArgumentNullException(nameof(baseQuery));
			}

			if (sortOperator?.Fields is null)
			{
				return baseQuery;
			}

			var query = baseQuery;
			foreach (var sortField in sortOperator.Fields)
			{
				var exposer = _map.FirstOrDefault(map => sortField.Column.Equals(map.Key)).Value;
				if (exposer is null)
				{
					continue;
				}

				var direction = TranslateDirection(sortField.Direction);
				var orderByElement = OrderByElementFactory.Create(exposer, direction);
				if (!query.Order.Elements.Any())
				{
					query = query.WithOrder(
						OrderBy.Default.SingleConcat(orderByElement));
				}
				else
				{
					query = query.WithOrder(
						query.Order.SingleConcat(orderByElement));
				}
			}

			return query;
		}

		private static SortOrder TranslateDirection(GQISortDirection direction)
		{
			switch (direction)
			{
				case GQISortDirection.Ascending:
					return SortOrder.Ascending;

				case GQISortDirection.Descending:
					return SortOrder.Descending;

				default:
					throw new NotSupportedException($"The sort direction '{direction}' is not supported.");
			}
		}
	}
}
