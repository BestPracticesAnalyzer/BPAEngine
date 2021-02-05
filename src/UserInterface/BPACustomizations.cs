using System;
using System.Collections;
using System.Drawing;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPACustomizations
	{
		public delegate void GetBPAScanInfoCallback(BPAScanInfo scanInfo, string[] argsIn);

		public delegate void ScheduleInfoCallback(ScheduleInfo scheduleInfo);

		public delegate void ProcessConfigurationCallback(Document config);

		private GetBPAScanInfoCallback getBPAScanInfo;

		private ScheduleInfoCallback updateScheduleInfo;

		private ProcessConfigurationCallback processConfiguration;

		private ArrayList customScreens;

		private ArrayList customReports;

		private string button_AppWebSite;

		private string appWebSite = "";

		private string applicationName = "";

		private string cmdAppName = "";

		private string longNameStart = "";

		private string longNameEnd = "";

		private string shortName = "";

		private string description = "";

		private string startScanLink = BPALoc.LinkLabel_WSelectOptions;

		private string selectScanLink = BPALoc.LinkLabel_WSelectScan;

		private string rootHelpId = "";

		private string checkForUpdatesHelpId;

		private string checkForUpdatesDescription = BPALoc.GetString("Label_CDesc");

		private string checkForUpdatesLabel = BPALoc.Label_CPleaseWait;

		private string versionUpdateHelpId;

		private string versionUpdateLabel = BPALoc.LinkLabel_NVDownloadConfig;

		private string downloadNewVersionHelpId;

		private string welcomeHelpId;

		private string scheduleOptionsHelpId;

		private string startScanHelpId;

		private string scanCompletedHelpId;

		private string selectScanHelpId;

		private string loadScanDescription = BPALoc.Lavel_GVDesc;

		private string loadScanHelpId;

		private string loadScanPleaseWait = BPALoc.GetString("Label_GVPleaseWait");

		private string loadScanTitle = BPALoc.Label_GVTitle;

		private string viewScanHelpId;

		private string viewScanTitle = BPALoc.Label_VSTitle;

		private string registryKey;

		private string defaultDataDirectory;

		private CustomRegistrySettings registrySettings;

		private bool allowAutoDownloads = true;

		private bool allowDetailedArticleLinks = true;

		private bool[] allowReport;

		private bool checkAssemblySignature = true;

		private bool checkConfigurationSignature = true;

		private string configurationPublicKey;

		private Image objectPic;

		private string downloadURLStart;

		private string articleURLStart;

		private bool appendToDownloadURL = true;

		private LogCallback logFunction;

		private bool allowReanalysis = true;

		public ScheduleInfoCallback UpdateScheduleInfo
		{
			get
			{
				return updateScheduleInfo;
			}
			set
			{
				updateScheduleInfo = value;
			}
		}

		public GetBPAScanInfoCallback GetBPAScanInfo
		{
			get
			{
				return getBPAScanInfo;
			}
		}

		public ProcessConfigurationCallback ProcessConfiguration
		{
			get
			{
				return processConfiguration;
			}
			set
			{
				processConfiguration = value;
			}
		}

		public ArrayList CustomScreens
		{
			get
			{
				return customScreens;
			}
		}

		public ArrayList CustomReports
		{
			get
			{
				return customReports;
			}
		}

		public string Button_AppWebSite
		{
			get
			{
				return button_AppWebSite;
			}
			set
			{
				button_AppWebSite = value;
			}
		}

		public string AppWebSite
		{
			get
			{
				return appWebSite;
			}
			set
			{
				appWebSite = value;
			}
		}

		public string ApplicationName
		{
			get
			{
				return applicationName;
			}
		}

		public string CommandLineApplicationName
		{
			get
			{
				return cmdAppName;
			}
			set
			{
				cmdAppName = value;
			}
		}

		public string LongNameStart
		{
			get
			{
				return longNameStart;
			}
			set
			{
				longNameStart = value;
			}
		}

		public string LongNameEnd
		{
			get
			{
				return longNameEnd;
			}
			set
			{
				longNameEnd = value;
			}
		}

		public string ShortName
		{
			get
			{
				return shortName;
			}
			set
			{
				shortName = value;
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

		public string StartScanLink
		{
			get
			{
				return startScanLink;
			}
			set
			{
				startScanLink = value;
			}
		}

		public string SelectScanLink
		{
			get
			{
				return selectScanLink;
			}
			set
			{
				selectScanLink = value;
			}
		}

		public string RootHelpId
		{
			get
			{
				return rootHelpId;
			}
			set
			{
				rootHelpId = value;
			}
		}

		public string CheckForUpdatesHelpId
		{
			get
			{
				return checkForUpdatesHelpId;
			}
			set
			{
				checkForUpdatesHelpId = value;
			}
		}

		public string CheckForUpdatesDescription
		{
			get
			{
				return checkForUpdatesDescription;
			}
			set
			{
				checkForUpdatesDescription = value;
			}
		}

		public string CheckForUpdatesLabel
		{
			get
			{
				return checkForUpdatesLabel;
			}
			set
			{
				checkForUpdatesLabel = value;
			}
		}

		public string VersionUpdateHelpId
		{
			get
			{
				return versionUpdateHelpId;
			}
			set
			{
				versionUpdateHelpId = value;
			}
		}

		public string VersionUpdateLabel
		{
			get
			{
				return versionUpdateLabel;
			}
			set
			{
				versionUpdateLabel = value;
			}
		}

		public string DownloadNewVersionHelpId
		{
			get
			{
				return downloadNewVersionHelpId;
			}
			set
			{
				downloadNewVersionHelpId = value;
			}
		}

		public string WelcomeHelpId
		{
			get
			{
				return welcomeHelpId;
			}
			set
			{
				welcomeHelpId = value;
			}
		}

		public string ScheduleOptionsHelpId
		{
			get
			{
				return scheduleOptionsHelpId;
			}
			set
			{
				scheduleOptionsHelpId = value;
			}
		}

		public string StartScanHelpId
		{
			get
			{
				return startScanHelpId;
			}
			set
			{
				startScanHelpId = value;
			}
		}

		public string ScanCompletedHelpId
		{
			get
			{
				return scanCompletedHelpId;
			}
			set
			{
				scanCompletedHelpId = value;
			}
		}

		public string SelectScanHelpId
		{
			get
			{
				return selectScanHelpId;
			}
			set
			{
				selectScanHelpId = value;
			}
		}

		public string LoadScanDescription
		{
			get
			{
				return loadScanDescription;
			}
			set
			{
				loadScanDescription = value;
			}
		}

		public string LoadScanHelpId
		{
			get
			{
				return loadScanHelpId;
			}
			set
			{
				loadScanHelpId = value;
			}
		}

		public string LoadScanPleaseWait
		{
			get
			{
				return loadScanPleaseWait;
			}
			set
			{
				loadScanPleaseWait = value;
			}
		}

		public string LoadScanTitle
		{
			get
			{
				return loadScanTitle;
			}
			set
			{
				loadScanTitle = value;
			}
		}

		public string ViewScanHelpId
		{
			get
			{
				return viewScanHelpId;
			}
			set
			{
				viewScanHelpId = value;
			}
		}

		public string ViewScanTitle
		{
			get
			{
				return viewScanTitle;
			}
			set
			{
				viewScanTitle = value;
			}
		}

		public string RegistryKey
		{
			get
			{
				return registryKey;
			}
			set
			{
				registryKey = value;
			}
		}

		public string DefaultDataDirectory
		{
			get
			{
				return defaultDataDirectory;
			}
			set
			{
				defaultDataDirectory = value;
			}
		}

		public CustomRegistrySettings RegistrySettings
		{
			get
			{
				return registrySettings;
			}
			set
			{
				registrySettings = value;
			}
		}

		public bool AllowAutoDownloads
		{
			get
			{
				return allowAutoDownloads;
			}
			set
			{
				allowAutoDownloads = value;
			}
		}

		public bool AllowDetailedArticleLinks
		{
			get
			{
				return allowDetailedArticleLinks;
			}
			set
			{
				allowDetailedArticleLinks = value;
			}
		}

		public bool[] AllowReport
		{
			get
			{
				return allowReport;
			}
			set
			{
				allowReport = value;
			}
		}

		public bool CheckAssemblySignature
		{
			get
			{
				return checkAssemblySignature;
			}
			set
			{
				checkAssemblySignature = value;
			}
		}

		public bool CheckConfigurationSignature
		{
			get
			{
				return checkConfigurationSignature;
			}
			set
			{
				checkConfigurationSignature = value;
			}
		}

		public string ConfigurationPublicKey
		{
			get
			{
				return configurationPublicKey;
			}
			set
			{
				configurationPublicKey = value;
			}
		}

		public Image ObjectPic
		{
			get
			{
				if (objectPic != null)
				{
					return objectPic;
				}
				return CommonData.DefaultObjectPic;
			}
			set
			{
				objectPic = value;
			}
		}

		public string DownloadURLStart
		{
			get
			{
				return downloadURLStart;
			}
			set
			{
				downloadURLStart = value;
			}
		}

		public string ArticleURLStart
		{
			get
			{
				return articleURLStart;
			}
			set
			{
				articleURLStart = value;
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

		public LogCallback LogFunction
		{
			get
			{
				return logFunction;
			}
			set
			{
				logFunction = value;
			}
		}

		public bool AllowReanalysis
		{
			get
			{
				return allowReanalysis;
			}
			set
			{
				allowReanalysis = value;
			}
		}

		public BPACustomizations(string applicationName, GetBPAScanInfoCallback getBPAScanInfo)
		{
			this.getBPAScanInfo = getBPAScanInfo;
			customScreens = new ArrayList();
			customReports = new ArrayList();
			allowReport = new bool[11];
			for (int i = 0; i < allowReport.Length; i++)
			{
				allowReport[i] = true;
			}
			this.applicationName = applicationName;
			registryKey = string.Format("Software\\Microsoft\\{0}", applicationName);
			defaultDataDirectory = string.Format("{0}\\Microsoft\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);
		}
	}
}
