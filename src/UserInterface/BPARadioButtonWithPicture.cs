using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPARadioButtonWithPicture : BPAPanel, IBPADataControl
	{
		private BPARadioButton radioButton;

		private BPAPictureBox image;

		private BPALabel text;

		public bool Checked
		{
			get
			{
				return radioButton.Checked;
			}
			set
			{
				radioButton.Checked = value;
			}
		}

		public string DataValue
		{
			get
			{
				if (!radioButton.Checked)
				{
					return "0";
				}
				return "1";
			}
			set
			{
				radioButton.Checked = value == "1";
			}
		}

		public event EventHandler DataChanged
		{
			add
			{
				radioButton.CheckedChanged += value;
			}
			remove
			{
				radioButton.CheckedChanged -= value;
			}
		}

		public BPARadioButtonWithPicture(string text, Image image, Point location, int width, Control parent)
			: base(location, width, image.Height, parent)
		{
			radioButton = new BPARadioButton("", new Point(0, 0), 0, this);
			this.image = new BPAPictureBox(image, new Point(radioButton.Width, 0), this);
			this.text = new BPALabel(text, new Point(this.image.Right + 5, 0), width, this);
			int height = radioButton.Height;
			if (this.image.Height > height)
			{
				height = this.image.Height;
			}
			if (this.text.Height > height)
			{
				height = this.text.Height;
			}
			radioButton.Top = (height - radioButton.Height) / 2;
			this.image.Top = (height - this.image.Height) / 2;
			this.text.Top = (height - this.text.Height) / 2;
			base.BorderStyle = BorderStyle.None;
			base.Width = radioButton.Width + this.image.Width + this.text.Width + 5;
			base.Height = height;
			SetOrigRect();
			base.ResizeFlags = 11;
		}

		public object[] Setting(Node node)
		{
			return new object[1]
			{
				radioButton.Checked ? "1" : "0"
			};
		}

		public void Highlight(bool highlight)
		{
			ForeColor = (highlight ? Color.Red : Color.Black);
		}

		public void AddClickEvent(EventHandler clickEvent)
		{
			text.Click += clickEvent;
			image.Click += clickEvent;
			radioButton.Click += clickEvent;
			base.Click += clickEvent;
		}

		public bool ContainsControl(object control)
		{
			if (control != this && control != radioButton && control != image)
			{
				return control == text;
			}
			return true;
		}
	}
}
