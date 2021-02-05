using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAComboBox : ComboBox, IBPADataControl
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
				if (SelectedIndex == -1)
				{
					return "";
				}
				return base.Items[SelectedIndex].ToString();
			}
			set
			{
				ArrayList arrayList = CommonData.DecodeStringArray(value);
				base.Items.Clear();
				foreach (string item in arrayList)
				{
					base.Items.Add(item);
				}
			}
		}

		public event EventHandler DataChanged
		{
			add
			{
				base.SelectedIndexChanged += value;
			}
			remove
			{
				base.SelectedIndexChanged -= value;
			}
		}

		public BPAComboBox(Point location, float scale, Control parent)
		{
			Font = MainGUI.DefaultFont;
			base.DropDownStyle = ComboBoxStyle.DropDownList;
			base.Location = new Point(location.X, location.Y - 2);
			base.Size = new Size((int)(200f * scale), Font.Height + 4);
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		public object[] Setting(Node node)
		{
			switch (node.GetAttribute("Key1"))
			{
			case "SelectedIndex":
				return new object[1]
				{
					SelectedIndex
				};
			default:
				return new object[1]
				{
					base.Items[SelectedIndex].ToString()
				};
			}
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
