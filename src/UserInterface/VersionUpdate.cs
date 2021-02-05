using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class VersionUpdate : BPAScreen
	{
		private UpdateInfo updateInfo;

		private string downloadConfigLabel;

		public VersionUpdate(MainGUI mainGUI)
			: this(mainGUI, mainGUI.Customizations.VersionUpdateLabel)
		{
		}

		public VersionUpdate(MainGUI mainGUI, string downloadConfigLabel)
			: base(mainGUI)
		{
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			this.downloadConfigLabel = downloadConfigLabel;
		}

		public void AddControls()
		{
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			base.Controls.Clear();
			int startingTabIndex = mainGUI.StartingTabIndex;
			BPATitle bPATitle = new BPATitle(BPALoc.Label_NVTitle(mainGUI.Customizations.ShortName), borderCornerPoint, mainGUI.FullWidth, this);
			bPATitle.TabIndex = startingTabIndex++;
			borderCornerPoint = Navigate.Below(bPATitle);
			bool flag = ((CheckForUpdates.UpdateInfoExtendedData)updateInfo.ExtendedData).CheckCanceled || ((CheckForUpdates.UpdateInfoExtendedData)updateInfo.ExtendedData).CheckError || (!updateInfo.BinaryVersionInfo.NewerThanLocal && !updateInfo.ConfigVersionInfo.NewerThanLocal);
			bool flag2 = false;
			BPALabel bPALabel = null;
			if (mainGUI.ConfigExists || !flag)
			{
				bPALabel = (updateInfo.BinaryVersionInfo.NewerThanLocal ? new BPALabel(BPALoc.Label_NVDescNewVersion(updateInfo.BinaryVersionInfo.Description), borderCornerPoint, mainGUI.FullWidth, this) : (updateInfo.ConfigVersionInfo.NewerThanLocal ? new BPALabel(BPALoc.Label_NVDescNewConfig(updateInfo.ConfigVersionInfo.Description), borderCornerPoint, mainGUI.FullWidth, this) : (((CheckForUpdates.UpdateInfoExtendedData)updateInfo.ExtendedData).CheckCanceled ? new BPALabel(BPALoc.Label_NVDescCanceled, borderCornerPoint, mainGUI.FullWidth, this) : ((!((CheckForUpdates.UpdateInfoExtendedData)updateInfo.ExtendedData).CheckError) ? new BPALabel(BPALoc.Label_NVDescNoneFound, borderCornerPoint, mainGUI.FullWidth, this) : new BPALabel(BPALoc.Label_NVDescError, borderCornerPoint, mainGUI.FullWidth, this)))));
			}
			else
			{
				bPALabel = new BPALabel(BPALoc.Label_NVNoConfig, borderCornerPoint, mainGUI.FullWidth, this);
				flag2 = true;
			}
			bPALabel.TabIndex = startingTabIndex++;
			borderCornerPoint = Navigate.Below(bPALabel);
			borderCornerPoint = Navigate.Indent(borderCornerPoint);
			if (updateInfo.BinaryVersionInfo.NewerThanLocal)
			{
				BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.Exit, false, BPALoc.LinkLabel_NVExit(updateInfo.BinaryVersionInfo.ManualUrl), mainGUI.ArrowPic, borderCornerPoint, 0, this);
				bPALink.SetTabIndex(startingTabIndex++);
				borderCornerPoint = Navigate.Below(bPALink, 0.5f);
				BPALink bPALink2 = new BPALink(mainGUI, MainGUI.Actions.DownloadNewMSI, false, BPALoc.LinkLabel_NVUpdateVersion, mainGUI.ArrowPic, borderCornerPoint, 0, this);
				bPALink2.Tag = updateInfo;
				bPALink2.SetTabIndex(startingTabIndex++);
				borderCornerPoint = Navigate.Below(bPALink2, 0.5f);
			}
			else if (updateInfo.ConfigVersionInfo.NewerThanLocal)
			{
				if (updateInfo.ConfigVersionInfo.ManualUrl.Length > 0)
				{
					BPALink bPALink3 = new BPALink(mainGUI, MainGUI.Actions.Exit, false, BPALoc.LinkLabel_NVExit(updateInfo.ConfigVersionInfo.ManualUrl), mainGUI.ArrowPic, borderCornerPoint, 0, this);
					bPALink3.SetTabIndex(startingTabIndex++);
					borderCornerPoint = Navigate.Below(bPALink3, 0.5f);
				}
				BPALink bPALink4 = new BPALink(mainGUI, MainGUI.Actions.DownloadNewConfig, false, downloadConfigLabel, mainGUI.ArrowPic, borderCornerPoint, 0, this);
				bPALink4.Tag = updateInfo;
				bPALink4.SetTabIndex(startingTabIndex++);
				borderCornerPoint = Navigate.Below(bPALink4, 0.5f);
			}
			if (flag2)
			{
				BPALink bPALink5 = new BPALink(mainGUI, MainGUI.Actions.Exit, false, BPALoc.LinkLabel_SHExit, mainGUI.ArrowPic, borderCornerPoint, 0, this);
				bPALink5.SetTabIndex(startingTabIndex++);
				borderCornerPoint = Navigate.Below(bPALink5);
				borderCornerPoint.X -= 15;
				return;
			}
			string text = BPALoc.LinkLabel_NVContinue;
			if (flag)
			{
				text = BPALoc.LinkLabel_NVWelcome;
			}
			BPALink bPALink6 = new BPALink(mainGUI, MainGUI.Actions.ShowWelcome, false, text, mainGUI.ArrowPic, borderCornerPoint, 0, this);
			bPALink6.SetTabIndex(startingTabIndex++);
			bPALink6.Enabled = mainGUI.ConfigExists;
			borderCornerPoint = Navigate.Below(bPALink6);
			borderCornerPoint.X -= 15;
		}

		public bool Start(UpdateInfo updateInfo)
		{
			if (this.updateInfo != updateInfo)
			{
				this.updateInfo = updateInfo;
				AddControls();
			}
			return true;
		}
	}
}
