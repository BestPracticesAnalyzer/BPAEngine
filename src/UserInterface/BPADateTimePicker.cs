using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPADateTimePicker : DateTimePicker, IBPADataControl
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
				return base.Value.ToString();
			}
			set
			{
				try
				{
					base.Value = DateTime.Parse(value);
				}
				catch (FormatException)
				{
				}
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

		public BPADateTimePicker(Point location, int width, Control parent)
		{
			Font = MainGUI.DefaultFont;
			base.Location = location;
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
			base.Size = size;
			return size;
		}

		public void SetOrigRect()
		{
			origRect.Y = base.Top;
			origRect.X = base.Left;
			origRect.Width = base.Width;
			origRect.Height = base.Height;
		}

		public object[] Setting(Node node)
		{
			return new object[1]
			{
				base.Value
			};
		}

		public void Highlight(bool highlight)
		{
			ForeColor = (highlight ? Color.Red : Color.Black);
		}
	}
}
