using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class Welcome : BPAScreen
	{
		private BPALink linkStart;

		private BPALink linkSelect;

		public Welcome(MainGUI mainGUI)
			: base(mainGUI)
		{
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			base.mainGUI = mainGUI;
			int startingTabIndex = mainGUI.StartingTabIndex;
			borderCornerPoint = Navigate.Below(new BPATitle(BPALoc.Label_WTitle(mainGUI.Customizations.ShortName), borderCornerPoint, mainGUI.FullWidth, Color.White, Color.FromArgb(49, 85, 156), MainGUI.TitleFont, this)
			{
				TabIndex = startingTabIndex++
			});
			borderCornerPoint = Navigate.Below(new BPALabel(mainGUI.Customizations.Description.Replace("\\n", "\n\n"), borderCornerPoint, mainGUI.FullWidth, this)
			{
				TabIndex = startingTabIndex++
			});
			borderCornerPoint = Navigate.Indent(borderCornerPoint);
			if (mainGUI.Customizations.CustomScreens.Count > 0)
			{
				linkStart = new BPALink(mainGUI, MainGUI.Actions.ShowCustomScreen, false, mainGUI.Customizations.StartScanLink, mainGUI.ArrowPic, borderCornerPoint, 0, this);
				linkStart.CustomScreen = (BPAScreen)mainGUI.Customizations.CustomScreens[0];
			}
			else
			{
				linkStart = new BPALink(mainGUI, MainGUI.Actions.ShowSelectAnalyzer, false, "Select an Analyzer to Load!", mainGUI.ArrowPic, borderCornerPoint, 0, this);
			}
			linkStart.SetTabIndex(startingTabIndex++);
			borderCornerPoint = Navigate.Below(linkStart, 0.5f);
			linkSelect = new BPALink(mainGUI, MainGUI.Actions.SelectScan, false, mainGUI.Customizations.SelectScanLink, mainGUI.ArrowPic, borderCornerPoint, 0, this);
			linkSelect.SetTabIndex(startingTabIndex++);
			borderCornerPoint = Navigate.Below(linkSelect, 3f);
		}

		public override bool Start()
		{
			return true;
		}
	}
}
