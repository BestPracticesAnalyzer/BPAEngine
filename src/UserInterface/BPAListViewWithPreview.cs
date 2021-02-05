using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAListViewWithPreview : BPAListView
	{
		private BPARichTextBox richTextBox;

		public BPAListViewWithPreview(Point location, Size size, float percent, bool sort, Control parent)
			: base(location, new Size((int)((float)size.Width * percent), size.Height), sort, parent)
		{
			if ((double)percent > 1.0 || (double)percent < 0.0)
			{
				throw new ArgumentException();
			}
			richTextBox = new BPARichTextBox(new Point(location.X + (int)((float)size.Width * percent), location.Y), new Size((int)((double)size.Width * (1.0 - (double)percent)), size.Height), true, 0, parent);
			richTextBox.ReadOnly = true;
			base.SelectedIndexChanged += ViewSelectedItem;
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		private void ViewSelectedItem(object o, EventArgs e)
		{
			SelectedListViewItemCollection selectedItems = base.SelectedItems;
			richTextBox.Clear();
			foreach (ListViewItem item in selectedItems)
			{
				int num = 0;
				foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
				{
					string text = base.Columns[num].Text;
					richTextBox.AppendText(text);
					richTextBox.SelectionStart = richTextBox.TextLength - text.Length;
					richTextBox.SelectionLength = text.Length;
					richTextBox.SelectionFont = new Font(richTextBox.SelectionFont, FontStyle.Bold);
					richTextBox.AppendText("\n");
					richTextBox.AppendText(subItem.Text);
					richTextBox.AppendText("\n\n");
					num++;
				}
			}
		}
	}
}
