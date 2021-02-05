using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.Updater
{
	public class PrettyFooter : FlowLayoutPanel
	{
		private Color startColor;

		private Color endColor;

		public Color GradientStartColor
		{
			get
			{
				return startColor;
			}
			set
			{
				startColor = value;
				Refresh();
			}
		}

		public Color GradientEndColor
		{
			get
			{
				return endColor;
			}
			set
			{
				endColor = value;
				Refresh();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			Rectangle rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
			Brush brush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.ForwardDiagonal);
			e.Graphics.FillRectangle(brush, rect);
		}
	}
}
