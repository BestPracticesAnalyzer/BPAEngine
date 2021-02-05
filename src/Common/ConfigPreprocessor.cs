namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public abstract class ConfigPreprocessor
	{
		protected ExecutionInterface executionInterface;

		protected Document config;

		public ConfigPreprocessor(ExecutionInterface executionInterface, Document config)
		{
			this.executionInterface = executionInterface;
			this.config = config;
		}

		public virtual void ProcessConfiguration()
		{
		}
	}
}
