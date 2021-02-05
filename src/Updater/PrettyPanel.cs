using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.Updater
{
	public class PrettyPanel : Panel
	{
		private string headerText;

		private string footerText;

		private PrettyLabel headerLabel;

		private PrettyFooter footerLabel;

		private Color startBackColor = Color.White;

		private Color endBackColor = Color.FromArgb(160, 191, 245);

		public string HeaderText
		{
			get
			{
				return headerText;
			}
			set
			{
				headerText = value;
				headerLabel.Text = headerText;
				headerLabel.Refresh();
			}
		}

		public string FooterText
		{
			get
			{
				return footerText;
			}
			set
			{
				footerText = value;
				footerLabel.Text = footerText;
				footerLabel.Refresh();
			}
		}

		public Color StartGradientColor
		{
			get
			{
				return startBackColor;
			}
			set
			{
				startBackColor = value;
				Refresh();
			}
		}

		public Color EndGradientColor
		{
			get
			{
				return endBackColor;
			}
			set
			{
				endBackColor = value;
				Refresh();
			}
		}

		public Color HeaderGradientStartColor
		{
			get
			{
				return headerLabel.StartGradientColor;
			}
			set
			{
				headerLabel.StartGradientColor = value;
				headerLabel.Refresh();
			}
		}

		public Color HeaderGradientEndColor
		{
			get
			{
				return headerLabel.EndGradientColor;
			}
			set
			{
				headerLabel.EndGradientColor = value;
				headerLabel.Refresh();
			}
		}

		public Color FooterGradientStartColor
		{
			get
			{
				return footerLabel.GradientStartColor;
			}
			set
			{
				footerLabel.GradientStartColor = value;
				footerLabel.Refresh();
			}
		}

		public Color FooterGradientEndColor
		{
			get
			{
				return footerLabel.GradientEndColor;
			}
			set
			{
				footerLabel.GradientEndColor = value;
				footerLabel.Refresh();
			}
		}

		public PrettyPanel()
		{
			headerLabel = new PrettyLabel();
			headerLabel.Height = 25;
			headerLabel.Parent = this;
			headerLabel.Dock = DockStyle.Top;
			headerLabel.ForeColor = Color.Black;
			headerLabel.BorderStyle = BorderStyle.FixedSingle;
			headerLabel.FlatStyle = FlatStyle.Standard;
			headerLabel.ForeColor = Color.White;
			headerLabel.StartGradientColor = Color.FromArgb(90, 160, 255);
			headerLabel.EndGradientColor = Color.FromArgb(25, 65, 165);
			footerLabel = new PrettyFooter();
			footerLabel.Height = 25;
			footerLabel.Parent = this;
			footerLabel.Dock = DockStyle.Bottom;
			footerLabel.ForeColor = Color.Black;
			footerLabel.BorderStyle = BorderStyle.FixedSingle;
			footerLabel.GradientStartColor = Color.FromArgb(90, 160, 255);
			footerLabel.GradientEndColor = Color.FromArgb(25, 65, 165);
			DoubleBuffered = true;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
			Brush brush = new LinearGradientBrush(rect, startBackColor, endBackColor, LinearGradientMode.Horizontal);
			e.Graphics.FillRectangle(brush, rect);
			base.OnPaint(e);
		}
	}
}
