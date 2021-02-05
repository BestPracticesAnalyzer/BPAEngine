using System;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	[AttributeUsage(AttributeTargets.Property)]
	public class AppCmdLineArgumentAttribute : Attribute
	{
		private string name = "";

		private string description = "";

		private bool required;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string Description
		{
			get
			{
				return description;
			}
		}

		public bool Required
		{
			get
			{
				return required;
			}
		}

		public AppCmdLineArgumentAttribute(string optionName, string descrip)
			: this(optionName, descrip, false)
		{
		}

		public AppCmdLineArgumentAttribute(string optionName, string description, bool required)
		{
			name = optionName;
			this.description = description;
			this.required = required;
		}
	}
}
