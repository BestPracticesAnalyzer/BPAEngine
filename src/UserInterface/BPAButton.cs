using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAButton : Button, IBPADataControl
	{
		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		private Control parent;

		private ShowDialogCallback dialogCallback;

		private ButtonCallbackBase callObject;

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

		public BPAButton(string buttonText, ButtonCallbackBase callObject, ShowDialogCallback dialogCallback, Point location, Control parent)
		{
			base.Location = new Point(location.X, location.Y);
			Text = buttonText;
			BackColor = Control.DefaultBackColor;
			base.Size = new Size(0, base.Size.Height);
			base.Size = GetSizeToFit();
			base.Click += button_Click;
			this.parent = parent;
			this.callObject = callObject;
			this.dialogCallback = dialogCallback;
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
			graphics.Dispose();
			return size;
		}

		public object[] Setting(Node node)
		{
			if (callObject == null)
			{
				return null;
			}
			return callObject.Setting(node);
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

		private void button_Click(object sender, EventArgs e)
		{
			if (callObject != null)
			{
				callObject.Callback(dialogCallback, FindForm().Handle);
			}
		}
	}
}
