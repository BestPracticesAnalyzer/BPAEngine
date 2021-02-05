using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class UpdateOption : BPAScreen
	{
		private BPARadioButton enableUpdates;

		private BPARadioButton disableUpdates;

		private BPARadioButton enableSQM;

		private BPARadioButton disableSQM;

		private BPALink linkWelcome;

		private BPALink linkCheckVersion;

		public UpdateOption(MainGUI mainGUI)
			: base(mainGUI)
		{
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			int startingTabIndex = mainGUI.StartingTabIndex;
			borderCornerPoint = Navigate.Below(new BPATitle(BPALoc.Label_UOTitle, borderCornerPoint, mainGUI.FullWidth, this)
			{
				TabIndex = startingTabIndex++
			});
			borderCornerPoint = Navigate.Below(new BPALabel(BPALoc.Label_UODescription(BPALoc.Label_UOTitle), borderCornerPoint, mainGUI.FullWidth, this)
			{
				TabIndex = startingTabIndex++
			});
			enableUpdates = new BPARadioButton(BPALoc.RadioButton_UOEnableUpdates, borderCornerPoint, 0, this);
			enableUpdates.TabIndex = startingTabIndex++;
			enableUpdates.Checked = mainGUI.RegSettings.VersionCheckAlways;
			enableUpdates.Click += UpdatesClicked;
			borderCornerPoint = Navigate.Below(enableUpdates);
			disableUpdates = new BPARadioButton(BPALoc.RadioButton_UODisableUpdates, borderCornerPoint, 0, this);
			disableUpdates.TabIndex = startingTabIndex++;
			disableUpdates.Checked = !mainGUI.RegSettings.VersionCheckAlways;
			disableUpdates.Click += UpdatesClicked;
			borderCornerPoint = Navigate.Below(disableUpdates);
			BPAGradiatedBox control = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, borderCornerPoint, new Size(mainGUI.FullWidth, 1), this);
			borderCornerPoint = Navigate.Below(control);
			Node node = mainGUI.ExecInterface.Options.Configuration.ConfigurationNode.GetNode("DataPostprocessor[@Class='Microsoft.WindowsServerSystem.BestPracticesAnalyzer.Common.SQMDataPostprocessor']");
			if (node != null)
			{
				borderCornerPoint = Navigate.Below(new BPALabel(BPALoc.Label_UOSQMDescription, borderCornerPoint, mainGUI.FullWidth, this)
				{
					TabIndex = startingTabIndex++
				});
				borderCornerPoint = Navigate.Indent(borderCornerPoint);
				BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.None, false, BPALoc.LinkLabel_UOSQMTellMeMore, mainGUI.ArrowPic, borderCornerPoint, 0, this);
				bPALink.SetTabIndex(startingTabIndex++);
				bPALink.AddClickEvent(SQMTellMeMore);
				borderCornerPoint = Navigate.UnIndent(Navigate.Below(bPALink));
				BPAPanel bPAPanel = new BPAPanel(borderCornerPoint, mainGUI.FullWidth, 0, this)
				{
					BorderStyle = BorderStyle.None
				};
				enableSQM = new BPARadioButton(location: new Point(0, 0), text: BPALoc.RadioButton_UOEnableSQM, width: 0, parent: bPAPanel);
				enableSQM.TabIndex = startingTabIndex++;
				enableSQM.Checked = mainGUI.RegSettings.SQMEnabledSet && mainGUI.RegSettings.SQMEnabled;
				enableSQM.Click += SQMClicked;
				borderCornerPoint = Navigate.Below(enableSQM);
				disableSQM = new BPARadioButton(BPALoc.RadioButton_UODisableSQM, borderCornerPoint, 0, bPAPanel);
				disableSQM.TabIndex = startingTabIndex++;
				disableSQM.Checked = mainGUI.RegSettings.SQMEnabledSet && !mainGUI.RegSettings.SQMEnabled;
				disableSQM.Click += SQMClicked;
				bPAPanel.Height = Navigate.Below(disableSQM).Y;
				bPAPanel.SetOrigRect();
				borderCornerPoint = Navigate.Below(bPAPanel);
				BPAGradiatedBox control2 = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, borderCornerPoint, new Size(mainGUI.FullWidth, 1), this);
				borderCornerPoint = Navigate.Below(control2);
			}
			borderCornerPoint = Navigate.Indent(borderCornerPoint);
			linkCheckVersion = new BPALink(mainGUI, MainGUI.Actions.CheckVersion, false, BPALoc.Label_UOCheckVersion, mainGUI.ArrowPic, borderCornerPoint, 0, this);
			linkCheckVersion.SetTabIndex(startingTabIndex++);
			linkCheckVersion.Enabled = mainGUI.ConfigExists;
			borderCornerPoint = Navigate.Below(linkCheckVersion);
			linkWelcome = new BPALink(mainGUI, MainGUI.Actions.ShowWelcome, false, BPALoc.LinkLabel_NVWelcome, mainGUI.ArrowPic, borderCornerPoint, 0, this);
			linkWelcome.SetTabIndex(startingTabIndex++);
			linkWelcome.Enabled = mainGUI.ConfigExists;
			borderCornerPoint = Navigate.Below(linkWelcome);
		}

		public override bool Start()
		{
			if (!mainGUI.RegSettings.SQMEnabledSet)
			{
				mainGUI.EnableLinks(false);
				linkWelcome.Enabled = false;
				linkCheckVersion.Enabled = false;
			}
			return true;
		}

		private void UpdatesClicked(object sender, EventArgs e)
		{
			try
			{
				enableUpdates.Checked = sender == enableUpdates;
				disableUpdates.Checked = !enableUpdates.Checked;
				mainGUI.RegSettings.VersionCheckAlways = enableUpdates.Checked;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void SQMClicked(object sender, EventArgs e)
		{
			try
			{
				mainGUI.RegSettings.SQMEnabled = enableSQM.Checked;
				mainGUI.EnableLinks(true);
				linkWelcome.Enabled = true;
				linkCheckVersion.Enabled = true;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		private void SQMTellMeMore(object sender, EventArgs e)
		{
			CommonData.BrowseURL("http://go.microsoft.com/fwlink/?LinkId=64471");
		}
	}
}
