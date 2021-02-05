using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ExecutionInterface
	{
		public const int DefaultMajorVersion = 2;

		public const int DefaultMinorVersion = 5;

		public const string CONFIGPOSTFIX = "config.xml";

		public const string LOGPOSTFIX = "log";

		public const string REPORTPOSTFIX = "report.htm";

		public const string DATAPOSTFIX = "data.xml";

		private bool aborting;

		private ExecutionOptions options;

		private ExecutionEngine engine;

		private DateTime execTime = DateTime.Now;

		private StreamWriter logFile;

		private string exeDirectory = "";

		private string appName = "";

		private string culture = "EN";

		private Thread engineThread;

		private Version engineVersion;

		private CodeLibraries codeLibraries;

		private Impersonate impersonate;

		private Parameters parameters;

		private int majorVersion = 2;

		private int minorVersion = 5;

		public bool Aborting
		{
			get
			{
				return aborting;
			}
		}

		public ExecutionOptions Options
		{
			get
			{
				return options;
			}
		}

		public DateTime ExecTime
		{
			get
			{
				return execTime;
			}
		}

		public Version EngineVersion
		{
			get
			{
				return engineVersion;
			}
		}

		public string ExecutionDirectory
		{
			get
			{
				return exeDirectory;
			}
			set
			{
				exeDirectory = value;
			}
		}

		public string ApplicationName
		{
			get
			{
				return appName;
			}
		}

		public string Culture
		{
			get
			{
				return culture;
			}
		}

		public string DataFileNameSearchPattern
		{
			get
			{
				return string.Format("{0}.*.{1}", ApplicationName, "data.xml");
			}
		}

		public bool Trace
		{
			get
			{
				return options.Trace;
			}
		}

		public bool Debug
		{
			get
			{
				return options.Debug;
			}
		}

		public CodeLibraries CodeLibraries
		{
			get
			{
				return codeLibraries;
			}
		}

		public Impersonate ImpersonateInstance
		{
			get
			{
				return impersonate;
			}
		}

		public Parameters Parameters
		{
			get
			{
				return parameters;
			}
		}

		[Obsolete("The value ExecutionInterface.EnableLogFileCreation is no longer used.  Use ExecutionOptions.EnableLogFileCreation instead.")]
		public bool EnableLogFileCreation
		{
			get
			{
				return options.EnableLogFileCreation;
			}
			set
			{
				options.EnableLogFileCreation = false;
			}
		}

		[Obsolete("The value ExecutionInterface.CheckAssemblySignature is no longer used.  Use ExecutionOptions.CheckAssemblySignature instead.")]
		public bool CheckAssemblySignature
		{
			get
			{
				return options.CheckAssemblySignature;
			}
			set
			{
				options.CheckAssemblySignature = value;
			}
		}

		public ExecutionInterface(string appName)
			: this(appName, 2, 5)
		{
		}

		public ExecutionInterface(string appName, int majorVersion, int minorVersion)
		{
			Process currentProcess = Process.GetCurrentProcess();
			exeDirectory = Directory.GetParent(currentProcess.MainModule.FileName).FullName;
			engineVersion = new Version(majorVersion, minorVersion, FileVersionInfo.GetVersionInfo(currentProcess.MainModule.FileName).ProductBuildPart, FileVersionInfo.GetVersionInfo(currentProcess.MainModule.FileName).ProductPrivatePart);
			options = new ExecutionOptions(this);
			this.appName = appName;
			codeLibraries = new CodeLibraries(this);
			impersonate = new Impersonate();
			parameters = new Parameters(this);
			Common.ExecutionInterface = this;
			ProcessingInfo.ExecutionInterface = this;
			ExtFormat.ExecutionInterface = this;
			ExtFunction.ExecutionInterface = this;
			this.majorVersion = majorVersion;
			this.minorVersion = minorVersion;
		}

		public void Dispose()
		{
			LogFileClose();
		}

		public ExecutionStatus Start(bool async)
		{
			ExecutionStatus executionStatus = ExecutionStatus.InProgress;
			try
			{
				execTime = DateTime.Now;
				aborting = false;
				if (Options.Data.FileName.Length == 0)
				{
					SetDefaultDataFileName();
				}
				if (Options.EnableLogFileCreation)
				{
					LogFileCreate();
				}
				if (!ValidateAndLoadDocuments())
				{
					executionStatus = ExecutionStatus.Failed;
					return executionStatus;
				}
				engine = new ExecutionEngine(this);
				if (async)
				{
					engineThread = new Thread(engine.Start);
					engineThread.Start();
				}
				else
				{
					engine.Start();
				}
			}
			catch (Exception ex)
			{
				LogException(ex.Message, ex);
				executionStatus = ExecutionStatus.Failed;
			}
			finally
			{
				if (engine != null && executionStatus != ExecutionStatus.Failed)
				{
					executionStatus = engine.Status;
				}
				if (!async || executionStatus == ExecutionStatus.Failed)
				{
					Completed(executionStatus);
				}
			}
			return executionStatus;
		}

		public void Abort()
		{
			aborting = true;
		}

		public bool WaitForCompletion(int waitTime)
		{
			bool flag = true;
			if (engineThread != null)
			{
				flag = engineThread.Join(TimeSpan.FromSeconds(waitTime));
			}
			if (flag)
			{
				engineThread = null;
			}
			return flag;
		}

		public void WaitForCompletion()
		{
			if (engineThread != null)
			{
				engineThread.Join();
			}
			engineThread = null;
		}

		public void SetDefaultConfigurationFileName()
		{
			string arg = string.Format("{0}.{1}", ApplicationName, "config.xml");
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			culture = currentUICulture.ToString();
			string text = string.Format("{0}\\{1}\\{2}", ExecutionDirectory, culture, arg);
			if (!File.Exists(text))
			{
				culture = currentUICulture.Parent.ToString();
				text = string.Format("{0}\\{1}\\{2}", ExecutionDirectory, culture, arg);
				if (!File.Exists(text))
				{
					culture = "EN";
					text = string.Format("{0}\\{1}\\{2}", ExecutionDirectory, culture, arg);
				}
			}
			Options.Configuration.FileName = text;
		}

		public void SetDefaultDataFileName()
		{
			string text = string.Format("{0:yyyyMMddHHmmssffff}.{1}", execTime, "data.xml");
			if (options.Label.Length > 0)
			{
				text = string.Format("{0}.{1}", Options.Label, text);
			}
			text = string.Format("{0}\\{1}.{2}", Directory.GetCurrentDirectory(), ApplicationName, text);
			Options.Data.FileName = text;
		}

		public string GetDefaultOutputFileName(string postfix)
		{
			string fileName = Options.Data.FileName;
			if (fileName.EndsWith("data.xml"))
			{
				return Options.Data.FileName.Substring(0, Options.Data.FileName.Length - "data.xml".Length) + postfix;
			}
			return fileName + "." + postfix;
		}

		public void LogTrace(string text)
		{
			if (options.Trace)
			{
				LogText(text);
			}
		}

		public void LogTrace(string format, params object[] args)
		{
			LogTrace(string.Format(format, args));
		}

		public void LogDebug(string text)
		{
			if (options.Debug)
			{
				LogText(text);
			}
		}

		public void LogDebug(string format, params object[] args)
		{
			LogDebug(string.Format(format, args));
		}

		public void LogException(Exception exception)
		{
			LogText(exception.Message);
			if (options.Debug)
			{
				LogText("\n" + exception.StackTrace);
			}
		}

		public void LogException(string text, Exception exception)
		{
			LogText(text);
			if (options.Debug)
			{
				LogText("\n" + exception.StackTrace);
			}
		}

		public void LogText(string text)
		{
			string text2 = string.Format("{0:HH:mm:ss.fff}", DateTime.Now);
			string text3 = string.Format("{0}: {1}", text2, text);
			if (engine != null && engine.Status == ExecutionStatus.InProgress)
			{
				try
				{
					options.Data.Log(text2, text);
				}
				catch
				{
				}
			}
			try
			{
				if (logFile != null)
				{
					lock (logFile)
					{
						logFile.WriteLine(text3);
					}
				}
			}
			catch
			{
			}
			try
			{
				if (options.Log != null)
				{
					if (options.StripLogTimestamps)
					{
						options.Log(text);
					}
					else
					{
						options.Log(text3);
					}
				}
			}
			catch
			{
			}
		}

		public void LogText(string format, params object[] args)
		{
			LogText(string.Format(format, args));
		}

		internal void Completed(ExecutionStatus status)
		{
			LogFileClose();
			if (Options.Completed != null)
			{
				Options.Completed(status);
			}
		}

		internal void LoadSecurityContexts()
		{
			impersonate.Clear();
			foreach (Credentials value in options.CredentialsList.Values)
			{
				impersonate.AddSecurityContext(value);
			}
		}

		private void LogFileCreate()
		{
			string defaultOutputFileName = GetDefaultOutputFileName("log");
			logFile = new StreamWriter(defaultOutputFileName, true);
			logFile.AutoFlush = true;
		}

		private void LogFileClose()
		{
			if (logFile != null)
			{
				logFile.Close();
			}
			logFile = null;
		}

		private bool ValidateAndLoadDocuments()
		{
			bool result = true;
			if (Options.ReloadConfiguration)
			{
				if (Options.Configuration.FileName.Length == 0)
				{
					SetDefaultConfigurationFileName();
				}
				if (!File.Exists(Options.Configuration.FileName))
				{
					LogText(CommonLoc.Error_BadCfgParam(Options.Configuration.FileName));
					result = false;
				}
				else
				{
					((XmlDocument)Options.Configuration.UnderlyingDocument).PreserveWhitespace = true;
					Options.Configuration.Load();
					ValidateConfigurations validateConfigurations = new ValidateConfigurations(this);
					if (!validateConfigurations.Validate(Options.Configuration))
					{
						Options.Configuration.ClearDocument();
						LogText(validateConfigurations.ValidationError);
						result = false;
					}
				}
			}
			lock (this)
			{
				Options.Configuration.ConfigurationNode = Options.Configuration.GetNode("/*/Configuration");
			}
			try
			{
				string directoryName = Path.GetDirectoryName(Options.Data.FileName);
				if (directoryName.Length > 0)
				{
					if (!Directory.Exists(directoryName))
					{
						throw new DirectoryNotFoundException();
					}
					return result;
				}
				return result;
			}
			catch (Exception ex)
			{
				LogText(CommonLoc.Error_BadDataParam(ex.Message, Options.Data.FileName));
				return false;
			}
		}
	}
}
