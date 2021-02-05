using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAPictureBox : PictureBox
	{
		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		public Rectangle OrigRect
		{
			get
			{
				return origRect;
			}
		}

		public BPAPictureBox(Image image, Point location, Control parent)
		{
			base.Image = image;
			base.Location = location;
			base.Size = image.Size;
			base.TabStop = false;
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		public BPAPictureBox(Color backColor, Point location, Size size, Control parent)
		{
			BackColor = backColor;
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
