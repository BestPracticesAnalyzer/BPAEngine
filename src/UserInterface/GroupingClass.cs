namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class GroupingClass
	{
		private GroupingClass parent;

		private string type;

		private string name;

		private int depth;

		public GroupingClass Parent
		{
			get
			{
				return parent;
			}
		}

		public string Type
		{
			get
			{
				return type;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public int Depth
		{
			get
			{
				return depth;
			}
			set
			{
				depth = value;
			}
		}

		public GroupingClass(GroupingClass parent, string type, string name, int depth)
		{
			this.parent = parent;
			this.type = type;
			this.name = name;
			this.depth = depth;
		}
	}
}
