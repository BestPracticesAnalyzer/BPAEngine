using System;
using System.Drawing;
using System.Threading;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class CheckForUpdates : InProgressPanel
	{
		public class UpdateInfoExtendedData
		{
			private bool checkCanceled;

			private bool checkError;

			public bool CheckCanceled
			{
				get
				{
					return checkCanceled;
				}
				set
				{
					checkCanceled = value;
				}
			}

			public bool CheckError
			{
				get
				{
					return checkError;
				}
				set
				{
					checkError = value;
				}
			}
		}

		private UpdateInfo updateInfo;

		private bool checkAborted;

		private Thread inProgThread;

		private object checkThreadLock = new object();

		private object abortLock = new object();

		public CheckForUpdates(MainGUI mainGUI)
			: this(mainGUI, mainGUI.Customizations.CheckForUpdatesLabel, string.Format(mainGUI.Customizations.CheckForUpdatesDescription, mainGUI.Customizations.ShortName, string.Format("{0}{1}", mainGUI.ConfigInfo.DownloadURL, mainGUI.Customizations.AppendToDownloadURL ? ("/" + mainGUI.ExecInterface.Culture) : "")))
		{
		}

		public CheckForUpdates(MainGUI mainGUI, string label, string description)
			: base(mainGUI)
		{
			base.Title = BPALoc.Label_CTitle;
			base.PleaseWait = label;
			base.EstimatedTime = BPALoc.Label_CEstimatedTime;
			base.Description = description;
			Point nextLocationPoint = base.NextLocationPoint;
			nextLocationPoint = Navigate.Indent(nextLocationPoint);
			BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.StopCheckVersion, false, BPALoc.LinkLabel_CStopCheck, mainGUI.ArrowPic, nextLocationPoint, 0, this);
			bPALink.SetTabIndex(nextTabIndex++);
			nextLocationPoint = Navigate.Below(bPALink);
		}

		public void Done(UpdateInfo updateInfo)
		{
			if (!base.InvokeRequired)
			{
				this.updateInfo = updateInfo;
				StopTimerThread();
			}
			else
			{
				CheckForUpdatesAction.DoneDelegate method = Done;
				BeginInvoke(method, updateInfo);
			}
		}

		public override bool Start()
		{
			if (!Monitor.TryEnter(checkThreadLock))
			{
				return false;
			}
			updateInfo = new UpdateInfo(mainGUI.ExecInterface, mainGUI.ConfigInfo);
			updateInfo.ExtendedData = new UpdateInfoExtendedData();
			base.EstimatedTimeTotal = TimeSpan.FromSeconds(60.0);
			StartTimerThread();
			CheckForUpdatesAction @object = new CheckForUpdatesAction(this, mainGUI);
			inProgThread = new Thread(@object.Start);
			inProgThread.Start();
			return true;
		}

		public void Status(int pctDone)
		{
			if (!base.InvokeRequired)
			{
				BaseStatus(pctDone);
				return;
			}
			CheckForUpdatesAction.StatusDelegate method = Status;
			BeginInvoke(method, pctDone);
		}

		public void StopCheck()
		{
			lock (abortLock)
			{
				if (inProgThread != null && !checkAborted)
				{
					checkAborted = true;
					inProgThread.Abort();
					updateInfo = new UpdateInfo(mainGUI.ExecInterface, mainGUI.ConfigInfo);
					updateInfo.ExtendedData = new UpdateInfoExtendedData();
					((UpdateInfoExtendedData)updateInfo.ExtendedData).CheckCanceled = true;
					mainGUI.TakeAction(4, null, updateInfo, "");
				}
			}
		}

		public override void TimerDone()
		{
			if (!base.InvokeRequired)
			{
				lock (abortLock)
				{
					if (!checkAborted)
					{
						mainGUI.TakeAction(4, null, updateInfo, "");
					}
					inProgThread = null;
					checkAborted = false;
				}
				Monitor.Exit(checkThreadLock);
			}
			else
			{
				TimerThread.DoneDelegate method = TimerDone;
				BeginInvoke(method, null);
			}
		}
	}
}
