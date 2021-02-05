namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ObjectProgress
	{
		public enum ObjectStatus
		{
			InProgress,
			CompletedOkay,
			CompletedWithWarning,
			CompletedWithError
		}

		private int percentageDone;

		private ObjectStatus status;

		private bool globalProgress;

		public int PercentageDone
		{
			get
			{
				return percentageDone;
			}
			set
			{
				percentageDone = value;
			}
		}

		public ObjectStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				status = value;
			}
		}

		public bool GlobalProgress
		{
			get
			{
				return globalProgress;
			}
			set
			{
				globalProgress = value;
			}
		}

		internal ObjectProgress()
		{
		}
	}
}
