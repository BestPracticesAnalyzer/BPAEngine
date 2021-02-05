namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class RuleExpression
	{
		private string text;

		private string[] refs;

		private bool query;

		private bool depend;

		public string Text
		{
			get
			{
				return text;
			}
			set
			{
				text = value;
			}
		}

		public string[] Refs
		{
			get
			{
				return refs;
			}
			set
			{
				refs = value;
			}
		}

		public bool IsQuery
		{
			get
			{
				return query;
			}
		}

		public bool IsDependency
		{
			get
			{
				return depend;
			}
		}

		public RuleExpression(string text, bool query, bool depend)
		{
			this.text = text;
			this.query = query;
			this.depend = depend;
		}
	}
}
