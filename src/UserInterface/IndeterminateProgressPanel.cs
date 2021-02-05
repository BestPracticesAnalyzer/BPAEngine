using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class IndeterminateProgressPanel : BaseProgressPanel
	{
		private Panel progBarPanel;

		private int progBarProgress;

		public IndeterminateProgressPanel()
		{
		}

		public IndeterminateProgressPanel(int width)
			: base(width)
		{
		}

		public IndeterminateProgressPanel(MainGUI mainGUI)
			: base(mainGUI)
		{
		}

		protected override void AddProgressBar()
		{
			progBarPanel = new BPAPanel(nextLocation, 350, 16, this);
			progBarPanel.BorderStyle = BorderStyle.None;
			progBarPanel.TabIndex = nextTabIndex++;
			progBarPanel.Paint += DrawEternalProgressBar;
			nextLocation = Navigate.Below(progBarPanel, 2f);
			progBarPanel.Invalidate();
		}

		protected override void UpdateTimer()
		{
			progBarPanel.Invalidate();
		}

		private void DrawEternalProgressBar(object sender, PaintEventArgs e)
		{
			try
			{
				int num = progBarPanel.Height - 1;
				int num2 = progBarPanel.Width - 1;
				int num3 = num - 4;
				progBarProgress += num3;
				if (progBarProgress >= num2)
				{
					progBarProgress = num3 + 1;
				}
				e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, num2, num);
				e.Graphics.DrawRectangle(new Pen(SystemColors.ActiveBorder), 0, 0, num2, num);
				for (int i = 0; i < 6; i++)
				{
					int num4 = progBarProgress + i * (num3 + 1);
					if (num4 + num3 >= num2)
					{
						num4 = (5 - i) * (num3 + 1);
					}
					e.Graphics.FillRectangle(new SolidBrush(Color.LightGreen), num4, 2, num3, num3);
				}
			}
			catch
			{
			}
		}
	}
}
