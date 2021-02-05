using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAListView : ListView, IBPADataControl
	{
		public class ColumnComparer : IComparer
		{
			private int col;

			public ColumnComparer(int column)
			{
				col = column;
			}

			public int Compare(object x, object y)
			{
				return Comparer.DefaultInvariant.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
			}
		}

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
				CheckedListViewItemCollection checkedItems = base.CheckedItems;
				IEnumerator enumerator = checkedItems.GetEnumerator();
				ArrayList arrayList = new ArrayList();
				while (enumerator.MoveNext())
				{
					ListViewItem listViewItem = (ListViewItem)enumerator.Current;
					ArrayList arrayList2 = new ArrayList();
					foreach (ListViewItem.ListViewSubItem subItem in listViewItem.SubItems)
					{
						arrayList2.Add(subItem.Text);
					}
					if (base.Columns.Count > 1)
					{
						arrayList.Add(CommonData.EncodeStringArray(arrayList2));
					}
					else if (base.Columns.Count == 1)
					{
						arrayList.Add((string)arrayList2[0]);
					}
				}
				return CommonData.EncodeStringArray(arrayList);
			}
			set
			{
				ArrayList arrayList = CommonData.DecodeStringArray(value);
				base.Items.Clear();
				for (int i = 0; i < arrayList.Count; i++)
				{
					IEnumerable enumerable = (IEnumerable)((base.Columns.Count <= 1) ? ((object)new string[1]
					{
						arrayList[i].ToString()
					}) : ((object)CommonData.DecodeStringArray((string)arrayList[i])));
					IEnumerator enumerator = enumerable.GetEnumerator();
					enumerator.MoveNext();
					ListViewItem listViewItem = new ListViewItem((string)enumerator.Current, 0);
					listViewItem.Checked = false;
					int num = 1;
					while (enumerator.MoveNext() && num < base.Columns.Count)
					{
						listViewItem.SubItems.Add((string)enumerator.Current);
						num++;
					}
					base.Items.Add(listViewItem);
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

		public BPAListView(Point location, Size size, bool sort, Control parent)
		{
			Font = MainGUI.DefaultFont;
			base.Location = location;
			base.Size = size;
			base.View = View.Details;
			base.FullRowSelect = true;
			base.MultiSelect = false;
			if (sort)
			{
				base.ColumnClick += ColumnClicked;
			}
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		private void ColumnClicked(object o, ColumnClickEventArgs e)
		{
			base.ListViewItemSorter = new ColumnComparer(e.Column);
		}

		public object[] Setting(Node node)
		{
			string text = string.Empty;
			switch (node.GetAttribute("Key1"))
			{
			case "Delimiter":
			{
				CheckedListViewItemCollection checkedItems = base.CheckedItems;
				IEnumerator enumerator2 = checkedItems.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					ListViewItem listViewItem = (ListViewItem)enumerator2.Current;
					foreach (ListViewItem.ListViewSubItem subItem in listViewItem.SubItems)
					{
						text = text + subItem.Text + ",";
					}
					text = text.TrimEnd(',') + ";";
				}
				text = text.TrimEnd(';');
				break;
			}
			case "Pack":
				text = DataValue;
				break;
			default:
			{
				CheckedIndexCollection checkedIndices = base.CheckedIndices;
				foreach (int item in checkedIndices)
				{
					text = text + checkedIndices[item] + ",";
				}
				text = text.TrimEnd(',');
				break;
			}
			}
			return new object[1]
			{
				text
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
