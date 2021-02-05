using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPALabel : Label, IBPADataControl
	{
		protected Rectangle morigRect = new Rectangle(0, 0, 0, 0);

		protected Color savedForeColor = Color.Black;

		public Rectangle OrigRect
		{
			get
			{
				return morigRect;
			}
		}

		public string DataValue
		{
			get
			{
				return Text;
			}
			set
			{
				Text = value;
			}
		}

		public event EventHandler DataChanged
		{
			add
			{
				base.TextChanged += value;
			}
			remove
			{
				base.TextChanged -= value;
			}
		}

		public BPALabel(string text, Point location, int width, Control parent)
			: this(text, location, width, Color.Empty, Control.DefaultForeColor, MainGUI.DefaultFont, parent)
		{
		}

		public BPALabel(string text, Point location, int width, Color backColor, Color foreColor, Font font, Control parent)
		{
			Font = font;
			ForeColor = foreColor;
			savedForeColor = foreColor;
			BackColor = backColor;
			base.FlatStyle = FlatStyle.System;
			base.Location = location;
			Text = text;
			base.Width = width;
			base.Size = GetSizeToFit();
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		public Size GetSizeToFit()
		{
			return GetSizeToFitSize(base.Size);
		}

		public Size GetSizeToFitSize(Size size)
		{
			if (size.Width == 0)
			{
				size.Width = GetWidth().Width;
				if (PreferredWidth > size.Width)
				{
					size.Width = PreferredWidth;
				}
				size.Width += 8;
				size.Height = PreferredHeight;
			}
			else
			{
				Graphics graphics = CreateGraphics();
				graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				graphics.PageUnit = GraphicsUnit.Pixel;
				StringFormat format = new StringFormat(StringFormat.GenericTypographic.FormatFlags | StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip);
				size.Height = Size.Ceiling(graphics.MeasureString(Text, Font, size.Width, format)).Height;
				if (size.Height > Font.Height)
				{
					size.Height += 3;
				}
				graphics.Dispose();
			}
			return size;
		}

		public Size GetWidth()
		{
			Graphics graphics = CreateGraphics();
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			graphics.PageUnit = GraphicsUnit.Pixel;
			Size result = Size.Ceiling(graphics.MeasureString(Text, Font));
			graphics.Dispose();
			return result;
		}

		public void SetOrigRect()
		{
			morigRect.Y = base.Top;
			morigRect.X = base.Left;
			morigRect.Width = base.Width;
			morigRect.Height = base.Height;
		}

		public object[] Setting(Node node)
		{
			return null;
		}

		public void Highlight(bool highlight)
		{
			ForeColor = (highlight ? Color.Red : savedForeColor);
		}
	}
}
