using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class HeaderInfo
	{
		private string header = "";

		private bool expanded = true;

		private int depth;

		private BPAListBox listBox;

		private ArrayList issuesList;

		private Color[] headerColors = new Color[6]
		{
			Color.FromArgb(177, 180, 177),
			Color.FromArgb(197, 197, 197),
			Color.FromArgb(214, 214, 214),
			Color.FromArgb(233, 233, 233),
			Color.FromArgb(244, 244, 244),
			Color.FromArgb(255, 255, 255)
		};

		public string Header
		{
			get
			{
				return header;
			}
		}

		public bool Expanded
		{
			get
			{
				return expanded;
			}
			set
			{
				expanded = value;
			}
		}

		public int Depth
		{
			get
			{
				return depth;
			}
		}

		public HeaderInfo(BPAListBox listBox, string header, int depth)
		{
			this.listBox = listBox;
			this.header = header;
			this.depth = depth;
			issuesList = new ArrayList();
		}

		public override string ToString()
		{
			return header;
		}

		public void Select(int index)
		{
			int num = index + 1;
			if (expanded)
			{
				while (num < listBox.Items.Count && (!(listBox.Items[num] is HeaderInfo) || ((HeaderInfo)listBox.Items[num]).Depth > depth))
				{
					issuesList.Add(listBox.Items[num]);
					listBox.Items.RemoveAt(num);
				}
			}
			else
			{
				while (issuesList.Count > 0)
				{
					listBox.Items.Insert(num++, issuesList[0]);
					issuesList.RemoveAt(0);
				}
			}
			expanded = !expanded;
		}

		public void Draw(DrawItemEventArgs e)
		{
			int num = e.Bounds.Y;
			Rectangle rectangle;
			if (e.Index != 0)
			{
				rectangle = new Rectangle(e.Bounds.X, num, e.Bounds.Width, 2);
				e.Graphics.FillRectangle(new SolidBrush(Color.White), rectangle);
				num += 2;
			}
			rectangle = new Rectangle(e.Bounds.X, num, e.Bounds.Width, MainGUI.DefaultFont.Height * 2);
			Color color = ((depth > 5) ? headerColors[5] : headerColors[depth]);
			e.Graphics.FillRectangle(new SolidBrush(color), rectangle);
			rectangle.X += 20 * depth;
			if (e.Index < listBox.Items.Count - 1 || issuesList.Count > 0)
			{
				Pen pen = new Pen(Color.Black);
				int num2 = 4;
				e.Graphics.DrawRectangle(pen, rectangle.X, rectangle.Y + (rectangle.Height - (num2 + 4)) / 2, num2 + 4, num2 + 4);
				int num3 = rectangle.X + 2;
				int y = rectangle.Y + rectangle.Height / 2;
				e.Graphics.DrawLine(pen, new Point(num3, y), new Point(num3 + num2, y));
				if (!expanded)
				{
					num3 += num2 / 2;
					y = rectangle.Y + (rectangle.Height - num2) / 2;
					e.Graphics.DrawLine(pen, new Point(num3, y), new Point(num3, y + num2));
				}
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Trimming = StringTrimming.EllipsisCharacter;
			rectangle.X += 16;
			rectangle.Y += (rectangle.Height - MainGUI.DefaultFont.Height) / 2;
			Font font = new Font(MainGUI.DefaultFont, FontStyle.Bold);
			e.Graphics.DrawString(header, font, new SolidBrush(Color.Black), rectangle, stringFormat);
		}

		public int Measure(MeasureItemEventArgs e)
		{
			return MainGUI.DefaultFont.Height * 2 + ((e.Index != 0) ? 2 : 0);
		}
	}
}
