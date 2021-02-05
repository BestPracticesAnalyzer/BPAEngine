using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class Navigate
	{
		public static Point Below(Control control)
		{
			return Below(control, 1f);
		}

		public static Point Below(Control control, float scale)
		{
			return new Point(control.Left, control.Bottom + (int)(10f * scale));
		}

		public static void CenterWith(Control fixedControl, Control controlToAdjust)
		{
			controlToAdjust.Top += (fixedControl.Height - controlToAdjust.Height) / 2;
		}

		public static Point Indent(Point currentLocation)
		{
			return new Point(currentLocation.X + 15, currentLocation.Y);
		}

		public static Point NextTo(Control control)
		{
			return NextTo(control, 1f);
		}

		public static Point NextTo(Control control, float scale)
		{
			return new Point((int)((float)control.Right + 10f * scale), control.Top);
		}

		public static void RightJustify(Control control)
		{
			control.Left = control.Parent.Width - control.Width - 10;
		}

		public static Point UnIndent(Point currentLocation)
		{
			return new Point(currentLocation.X - 15, currentLocation.Y);
		}

		public static Point LeftBelow(Control control)
		{
			return LeftBelow(control, 1f);
		}

		public static Point LeftBelow(Control control, float scale)
		{
			return new Point(MainGUI.BorderCornerPoint.X, control.Bottom + (int)(10f * scale));
		}
	}
}
