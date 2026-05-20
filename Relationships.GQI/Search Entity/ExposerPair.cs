namespace Skyline.DataMiner.Solutions.Relationships.GQI.SearchEntity
{
	using System;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	internal struct ExposerPair
	{
		public ExposerPair(FieldExposer sourceExposer, FieldExposer targetExposer, Func<GQIRow, GQICell> cellSelector)
		{
			SourceExposer = sourceExposer;
			TargetExposer = targetExposer;
			CellSelector = cellSelector ?? throw new ArgumentNullException(nameof(cellSelector));
		}

		public FieldExposer SourceExposer { get; }

		public FieldExposer TargetExposer { get; }

		public Func<GQIRow, GQICell> CellSelector { get; }
	}
}
