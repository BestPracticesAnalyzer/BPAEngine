using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPATabControl : TabControl
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

		public BPATabControl(Point location, Size size, Control parent)
		{
			Font = MainGUI.DefaultFont;
			base.Location = location;
			base.Size = size;
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
