using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAShowHide : BPALink, IBPAControlAccess
	{
		private string showText = "";

		private string hideText = "";

		private Panel panel;

		private int maxHeight;

		public Control EffectivePositioningControl
		{
			get
			{
				Control result = null;
				if (panel.Controls.Count > 0)
				{
					result = panel.Controls[panel.Controls.Count - 1];
				}
				return result;
			}
		}

		public Control EffectiveContainmentControl
		{
			get
			{
				return panel;
			}
		}

		public BPAShowHide(MainGUI mainGUI, string showText, string hideText, Panel panel, Point location, int width, Control parent)
			: this(mainGUI, showText, hideText, panel, location, width, 0, parent)
		{
		}

		public BPAShowHide(MainGUI mainGUI, string showText, string hideText, Panel panel, Point location, int width, int maxHeight, Control parent)
			: this(showText, mainGUI.ShowPic, hideText, mainGUI.HidePic, panel, location, width, maxHeight, parent, mainGUI.TraceError)
		{
		}

		public BPAShowHide(string showText, Image showPic, string hideText, Image hidePic, Panel panel, Point location, int width, int maxHeight, Control parent, ExceptionCallback exceptionCallback)
			: base(showText, location, width, parent, showPic, hidePic, false, false, exceptionCallback, null, -1)
		{
			Font = MainGUI.DefaultFont;
			this.panel = panel;
			this.showText = showText;
			this.hideText = hideText;
			this.maxHeight = maxHeight;
			mactionCallback = ShowHide;
			panel.Left = 0;
			panel.Visible = false;
			panel.TabStop = false;
			base.Controls.Add(panel);
		}

		public void Show(bool show)
		{
			if (!show && panel.Visible)
			{
				image.Image = enabledImage;
				link.Text = showText;
				panel.Visible = false;
				base.Height -= panel.Height;
				foreach (Control control3 in base.Parent.Controls)
				{
					if (control3.Top > base.Top)
					{
						control3.Top -= panel.Height;
					}
				}
			}
			else
			{
				if (!show || panel.Visible)
				{
					return;
				}
				image.Image = disabledImage;
				link.Text = hideText;
				panel.Visible = true;
				base.Size = GetSizeToFit();
				foreach (Control control4 in base.Parent.Controls)
				{
					if (control4.Top > base.Top)
					{
						control4.Top += panel.Height;
					}
				}
			}
		}

		public override Size GetSizeToFit()
		{
			return GetSizeToFitSize(base.Size);
		}

		public override Size GetSizeToFitSize(Size size)
		{
			Size sizeToFitSize = base.GetSizeToFitSize(size);
			if (panel != null && panel.Visible)
			{
				panel.Top = sizeToFitSize.Height;
				panel.Width = sizeToFitSize.Width;
				MainGUI.ResizePanel(panel);
				if (maxHeight > 0 && panel.Height > maxHeight)
				{
					panel.Height = maxHeight;
				}
				sizeToFitSize.Height += panel.Height;
			}
			return sizeToFitSize;
		}

		public override void SetTabIndex(int nextTabIndex)
		{
			base.SetTabIndex(nextTabIndex);
			panel.TabIndex = nextTabIndex;
		}

		private void ShowHide(int action, BPAScreen customScreen, object actionInfo, string message)
		{
			try
			{
				if (panel.Visible)
				{
					Show(false);
				}
				else
				{
					Show(true);
				}
			}
			catch (Exception exception)
			{
				exceptionCallback(exception);
			}
		}
	}
}
