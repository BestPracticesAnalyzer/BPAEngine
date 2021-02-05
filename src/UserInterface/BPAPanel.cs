using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAPanel : Panel
	{
		public const int CR_NoResizeHeight = 1;

		public const int CR_NoResizeWidth = 2;

		public const int CR_FillHeight = 4;

		public const int CR_NoRecurse = 8;

		public const int CR_NoBorder = 16;

		protected Rectangle morigRect = default(Rectangle);

		private int resizeFlags;

		public Rectangle OrigRect
		{
			get
			{
				return morigRect;
			}
		}

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

		public BPAPanel()
			: this(new Point(0, 0), 0, 0, null)
		{
		}

		public BPAPanel(Point location, int width, int height, Control parent)
		{
			base.BorderStyle = BorderStyle.Fixed3D;
			base.Location = location;
			base.Width = width;
			base.Height = height;
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		public void SetOrigRect()
		{
			morigRect.Y = base.Top;
			morigRect.X = base.Left;
			morigRect.Width = base.Width;
			morigRect.Height = base.Height;
		}
	}
}
