namespace Skyline.DataMiner.Solutions.Relationships.GQI.Entity_Node_Edge
{
	using System;

	internal enum NodeStatus
	{
		Active,
		Inactive,
		Both,
	}

	internal class NodeContext : IEquatable<NodeContext>
	{
		public static readonly NodeContext Empty = new NodeContext();

		public NodeContext(EntityDescriptor[] entityDescriptors)
		{
			if (entityDescriptors.Length == 0)
			{
				return;
			}

			ID = entityDescriptors[0].ID;
			ModelName = entityDescriptors[0].ModelName;
			DisplayName = entityDescriptors[0].DisplayName;
			SolutionID = entityDescriptors[0].SolutionID;
			SolutionName = entityDescriptors[0].SolutionName;
			ParentID = entityDescriptors[0].ParentID;
			ParentModelName = entityDescriptors[0].ParentModelName;

			bool hasActive = false;
			bool hasInactive = false;
			foreach (var descriptor in entityDescriptors)
			{
				if (descriptor.Status == EntityStatus.Active)
				{
					hasActive = true;
				}
				else
				{
					hasInactive = true;
				}

				if (hasActive && hasInactive)
				{
					break;
				}
			}

			Status = hasActive && hasInactive ? NodeStatus.Both
				   : hasActive ? NodeStatus.Active
				   : NodeStatus.Inactive;
		}

		private NodeContext()
		{
		}

		public EntityDescriptor[] EntityDescriptors { get; }

		public string ID { get; }

		public string ModelName { get; }

		public string DisplayName { get; }

		public string SolutionID { get; }

		public string SolutionName { get; }

		public string ParentID { get; }

		public string ParentModelName { get; }

		public NodeStatus Status { get; }

		public bool Equals(NodeContext other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return other.ID == ID
				&& other.ModelName == ModelName
				&& other.DisplayName == DisplayName
				&& other.SolutionID == SolutionID
				&& other.SolutionName == SolutionName
				&& other.ParentID == ParentID
				&& other.ParentModelName == ParentModelName
				&& other.Status == Status;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as NodeContext);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = (hash * 31) + (ID?.GetHashCode() ?? 0);
				hash = (hash * 31) + (ModelName?.GetHashCode() ?? 0);
				hash = (hash * 31) + (DisplayName?.GetHashCode() ?? 0);
				hash = (hash * 31) + (SolutionID?.GetHashCode() ?? 0);
				hash = (hash * 31) + (SolutionName?.GetHashCode() ?? 0);
				hash = (hash * 31) + (ParentID?.GetHashCode() ?? 0);
				hash = (hash * 31) + (ParentModelName?.GetHashCode() ?? 0);
				hash = (hash * 31) + Status.GetHashCode();
				return hash;
			}
		}
	}
}
