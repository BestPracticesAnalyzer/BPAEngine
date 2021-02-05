using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPATextBoxButton : TextBox, IBPADataControl
	{
		private delegate void UpdateCallback(string value);

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

		public BPATextBoxButton(string name, string buttonText, ButtonCallbackBase callObject, Point location, float scale, Control parent)
		{
			base.Location = new Point(location.X, location.Y);
			base.Size = new Size((int)(150f * scale), Font.Height + 4);
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
			BPAButton bPAButton = new BPAButton(buttonText, callObject, Update, new Point(location.X + base.Size.Width + 4, location.Y), parent)
			{
				Name = name + "__Button"
			};
		}

		public void Update(string value)
		{
			if (base.InvokeRequired)
			{
				Invoke(new UpdateCallback(Update), value);
			}
			else
			{
				Text = value;
			}
		}

		public object[] Setting(Node node)
		{
			return new object[1]
			{
				Text
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
