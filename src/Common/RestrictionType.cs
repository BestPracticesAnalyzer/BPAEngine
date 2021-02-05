using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class RestrictionType
	{
		private string name;

		private string display;

		private string description;

		private bool collapse;

		private bool filter;

		private bool enabled;

		private Restrictions allRestrictions;

		private ArrayList optionList = new ArrayList();

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

		public bool Collapse
		{
			get
			{
				return collapse;
			}
			set
			{
				collapse = value;
			}
		}

		public bool Filter
		{
			get
			{
				return filter;
			}
			set
			{
				filter = value;
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

		public ArrayList OptionList
		{
			get
			{
				return optionList;
			}
		}

		public Restrictions AllRestrictions
		{
			get
			{
				return allRestrictions;
			}
		}

		public RestrictionType(string name, Restrictions allRestrictions)
		{
			this.name = name;
			this.allRestrictions = allRestrictions;
		}
	}
}
