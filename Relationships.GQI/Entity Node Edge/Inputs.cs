namespace Skyline.DataMiner.Solutions.Relationships.GQI.Entity_Node_Edge
{
	using System;

	using Skyline.DataMiner.Analytics.GenericInterface;

	internal class Inputs
	{
		private readonly GQIStringArgument _entityIdArg = new GQIStringArgument("Entity ID") { IsRequired = true };
		private readonly GQIStringArgument _entityModelNameArg = new GQIStringArgument("Entity Model Name") { IsRequired = true };
		private readonly GQIStringArgument _entitySolutionIdArg = new GQIStringArgument("Entity Solution ID") { IsRequired = true };
		private readonly GQIIntArgument _depthArg = new GQIIntArgument("Depth") { IsRequired = false, DefaultValue = 1 };
		private readonly GQIBooleanArgument _includeInactiveArg = new GQIBooleanArgument("Include InActive Relations") { IsRequired = false, DefaultValue = false };

		public EntityKey EntityKey { get; private set; }

		public ushort Depth { get; private set; }

		public bool IncludeInActive { get; private set; }

		public bool IsValid { get; private set; }

		public GQIArgument[] GetInputArguments()
		{
			return new GQIArgument[]
			{
				_entityIdArg,
				_entityModelNameArg,
				_entitySolutionIdArg,
				_depthArg,
				_includeInactiveArg,
			};
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			if (!args.TryGetArgumentValue(_entityIdArg, out var entityId) ||
				String.IsNullOrEmpty(entityId))
			{
				throw new ArgumentException(String.Format("{0} should contain a valid non empty string. Value: {1}", _entityIdArg.Name, entityId), nameof(args));
			}

			if (!args.TryGetArgumentValue(_entityModelNameArg, out var modelName) ||
				String.IsNullOrEmpty(entityId))
			{
				throw new ArgumentException(String.Format("{0} should contain a valid non empty string. Value: {1}", _entityModelNameArg.Name, entityId), nameof(args));
			}

			if (!args.TryGetArgumentValue(_entitySolutionIdArg, out var solutionId) ||
				String.IsNullOrEmpty(entityId))
			{
				throw new ArgumentException(String.Format("{0} should contain a valid non empty string. Value: {1}", _entitySolutionIdArg.Name, entityId), nameof(args));
			}

			if (args.TryGetArgumentValue(_includeInactiveArg, out var includeInactive))
			{
				IncludeInActive = includeInactive;
			}

			if (!args.TryGetArgumentValue(_depthArg, out var depth))
			{
				depth = 1;
			}

			if (depth <= 0 ||
				depth >= 16)
			{
				throw new ArgumentException(String.Format("{0} should contain a valid depth (min: 1, max: 16). Value: {1}", _depthArg.Name, depth), nameof(args));
			}

			EntityKey = new EntityKey(solutionId, modelName, entityId);
			Depth = Convert.ToUInt16(depth);
			IsValid = true;
			return default;
		}
	}
}
