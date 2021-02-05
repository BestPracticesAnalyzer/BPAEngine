using System;
using System.Collections;
using System.IO;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class MainCmd
	{
		protected string[] args;

		protected BPACustomizations customizations;

		protected ExecutionInterface mexecInterface;

		protected RegistrySettings regSettings;

		private object logLock;

		public ExecutionInterface ExecInterface
		{
			get
			{
				return mexecInterface;
			}
		}

		public MainCmd(string[] args, BPACustomizations customizations)
			: this(args, customizations, 2, 5)
		{
		}

		public MainCmd(string[] args, BPACustomizations customizations, int majorVersion, int minorVersion)
		{
			string argumentValue = CommandLineParameters.GetArgumentValue(args, "MAJOR");
			if (argumentValue.Length > 0)
			{
				majorVersion = int.Parse(argumentValue);
			}
			string argumentValue2 = CommandLineParameters.GetArgumentValue(args, "MINOR");
			if (argumentValue2.Length > 0)
			{
				minorVersion = int.Parse(argumentValue2);
			}
			if (CommandLineParameters.ArgumentSet(args, "NOCCS"))
			{
				customizations.CheckConfigurationSignature = false;
			}
			this.args = args;
			this.customizations = customizations;
			mexecInterface = new ExecutionInterface(customizations.ApplicationName, majorVersion, minorVersion);
			logLock = new object();
		}

		public virtual ExecutionStatus Start()
		{
			ExecutionStatus executionStatus = ExecutionStatus.InProgress;
			try
			{
				executionStatus = Initialize();
				if (executionStatus == ExecutionStatus.InProgress)
				{
					mexecInterface.Options.Progress = StatusReport;
					mexecInterface.Options.Completed = DoneExecuting;
					if (mexecInterface.Options.Data.FileName.Length == 0)
					{
						string currentDirectory = Directory.GetCurrentDirectory();
						Directory.SetCurrentDirectory(regSettings.DataDirectory);
						mexecInterface.SetDefaultDataFileName();
						Directory.SetCurrentDirectory(currentDirectory);
					}
					executionStatus = mexecInterface.Start(true);
					mexecInterface.WaitForCompletion();
				}
			}
			catch (Exception ex)
			{
				mexecInterface.LogException(CommonLoc.Error_Executing(ex.Message), ex);
				executionStatus = ExecutionStatus.Failed;
			}
			if (executionStatus == ExecutionStatus.InProgress || executionStatus == ExecutionStatus.Okay)
			{
				try
				{
					mexecInterface.Options.Data.Save();
				}
				catch (Exception ex2)
				{
					mexecInterface.LogException(CommonLoc.Error_XMLWrite(ex2.Message), ex2);
					executionStatus = ExecutionStatus.Failed;
				}
			}
			if (executionStatus == ExecutionStatus.InProgress)
			{
				executionStatus = ExecutionStatus.Okay;
			}
			return executionStatus;
		}

		protected ExecutionStatus Initialize()
		{
			ExecutionStatus result = ExecutionStatus.InProgress;
			try
			{
				if (customizations.LogFunction == null)
				{
					mexecInterface.Options.Log = Output;
				}
				else
				{
					mexecInterface.Options.Log = customizations.LogFunction;
				}
				if (CommandLineParameters.HelpSet(args))
				{
					CommandLineParameters commandLineParameters = new CommandLineParameters(mexecInterface);
					commandLineParameters.ParseArgs(args);
					return ExecutionStatus.Aborted;
				}
				mexecInterface.Options.CheckAssemblySignature = customizations.CheckAssemblySignature;
				mexecInterface.Options.CheckConfigurationSignature = customizations.CheckConfigurationSignature;
				if (CommandLineParameters.ArgumentSet(args, "S"))
				{
					string[] array = null;
					try
					{
						array = Schedule.LoadSchedule(mexecInterface);
					}
					catch (Exception ex)
					{
						if (args.Length <= 1)
						{
							throw;
						}
						mexecInterface.LogException(CommonLoc.Error_Executing(ex.Message), ex);
						array = new string[0];
					}
					string[] array2 = new string[args.Length + array.Length];
					args.CopyTo(array2, 0);
					array.CopyTo(array2, args.Length);
					args = array2;
				}
				ArrayList argumentValues = CommandLineParameters.GetArgumentValues(args, "UK", 1, false);
				string userKeyName = "";
				if (argumentValues != null && argumentValues.Count > 0)
				{
					userKeyName = argumentValues[0].ToString();
				}
				regSettings = new RegistrySettings(mexecInterface, customizations, userKeyName);
				try
				{
					if (!CommandLineParameters.ArgumentSet(args, "ND") && customizations.AllowAutoDownloads && regSettings.VersionCheckAlways && CommandLineParameters.GetArgumentValues(args, "CFG", 1, false) == null)
					{
						mexecInterface.SetDefaultConfigurationFileName();
						ConfigurationInfo configurationInfo = new ConfigurationInfo(mexecInterface);
						if (!configurationInfo.ConfigFound)
						{
							mexecInterface.LogText(BPALoc.Label_NVNoConfig);
							return ExecutionStatus.Failed;
						}
						UpdateInfo updateInfo = new UpdateInfo(mexecInterface, configurationInfo);
						updateInfo.CheckForUpdates();
						if (updateInfo.ConfigVersionInfo.Found && updateInfo.ConfigVersionInfo.NewerThanLocal)
						{
							mexecInterface.LogText(BPALoc.Label_DDesc(string.Format("{0}/{1}", configurationInfo.DownloadURL, mexecInterface.Culture), customizations.ShortName));
							updateInfo.DownloadConfig(DownloadStatus, customizations.AllowDetailedArticleLinks);
						}
					}
				}
				catch (Exception exception)
				{
					mexecInterface.LogException(exception);
				}
				BPAScanInfo scanInfo = new BPAScanInfo(mexecInterface.Options);
				customizations.GetBPAScanInfo(scanInfo, args);
				CommandLineParameters commandLineParameters2 = new CommandLineParameters(mexecInterface);
				if (!commandLineParameters2.ParseArgs(args))
				{
					return ExecutionStatus.Aborted;
				}
				if (customizations.ProcessConfiguration != null)
				{
					customizations.ProcessConfiguration(mexecInterface.Options.Configuration);
					return result;
				}
				return result;
			}
			catch (Exception exception2)
			{
				mexecInterface.LogException(exception2);
				return ExecutionStatus.Failed;
			}
		}

		protected void DownloadStatus(int pctDone)
		{
			Console.Write("%{0}\r", pctDone);
		}

		protected void StatusReport(string name, ObjectProgress progress)
		{
		}

		protected void DoneExecuting(ExecutionStatus status)
		{
		}

		protected void Output(string message)
		{
			lock (logLock)
			{
				Console.WriteLine(message);
			}
		}
	}
}
