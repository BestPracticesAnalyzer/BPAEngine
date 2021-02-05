namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class DataChange
	{
		private StepID stepID;

		private DataChangeType changeType;

		private DataScope scope;

		internal StepID StepID
		{
			get
			{
				return stepID;
			}
		}

		internal DataChangeType ChangeType
		{
			get
			{
				return changeType;
			}
		}

		internal DataScope Scope
		{
			get
			{
				return scope;
			}
		}

		internal DataChange(StepID stepID, DataChangeType changeType, DataScope scope)
		{
			this.stepID = stepID;
			this.changeType = changeType;
			this.scope = scope;
		}
	}
}
