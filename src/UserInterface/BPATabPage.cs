using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPATabPage : TabPage
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

		public BPATabPage(string tabText, Point location, Size size, BPATabControl parentGroup)
		{
			Font = MainGUI.DefaultFont;
			base.Location = location;
			base.Size = size;
			Text = tabText;
			SetOrigRect();
			if (parentGroup != null)
			{
				parentGroup.TabPages.Add(this);
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
