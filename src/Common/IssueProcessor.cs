namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public abstract class IssueProcessor
	{
		protected ExecutionInterface executionInterface;

		public IssueProcessor(ExecutionInterface executionInterface)
		{
			this.executionInterface = executionInterface;
		}

		public virtual void ProcessIssue(Node messageNode, Node ruleNode, params object[] args)
		{
			string[] array = new string[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				array[i] = ExtFunction.Stringify(args[i]);
			}
			ProcessIssue(messageNode, ruleNode, array);
		}

		public virtual void ProcessIssue(Node messageNode, Node ruleNode, params string[] args)
		{
		}
	}
}
