namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataInstanceLink
	{
		private DataChange change;

		private DataObjectLink objectLink;

		private DataInstance target;

		private DataInstanceLink backlink;

		public StepID StepID
		{
			get
			{
				return change.StepID;
			}
		}

		public DataChangeType ChangeType
		{
			get
			{
				return change.ChangeType;
			}
		}

		public DataScope Scope
		{
			get
			{
				return change.Scope;
			}
		}

		public DataObjectLink ObjectLink
		{
			get
			{
				return objectLink;
			}
		}

		public DataInstance Target
		{
			get
			{
				return target;
			}
		}

		public DataInstanceLink Backlink
		{
			get
			{
				return backlink;
			}
		}

		private DataInstanceLink()
		{
		}

		private DataInstanceLink(DataChange change, DataObjectLink objectLink, DataInstance target, DataInstanceLink backlink)
		{
			this.change = change;
			this.objectLink = objectLink;
			this.target = target;
			this.backlink = backlink;
		}

		internal DataInstanceLink(DataObjectLink objectLink, DataInstance source, DataInstance target, StepID stepID, DataScope scope, DataChangeType changeType)
			: this(null, objectLink, target, null)
		{
			change = new DataChange(stepID, changeType, scope);
			backlink = new DataInstanceLink(change, this.objectLink.Backlink, source, this);
		}

		internal bool IsInScope(StepID stepID)
		{
			if (Scope == DataScope.Local)
			{
				if (!stepID.Under(StepID))
				{
					return false;
				}
			}
			else if (Scope == DataScope.Step && !stepID.Equals(StepID))
			{
				return false;
			}
			return true;
		}

		internal void ApplyToSet(DataInstanceSet instances)
		{
			switch (ChangeType)
			{
			case DataChangeType.Add:
				if (!instances.Contains(Target.Name))
				{
					instances.Add(Target);
				}
				break;
			case DataChangeType.Delete:
				if (Target.Name.Length == 0)
				{
					instances.Clear();
				}
				else if (instances.Contains(Target.Name))
				{
					instances.Remove(Target.Name);
				}
				break;
			case DataChangeType.Replace:
				instances.Clear();
				instances.Add(Target);
				break;
			}
		}
	}
}
