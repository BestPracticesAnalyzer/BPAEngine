namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class RuleReference
	{
		private string name;

		private PrePivot pivot;

		private string path;

		private bool query;

		private bool depend;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public PrePivot Pivot
		{
			get
			{
				return pivot;
			}
			set
			{
				pivot = value;
			}
		}

		public string Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
			}
		}

		public bool IsInQuery
		{
			get
			{
				return query;
			}
			set
			{
				query = value;
			}
		}

		public bool IsDependency
		{
			get
			{
				return depend;
			}
			set
			{
				depend = value;
			}
		}

		public RuleReference(string name)
		{
			this.name = name;
			path = null;
			depend = false;
			query = false;
		}
	}
}
