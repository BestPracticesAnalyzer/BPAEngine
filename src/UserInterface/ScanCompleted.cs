using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class ScanCompleted : BPAScreen
	{
		public class CompletionInfo
		{
			public bool completedOk;

			public DataInfo dataInfo;
		}

		public ScanCompleted(MainGUI mainGUI)
			: base(mainGUI)
		{
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			base.mainGUI = mainGUI;
		}

		public bool Start(CompletionInfo completionInfo)
		{
			BPAScanInfo bPAScanInfo = new BPAScanInfo(mainGUI.ExecInterface.Options);
			if (bPAScanInfo.ObjectsToBeProcessed.Count == 0)
			{
				mainGUI.TakeAction(MainGUI.Actions.LoadScan, completionInfo.dataInfo);
				return false;
			}
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			base.Controls.Clear();
			int startingTabIndex = mainGUI.StartingTabIndex;
			string text = "";
			string text2 = "";
			if (completionInfo.completedOk)
			{
				text = BPALoc.Label_SCTitleCompleted;
				text2 = BPALoc.Label_SCStatusSuccess;
			}
			else
			{
				text = BPALoc.Label_SCTitleAborted;
				text2 = BPALoc.Label_SCStatusAborted;
			}
			BPATitle bPATitle = new BPATitle(text, borderCornerPoint, mainGUI.FullWidth, this);
			bPATitle.TabIndex = startingTabIndex++;
			borderCornerPoint = Navigate.Below(bPATitle);
			BPALabel bPALabel = new BPALabel(text2, borderCornerPoint, mainGUI.FullWidth, this);
			bPALabel.TabIndex = startingTabIndex++;
			borderCornerPoint = Navigate.Below(bPALabel);
			borderCornerPoint = Navigate.Indent(borderCornerPoint);
			BPALink bPALink = new BPALink(mainGUI, MainGUI.Actions.LoadScan, false, BPALoc.LinkLabel_SCViewScan, mainGUI.ArrowPic, borderCornerPoint, 0, this);
			bPALink.Tag = completionInfo.dataInfo;
			bPALink.Enabled = completionInfo.dataInfo != null;
			bPALink.SetTabIndex(startingTabIndex++);
			borderCornerPoint = Navigate.Below(bPALink);
			borderCornerPoint = Navigate.UnIndent(borderCornerPoint);
			BPALabel bPALabel2 = new BPALabel(BPALoc.Label_SCDetails, borderCornerPoint, 0, this);
			bPALabel2.TabIndex = startingTabIndex++;
			borderCornerPoint = Navigate.Below(bPALabel2);
			BPAPanel statusPanel = mainGUI.StartScanScreen.StatusPanel;
			if (statusPanel.Parent != null)
			{
				statusPanel.Parent.Controls.Remove(statusPanel);
			}
			statusPanel.Location = borderCornerPoint;
			statusPanel.Height = mainGUI.FullHeight - (statusPanel.Top - MainGUI.BorderCornerPoint.Y);
			statusPanel.SetOrigRect();
			statusPanel.ResizeFlags = 4;
			base.Controls.Add(statusPanel);
			statusPanel.Invalidate(true);
			return true;
		}
	}
}
