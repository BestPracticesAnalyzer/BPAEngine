using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPARadioButton : RadioButton, IBPADataControl
	{
		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		public Rectangle OrigRect
		{
			get
			{
				return origRect;
			}
		}

		public string DataValue
		{
			get
			{
				if (!base.Checked)
				{
					return "0";
				}
				return "1";
			}
			set
			{
				base.Checked = value == "1";
			}
		}

		public event EventHandler DataChanged
		{
			add
			{
				base.CheckedChanged += value;
			}
			remove
			{
				base.CheckedChanged -= value;
			}
		}

		public BPARadioButton(string text, Point location, int width, Control parent)
		{
			base.FlatStyle = FlatStyle.System;
			base.Location = location;
			TextAlign = ContentAlignment.MiddleLeft;
			base.Width = width;
			Text = text;
			Font = MainGUI.DefaultFont;
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
			Graphics graphics = CreateGraphics();
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			graphics.PageUnit = GraphicsUnit.Pixel;
			if (size.Width == 0)
			{
				size = Size.Ceiling(graphics.MeasureString(Text, Font));
				size.Width += SystemInformation.SmallIconSize.Width + 4;
			}
			else
			{
				StringFormat format = new StringFormat(StringFormat.GenericTypographic.FormatFlags | StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip);
				size.Height = Size.Ceiling(graphics.MeasureString(Text, Font, size.Width - SystemInformation.SmallIconSize.Width - 4, format)).Height;
			}
			if (size.Height > Font.Height)
			{
				size.Height += 3;
			}
			else if (size.Height == 0)
			{
				size.Height = SystemInformation.SmallIconSize.Height;
			}
			graphics.Dispose();
			return size;
		}

		public object[] Setting(Node node)
		{
			return new object[1]
			{
				base.Checked ? "1" : "0"
			};
		}

		public void Highlight(bool highlight)
		{
			ForeColor = (highlight ? Color.Red : Color.Black);
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
