using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAGroupBox : GroupBox
	{
		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		private int resizeFlags;

		public int ResizeFlags
		{
			get
			{
				return resizeFlags;
			}
			set
			{
				resizeFlags = value;
			}
		}

		public Rectangle OrigRect
		{
			get
			{
				return origRect;
			}
		}

		public BPAGroupBox(string text, Point location, Size size, Control parent)
		{
			Font = MainGUI.DefaultFont;
			base.FlatStyle = FlatStyle.System;
			base.Location = location;
			base.Size = size;
			Text = text;
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		public void SetOrigRect()
		{
			origRect.Y = base.Top;
			origRect.X = base.Left;
			origRect.Width = base.Width;
			origRect.Height = base.Height;
		}
	}
}
