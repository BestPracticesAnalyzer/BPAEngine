using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DownloadNewVersion : InProgressPanel
	{
		private object completionInfo;

		private MainGUI.Actions doneAction = MainGUI.Actions.NewConfigDownloadCompleted;

		private UpdateInfo updateInfo;

		public DownloadNewVersion(MainGUI mainGUI)
			: base(mainGUI)
		{
		}

		public void Done(object completionInfo)
		{
			if (!base.InvokeRequired)
			{
				this.completionInfo = completionInfo;
				StopTimerThread();
			}
			else
			{
				DownloadNewVersionAction.DoneDelegate method = Done;
				BeginInvoke(method, completionInfo);
			}
		}

		public bool Start(MainGUI.Actions downloadAction, UpdateInfo updateInfo)
		{
			this.updateInfo = updateInfo;
			ResetControls();
			if (downloadAction == MainGUI.Actions.DownloadNewConfig)
			{
				base.Title = BPALoc.Label_DTitle;
				base.PleaseWait = BPALoc.Label_DPleaseWait;
				base.EstimatedTime = BPALoc.Label_DEstimatedTime;
				base.Description = BPALoc.Label_DDesc(string.Format("{0}/{1}", mainGUI.ConfigInfo.DownloadURL, mainGUI.ExecInterface.Culture), mainGUI.Customizations.ShortName);
				base.EstimatedTimeTotal = TimeSpan.FromSeconds(300.0);
			}
			else
			{
				base.Title = BPALoc.Label_DVTitle;
				base.PleaseWait = BPALoc.Label_DVPleaseWait;
				base.EstimatedTime = BPALoc.Label_DVEstimatedTime;
				base.Description = BPALoc.Label_DVDesc(mainGUI.Customizations.ShortName, string.Format("{0}/{1}", mainGUI.ConfigInfo.DownloadURL, mainGUI.ExecInterface.Culture));
				base.EstimatedTimeTotal = TimeSpan.FromSeconds(450.0);
				doneAction = MainGUI.Actions.NewMSIDownloadCompleted;
			}
			StartTimerThread();
			DownloadNewVersionAction @object = new DownloadNewVersionAction(this, mainGUI, downloadAction, updateInfo);
			Thread thread = new Thread(@object.Start);
			thread.Start();
			return true;
		}

		public void StartMSI()
		{
			bool flag = false;
			try
			{
				ProcessStartInfo processStartInfo = new ProcessStartInfo();
				processStartInfo.FileName = string.Format("{0}\\{1}\\{2}.{3}", CommonData.GetRootDirectory(mainGUI.ExecInterface.Options.Configuration.FileName), mainGUI.ExecInterface.Culture, mainGUI.ExecInterface.ApplicationName, "msi");
				processStartInfo.UseShellExecute = true;
				Process.Start(processStartInfo);
				Thread.Sleep(1000);
				flag = true;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
			if (flag)
			{
				mainGUI.TakeAction(9, null, null, "");
			}
			else
			{
				mainGUI.TakeAction(12, null, null, BPALoc.Label_DVDownloadError(updateInfo.BinaryVersionInfo.ManualUrl));
			}
		}

		public void Status(int pctDone)
		{
			if (!base.InvokeRequired)
			{
				BaseStatus(pctDone);
				return;
			}
			UpdateInfo.StatusDelegate method = Status;
			BeginInvoke(method, pctDone);
		}

		public override void TimerDone()
		{
			if (!base.InvokeRequired)
			{
				mainGUI.TakeAction((int)doneAction, null, completionInfo, "");
				return;
			}
			TimerThread.DoneDelegate method = TimerDone;
			BeginInvoke(method, null);
		}
	}
}
