using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class Analyzer : Form
	{
		public const int MajorVersion = 1;

		protected IContainer components;

		protected MainGUI mainGUI;

		protected string[] margs;

		protected string mconfigurationFileName;

		protected BPACustomizations mcustomizations;

		protected string Label_SSTitle = "Start a Scan";

		protected string Label_SSADServer = "Enter the AD server:";

		protected string Tool_ShortName = BPALoc.Tool_FullName;

		protected string Tool_LongNameStart = BPALoc.Tool_LongNameStart;

		protected string Tool_LongNameEnd = BPALoc.Tool_LongNameEnd;

		protected string Label_WDesc = BPALoc.Label_WDesc;

		protected string Button_StartScan = BPALoc.Button_StartScan;

		protected string Label_SSLabel = BPALoc.Label_SSLabel;

		public string ShortName
		{
			get
			{
				return Tool_ShortName;
			}
			set
			{
				Tool_ShortName = value;
			}
		}

		public string LongNameStart
		{
			get
			{
				return Tool_LongNameStart;
			}
			set
			{
				Tool_LongNameStart = value;
			}
		}

		public string LongNameEnd
		{
			get
			{
				return Tool_LongNameEnd;
			}
			set
			{
				Tool_LongNameEnd = value;
			}
		}

		public string Description
		{
			get
			{
				return Label_WDesc;
			}
			set
			{
				Label_WDesc = value;
			}
		}

		public string StartScan
		{
			get
			{
				return Button_StartScan;
			}
			set
			{
				Button_StartScan = value;
			}
		}

		public string SSLabel
		{
			get
			{
				return Label_SSLabel;
			}
			set
			{
				Label_SSLabel = value;
			}
		}

		public MainGUI CommonGUI
		{
			get
			{
				return mainGUI;
			}
			set
			{
				mainGUI = value;
			}
		}

		public string[] Args
		{
			get
			{
				return margs;
			}
		}

		public BPACustomizations Customizations
		{
			get
			{
				return mcustomizations;
			}
			set
			{
				mcustomizations = value;
			}
		}

		public BPARegistrySettings CustomRegistrySettings
		{
			get
			{
				return (BPARegistrySettings)mcustomizations.RegistrySettings;
			}
		}

		public string ConfigurationFileName
		{
			get
			{
				return mconfigurationFileName;
			}
		}

		public string SSTitle
		{
			get
			{
				return Label_SSTitle;
			}
			set
			{
				Label_SSTitle = value;
			}
		}

		public string SSADServer
		{
			get
			{
				return Label_SSADServer;
			}
			set
			{
				Label_SSADServer = value;
			}
		}

		public Analyzer(string[] args)
		{
			mconfigurationFileName = null;
			margs = args;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
			Kernel32.ExitProcess(0);
		}

		public override string ToString()
		{
			return LongNameStart + " " + LongNameEnd;
		}

		public virtual void InitializeAnalyzerCustomizations()
		{
			mcustomizations = new BPACustomizations("BPA", GetBPAScanInfo);
			mcustomizations.ProcessConfiguration = ProcessConfiguration;
			mcustomizations.Button_AppWebSite = BPALoc.Button_Exchange(Tool_ShortName);
			mcustomizations.AppWebSite = null;
			mcustomizations.CommandLineApplicationName = "bpacmd.exe";
			mcustomizations.ShortName = Tool_ShortName;
			mcustomizations.LongNameStart = Tool_LongNameStart;
			mcustomizations.LongNameEnd = Tool_LongNameEnd;
			mcustomizations.RegistrySettings = new BPARegistrySettings();
			mcustomizations.AllowAutoDownloads = false;
			mcustomizations.Description = Label_WDesc;
			mcustomizations.CheckAssemblySignature = false;
			mcustomizations.CheckConfigurationSignature = false;
		}

		public virtual void Initialize()
		{
			components = new Container();
		}

		protected virtual void GetBPAScanInfo(BPAScanInfo scanInfo, string[] argsIn)
		{
		}

		protected virtual void ProcessConfiguration(Document doc)
		{
		}
	}
}
