using System;
using System.IO;
using System.Xml;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class ConfigurationInfo
	{
		private ExecutionInterface execInterface;

		private string fileName = "";

		private Version configVersion;

		private string downloadURL = "";

		private string articleURL = "";

		private string helpFile = "";

		private string downloadStart = "http://www.microsoft.com";

		private string articleStart = "http://go.microsoft.com";

		private bool appendToDownloadURL = true;

		private string[] referencedFiles;

		private bool masterConfig = true;

		public Version ConfigVersion
		{
			get
			{
				return configVersion;
			}
		}

		public string DownloadURL
		{
			get
			{
				return downloadURL;
			}
		}

		public string ArticleURL
		{
			get
			{
				return articleURL;
			}
		}

		public bool ConfigFound
		{
			get
			{
				return configVersion.ToString() != "0.0.0.0";
			}
		}

		public string DownloadStart
		{
			get
			{
				return downloadStart;
			}
			set
			{
				downloadStart = value;
			}
		}

		public string ArticleStart
		{
			get
			{
				return articleStart;
			}
			set
			{
				articleStart = value;
			}
		}

		public bool AppendToDownloadURL
		{
			get
			{
				return appendToDownloadURL;
			}
			set
			{
				appendToDownloadURL = value;
			}
		}

		public string HelpFile
		{
			get
			{
				return helpFile;
			}
		}

		public string[] ReferencedFiles
		{
			get
			{
				return referencedFiles;
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
		}

		public string Name
		{
			get
			{
				return fileName.Substring(fileName.LastIndexOf("\\") + 1);
			}
		}

		public bool Master
		{
			get
			{
				return masterConfig;
			}
		}

		public ConfigurationInfo(ExecutionInterface execInterface)
			: this(execInterface, true, execInterface.Options.Configuration.FileName, null)
		{
		}

		public ConfigurationInfo(ExecutionInterface execInterface, bool load)
			: this(execInterface, load, execInterface.Options.Configuration.FileName, null)
		{
		}

		public ConfigurationInfo(ExecutionInterface execInterface, string fileName, ConfigurationInfo masterConfigInfo)
			: this(execInterface, true, fileName, masterConfigInfo)
		{
		}

		public ConfigurationInfo(ExecutionInterface execInterface, bool load, string fileName, ConfigurationInfo masterConfigInfo)
		{
			masterConfig = masterConfigInfo == null;
			if (!masterConfig)
			{
				downloadStart = masterConfigInfo.DownloadStart;
				articleStart = masterConfigInfo.ArticleStart;
				appendToDownloadURL = masterConfigInfo.AppendToDownloadURL;
				downloadURL = masterConfigInfo.DownloadURL;
			}
			this.execInterface = execInterface;
			this.fileName = fileName;
			if (load)
			{
				ReloadInfo();
			}
		}

		public void ReloadInfo()
		{
			articleURL = "";
			referencedFiles = null;
			if (masterConfig)
			{
				downloadURL = "";
			}
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(fileName);
				xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
				bool flag = false;
				while (xmlTextReader.Read() && !flag)
				{
					XmlNodeType nodeType = xmlTextReader.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						continue;
					}
					if (xmlTextReader.Name == "Configuration")
					{
						configVersion = new Version(CommonData.GetAttributeNonNull(xmlTextReader, "ConfigVersion"));
						string attributeNonNull = CommonData.GetAttributeNonNull(xmlTextReader, "DownloadURL");
						if (attributeNonNull.Length > 0)
						{
							downloadURL = attributeNonNull;
							if (appendToDownloadURL)
							{
								downloadURL = string.Format("{0}/{1}.{2}", downloadURL, execInterface.EngineVersion.Major, execInterface.EngineVersion.Minor);
							}
							if (!downloadURL.StartsWith(downloadStart))
							{
								downloadURL = "http://invalidurlnamegiveninconfig";
							}
						}
						articleURL = CommonData.GetAttributeNonNull(xmlTextReader, "ArticleURL");
						if (articleURL.Length > 0 && (!articleURL.StartsWith(articleStart) || articleURL.IndexOf("{0}") == -1 || articleURL.IndexOf("{1}") == -1 || articleURL.IndexOf("{2}") == -1))
						{
							articleURL = "http://invalidurlnamegiveninconfig{0}{1}{2}";
						}
						helpFile = CommonData.GetAttributeNonNull(xmlTextReader, "HelpFile");
						if (helpFile.Length > 0)
						{
							helpFile = Directory.GetParent(execInterface.Options.Configuration.FileName).FullName + "\\" + helpFile;
						}
						string attributeNonNull2 = CommonData.GetAttributeNonNull(xmlTextReader, "ReferencedFiles");
						if (attributeNonNull2.Length > 0)
						{
							referencedFiles = attributeNonNull2.Split(',');
						}
					}
					else if (xmlTextReader.Name == "Object")
					{
						flag = true;
					}
				}
			}
			catch (Exception exception)
			{
				configVersion = new Version("0.0.0.0");
				execInterface.LogException(exception);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
				xmlTextReader = null;
			}
			if (!ConfigFound || !masterConfig)
			{
				return;
			}
			try
			{
				if (execInterface.Options.Configuration.IsEmpty())
				{
					execInterface.Options.Configuration.Load();
				}
				execInterface.Options.Restrictions.LoadValid(execInterface.Options.Configuration);
			}
			catch (Exception exception2)
			{
				execInterface.LogException(exception2);
			}
		}
	}
}
