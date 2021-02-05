using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPASimpleListBox : ListBox, IBPADataControl
	{
		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		private int lines;

		public Rectangle OrigRect
		{
			get
			{
				return origRect;
			}
		}

		public int Lines
		{
			get
			{
				return lines;
			}
			set
			{
				lines = value;
				base.Height = (Font.Height + 3) * lines + 2;
				SetOrigRect();
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

		public BPASimpleListBox(Point location, float scale, int lines, Control parent)
		{
			Font = MainGUI.DefaultFont;
			base.BorderStyle = BorderStyle.Fixed3D;
			base.Location = new Point(location.X, location.Y);
			base.Width = (int)(150f * scale);
			Lines = lines;
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		public object[] Setting(Node node)
		{
			string text = ((SelectedIndex >= 0) ? base.Items[SelectedIndex].ToString() : string.Empty);
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
					text
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
