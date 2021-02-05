using System;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class CheckForUpdatesAction
	{
		public delegate void DoneDelegate(UpdateInfo updateInfo);

		public delegate void StatusDelegate(int pctDone);

		private CheckForUpdates checkForUpdates;

		private MainGUI mainGUI;

		public CheckForUpdatesAction(CheckForUpdates checkForUpdates, MainGUI mainGUI)
		{
			this.checkForUpdates = checkForUpdates;
			this.mainGUI = mainGUI;
		}

		public void Start()
		{
			UpdateInfo updateInfo = null;
			try
			{
				updateInfo = new UpdateInfo(mainGUI.ExecInterface, mainGUI.ConfigInfo);
				updateInfo.ExtendedData = new CheckForUpdates.UpdateInfoExtendedData();
				updateInfo.CheckForUpdates();
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
			finally
			{
				if (updateInfo != null && updateInfo.ExtendedData != null && !updateInfo.ConfigVersionInfo.Found)
				{
					((CheckForUpdates.UpdateInfoExtendedData)updateInfo.ExtendedData).CheckError = true;
				}
			}
			checkForUpdates.Done(updateInfo);
			Application.ExitThread();
		}
	}
}
