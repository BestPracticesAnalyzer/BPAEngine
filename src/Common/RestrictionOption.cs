using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class RestrictionOption
	{
		private string name;

		private string display;

		private string description;

		private bool enabled;

		private RestrictionType type;

		private ArrayList includes = new ArrayList();

		private string value;

		private string cost = "0,0";

		private string group = "";

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string Display
		{
			get
			{
				return display;
			}
			set
			{
				display = value;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		public RestrictionType Type
		{
			get
			{
				return type;
			}
		}

		public ArrayList Includes
		{
			get
			{
				return includes;
			}
		}

		public string Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		public string Cost
		{
			get
			{
				return cost;
			}
			set
			{
				cost = value;
			}
		}

		public string Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public RestrictionOption(string name, RestrictionType type)
		{
			this.name = name;
			this.type = type;
		}

		public override string ToString()
		{
			return display;
		}
	}
}
