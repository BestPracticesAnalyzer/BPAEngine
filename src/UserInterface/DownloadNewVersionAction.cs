using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class DownloadNewVersionAction
	{
		public delegate void DoneDelegate(object completionInfo);

		private DownloadNewVersion downloadNewVersion;

		private MainGUI mainGUI;

		private MainGUI.Actions downloadAction = MainGUI.Actions.DownloadNewConfig;

		private UpdateInfo updateInfo;

		public DownloadNewVersionAction(DownloadNewVersion downloadNewVersion, MainGUI mainGUI, MainGUI.Actions downloadAction, UpdateInfo updateInfo)
		{
			this.downloadNewVersion = downloadNewVersion;
			this.mainGUI = mainGUI;
			this.downloadAction = downloadAction;
			this.updateInfo = updateInfo;
		}

		public void Start()
		{
			if (downloadAction == MainGUI.Actions.DownloadNewConfig)
			{
				updateInfo.DownloadConfig(downloadNewVersion.Status, mainGUI.Customizations.AllowDetailedArticleLinks);
			}
			else
			{
				updateInfo.DownloadBinaries(downloadNewVersion.Status);
			}
			mainGUI.ReloadConfigInfo();
			downloadNewVersion.Done(null);
			Application.ExitThread();
		}
	}
}
