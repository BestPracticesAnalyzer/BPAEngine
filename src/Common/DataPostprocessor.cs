namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public abstract class DataPostprocessor
	{
		protected ExecutionInterface executionInterface;

		protected Document data;

		public DataPostprocessor(ExecutionInterface executionInterface, Document data)
		{
			this.executionInterface = executionInterface;
			this.data = data;
		}

		public virtual void ProcessData()
		{
		}
	}
}
