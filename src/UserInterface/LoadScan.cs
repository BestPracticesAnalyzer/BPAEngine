using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class LoadScan : InProgressPanel
	{
		private DataInfo dataInfo;

		public LoadScan(MainGUI mainGUI, string title, string pleaseWait, string description)
			: base(mainGUI)
		{
			base.Title = title;
			base.PleaseWait = pleaseWait;
			base.EstimatedTime = BPALoc.Label_DVEstimatedTime;
			base.Description = description;
		}

		public LoadScan(MainGUI mainGUI)
			: this(mainGUI, mainGUI.Customizations.LoadScanTitle, string.Format(mainGUI.Customizations.LoadScanPleaseWait, mainGUI.Customizations.ShortName), mainGUI.Customizations.LoadScanDescription)
		{
		}

		public void AnalysisDone(ExecutionStatus status)
		{
			if (!base.InvokeRequired)
			{
				mainGUI.ExecInterface.Options.Data.Save();
				mainGUI.ExecInterface.Options.Data.ClearDocument();
				dataInfo.Analyzed = true;
				dataInfo.Completed = true;
				dataInfo.ConfigVersion = mainGUI.ConfigInfo.ConfigVersion;
				DoLoadScan(base.PercentComplete);
			}
			else
			{
				CompletedCallback method = AnalysisDone;
				BeginInvoke(method, status);
			}
		}

		public void AnalysisStatus(string server, ObjectProgress progress)
		{
		}

		public void LoadDone()
		{
			if (!base.InvokeRequired)
			{
				StopTimerThread();
				return;
			}
			LoadScanAction.DoneDelegate method = LoadDone;
			BeginInvoke(method, null);
		}

		public void LoadStatus(int pctDone)
		{
			if (!base.InvokeRequired)
			{
				BaseStatus(pctDone);
				return;
			}
			DataInfo.StatusDelegate method = LoadStatus;
			BeginInvoke(method, pctDone);
		}

		public bool Start(DataInfo dataInfo, Keys keyPressed)
		{
			bool flag = false;
			if (mainGUI.Customizations.AllowReanalysis)
			{
				flag = !dataInfo.Analyzed || ReanalyzeNeeded(dataInfo.ConfigVersion);
				switch (keyPressed)
				{
				case Keys.ControlKey:
					flag = false;
					break;
				case Keys.ShiftKey:
					flag = true;
					break;
				}
			}
			this.dataInfo = dataInfo;
			base.EstimatedTimeTotal = TimeSpan.FromSeconds((int)(dataInfo.FileSize / 1200000));
			if (flag)
			{
				base.EstimatedTimeTotal = base.EstimatedTimeTotal.Add(base.EstimatedTimeTotal);
				base.EstimatedTimeTotal = base.EstimatedTimeTotal.Add(TimeSpan.FromSeconds(120.0));
			}
			StartTimerThread();
			if (flag)
			{
				mainGUI.ExecInterface.Options.Operations = OperationsFlags.Analyze;
				mainGUI.ExecInterface.Options.Data.FileName = dataInfo.FileName;
				mainGUI.ExecInterface.Options.Restrictions.DisableAll();
				mainGUI.ExecInterface.Options.Restrictions.EnableOptions(dataInfo.Restriction);
				mainGUI.ExecInterface.Options.SaveInterval = 0;
				mainGUI.ExecInterface.Options.LoadDataOnRun = true;
				mainGUI.ExecInterface.Options.Progress = AnalysisStatus;
				mainGUI.ExecInterface.Options.Completed = AnalysisDone;
				mainGUI.ExecInterface.Start(true);
			}
			else
			{
				DoLoadScan(0);
			}
			return true;
		}

		public override void TimerDone()
		{
			if (!base.InvokeRequired)
			{
				mainGUI.TakeAction(26, null, null, "");
				return;
			}
			TimerThread.DoneDelegate method = TimerDone;
			BeginInvoke(method, null);
		}

		private void DoLoadScan(int pctComplete)
		{
			LoadScanAction @object = new LoadScanAction(this, mainGUI, dataInfo, pctComplete);
			Thread thread = new Thread(@object.Start);
			thread.Start();
		}

		private bool ReanalyzeNeeded(Version scanVersion)
		{
			Version configVersion = mainGUI.ConfigInfo.ConfigVersion;
			if (scanVersion.Major == configVersion.Major && scanVersion.Minor == configVersion.Minor && scanVersion.Build == configVersion.Build && scanVersion.CompareTo(configVersion) < 0)
			{
				return true;
			}
			return false;
		}
	}
}
