using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPATreeView : TreeView
	{
		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		public Rectangle OrigRect
		{
			get
			{
				return origRect;
			}
		}

		public BPATreeView(Point location, Size size, Control parent)
		{
			Font = MainGUI.DefaultFont;
			base.Location = location;
			base.Size = size;
			base.ShowLines = false;
			base.HideSelection = false;
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
