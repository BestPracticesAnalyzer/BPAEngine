using System;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class VersionInfo
	{
		private Version webVersion = new Version("0.0.0.0");

		private string description = string.Empty;

		private string autoUrl = string.Empty;

		private string manualUrl = string.Empty;

		private bool found;

		private bool newerThanLocal;

		private ConfigurationInfo configInfo;

		private string[] referencedFiles;

		public Version WebVersion
		{
			get
			{
				return webVersion;
			}
			set
			{
				webVersion = value;
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

		public string AutoUrl
		{
			get
			{
				return autoUrl;
			}
			set
			{
				autoUrl = value;
			}
		}

		public string ManualUrl
		{
			get
			{
				return manualUrl;
			}
			set
			{
				manualUrl = value;
			}
		}

		public bool Found
		{
			get
			{
				return found;
			}
			set
			{
				found = value;
			}
		}

		public bool NewerThanLocal
		{
			get
			{
				return newerThanLocal;
			}
			set
			{
				newerThanLocal = value;
			}
		}

		public ConfigurationInfo ConfigInfo
		{
			get
			{
				return configInfo;
			}
			set
			{
				configInfo = value;
			}
		}

		public string[] ReferencedFiles
		{
			get
			{
				return referencedFiles;
			}
			set
			{
				referencedFiles = value;
			}
		}
	}
}
