namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataObjectLink
	{
		private string name;

		private DataObjectLink backlink;

		private DataObject target;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public DataObjectLink Backlink
		{
			get
			{
				return backlink;
			}
		}

		public DataObject Target
		{
			get
			{
				return target;
			}
		}

		private DataObjectLink()
		{
		}

		private DataObjectLink(string name, DataObject target, DataObjectLink backlink)
		{
			this.name = name;
			this.target = target;
			this.backlink = backlink;
		}

		internal DataObjectLink(DataObject source, string sourceName, DataObject target, string targetName)
			: this(targetName, target, null)
		{
			backlink = new DataObjectLink(sourceName, source, this);
		}
	}
}
