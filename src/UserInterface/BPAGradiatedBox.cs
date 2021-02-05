using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAGradiatedBox : PictureBox
	{
		private Color colorLeft = Color.White;

		private Color colorRight = Color.White;

		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		public Rectangle OrigRect
		{
			get
			{
				return origRect;
			}
		}

		public BPAGradiatedBox(Color colorLeft, Color colorRight, Point location, Size size, Control parent)
		{
			base.Location = location;
			base.Size = size;
			this.colorLeft = colorLeft;
			this.colorRight = colorRight;
			base.TabStop = false;
			base.Paint += PaintGradiatedBox;
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

		private void PaintGradiatedBox(object sender, PaintEventArgs e)
		{
			try
			{
				if (base.Parent != null)
				{
					Control control = (Control)sender;
					e.Graphics.Clear(base.Parent.BackColor);
					Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);
					e.Graphics.FillRectangle(new LinearGradientBrush(rect, colorLeft, colorRight, 0f, false), rect);
				}
			}
			catch
			{
			}
		}
	}
}
