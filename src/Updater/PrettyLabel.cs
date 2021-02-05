using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.Updater
{
	public class PrettyLabel : Label
	{
		private Color startGradient;

		private Color endGradient;

		public Color StartGradientColor
		{
			get
			{
				return startGradient;
			}
			set
			{
				startGradient = value;
				Refresh();
			}
		}

		public Color EndGradientColor
		{
			get
			{
				return endGradient;
			}
			set
			{
				endGradient = value;
				Refresh();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			Rectangle rect = new Rectangle(0, 0, base.Width, base.Height);
			LinearGradientBrush brush = new LinearGradientBrush(rect, startGradient, endGradient, LinearGradientMode.ForwardDiagonal);
			pevent.Graphics.FillRectangle(brush, rect);
		}
	}
}
