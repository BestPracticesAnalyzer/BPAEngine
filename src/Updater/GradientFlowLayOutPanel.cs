using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.Updater
{
	public class GradientFlowLayOutPanel : FlowLayoutPanel
	{
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			pevent.Graphics.CompositingQuality = CompositingQuality.HighQuality;
			int width = base.ClientRectangle.Width;
			int height = base.ClientRectangle.Height;
			Rectangle rect = new Rectangle(0, 0, width - 1, height - 1);
			LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(90, 160, 255), Color.FromArgb(25, 65, 165), LinearGradientMode.ForwardDiagonal);
			pevent.Graphics.FillRectangle(brush, rect);
		}
	}
}
