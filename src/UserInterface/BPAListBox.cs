using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAListBox : ListBox
	{
		public delegate void CallerDrawItem(ListBox sender, DrawItemEventArgs e, Rectangle bounds, bool toExpand);

		public delegate int CallerMeasureItem(ListBox sender, MeasureItemEventArgs e);

		public const int INDENT_VALUE = 20;

		private CallerDrawItem drawUnexpanded;

		private CallerDrawItem drawExpanded;

		private CallerMeasureItem measureUnexpanded;

		private CallerMeasureItem measureExpanded;

		private int expandedIndex = -1;

		private bool inSelectionLogic;

		private int selectHeaderAfterItemRefresh = -1;

		private bool inDrawLogic;

		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		private Font headerFont;

		private ExceptionCallback exceptionCallback;

		private bool allTextEntries;

		private int highlightedTextIndex = -1;

		public Rectangle OrigRect
		{
			get
			{
				return origRect;
			}
		}

		public BPAListBox(MainGUI mainGUI, CallerDrawItem drawUnexpanded, CallerDrawItem drawExpanded, CallerMeasureItem measureUnexpanded, CallerMeasureItem measureExpanded, bool allTextEntries, Control parent)
			: this(mainGUI.TraceError, drawUnexpanded, drawExpanded, measureUnexpanded, measureExpanded, allTextEntries, parent)
		{
		}

		public BPAListBox(ExceptionCallback exceptionCallback, CallerDrawItem drawUnexpanded, CallerDrawItem drawExpanded, CallerMeasureItem measureUnexpanded, CallerMeasureItem measureExpanded, bool allTextEntries, Control parent)
		{
			Font = MainGUI.DefaultFont;
			this.exceptionCallback = exceptionCallback;
			this.drawUnexpanded = drawUnexpanded;
			this.drawExpanded = drawExpanded;
			this.measureUnexpanded = measureUnexpanded;
			this.measureExpanded = measureExpanded;
			DrawMode = DrawMode.OwnerDrawVariable;
			base.DrawItem += LocalDrawItem;
			base.MeasureItem += LocalMeasureItem;
			base.SelectedIndexChanged += IndexChanged;
			ItemHeight = MainGUI.DefaultFont.Height + 2;
			base.BorderStyle = BorderStyle.None;
			this.allTextEntries = allTextEntries;
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
			headerFont = new Font(MainGUI.DefaultFont.FontFamily, MainGUI.DefaultFont.Size, FontStyle.Bold);
		}

		public void ClearExpandedIndex()
		{
			expandedIndex = -1;
		}

		public void SetOrigRect()
		{
			origRect.Y = base.Top;
			origRect.X = base.Left;
			origRect.Width = base.Width;
			origRect.Height = base.Height;
		}

		private void IndexChanged(object sender, EventArgs e)
		{
			try
			{
				lock (this)
				{
					if (allTextEntries && SelectedIndex != -1 && SelectedIndex < base.Items.Count)
					{
						if (highlightedTextIndex != -1 && highlightedTextIndex < base.Items.Count)
						{
							Invalidate(new Region(GetItemRectangle(highlightedTextIndex)));
						}
						Invalidate(new Region(GetItemRectangle(SelectedIndex)));
						highlightedTextIndex = SelectedIndex;
					}
					if (drawExpanded == null || inSelectionLogic || (SelectedIndex != -1 && SelectedIndex < base.Items.Count && base.Items[SelectedIndex].GetType() == typeof(string)))
					{
						return;
					}
					inSelectionLogic = true;
					int num = expandedIndex;
					if (expandedIndex != -1 && expandedIndex < base.Items.Count)
					{
						expandedIndex = -1;
						object item = base.Items[num];
						base.Items.RemoveAt(num);
						if (num >= base.Items.Count)
						{
							base.Items.Add(item);
						}
						else
						{
							base.Items.Insert(num, item);
						}
					}
					if (SelectedIndex != -1 && SelectedIndex < base.Items.Count && base.Items[SelectedIndex] is HeaderInfo)
					{
						if (num != -1 && num < base.Items.Count)
						{
							selectHeaderAfterItemRefresh = num;
						}
						else
						{
							((HeaderInfo)base.Items[SelectedIndex]).Select(SelectedIndex);
						}
					}
					else if (num != SelectedIndex && SelectedIndex != -1 && SelectedIndex < base.Items.Count && !(base.Items[SelectedIndex] is HeaderInfo))
					{
						expandedIndex = SelectedIndex;
						object item2 = base.Items[expandedIndex];
						base.Items.RemoveAt(expandedIndex);
						if (expandedIndex >= base.Items.Count)
						{
							expandedIndex = base.Items.Add(item2);
						}
						else
						{
							base.Items.Insert(expandedIndex, item2);
						}
						SelectedIndex = expandedIndex;
					}
					else
					{
						SelectedIndex = num;
					}
					inSelectionLogic = false;
				}
			}
			catch (Exception exception)
			{
				exceptionCallback(exception);
				inSelectionLogic = false;
			}
		}

		private void LocalDrawItem(object sender, DrawItemEventArgs e)
		{
			try
			{
				if (!inDrawLogic && e.Index != -1 && e.Index < base.Items.Count)
				{
					inDrawLogic = true;
					Rectangle bounds = e.Bounds;
					bounds.X += 15;
					bounds.Width -= 30;
					if (base.Items[e.Index].GetType() == typeof(string) && !allTextEntries)
					{
						e.Graphics.DrawString((string)base.Items[e.Index], headerFont, new SolidBrush(MainGUI.DarkGray), bounds.X + 2, bounds.Y + MainGUI.DefaultFont.Height);
					}
					else if (base.Items[e.Index] is HeaderInfo && !allTextEntries)
					{
						((HeaderInfo)base.Items[e.Index]).Draw(e);
					}
					else if (e.Index == expandedIndex && drawExpanded != null)
					{
						drawExpanded(this, e, bounds, true);
					}
					else
					{
						drawUnexpanded(this, e, bounds, false);
					}
					if (SelectedIndex != -1 && SelectedIndex < base.Items.Count && base.Items[SelectedIndex] is HeaderInfo && selectHeaderAfterItemRefresh == e.Index)
					{
						((HeaderInfo)base.Items[SelectedIndex]).Select(SelectedIndex);
						selectHeaderAfterItemRefresh = -1;
					}
					inDrawLogic = false;
				}
			}
			catch (Exception exception)
			{
				exceptionCallback(exception);
				inDrawLogic = false;
			}
		}

		private void LocalMeasureItem(object sender, MeasureItemEventArgs e)
		{
			try
			{
				if (e.Index != -1 && e.Index < base.Items.Count)
				{
					if (base.Items[e.Index].GetType() == typeof(string) && !allTextEntries)
					{
						e.ItemHeight = MainGUI.DefaultFont.Height * 2;
					}
					else if (base.Items[e.Index] is HeaderInfo && !allTextEntries)
					{
						e.ItemHeight = ((HeaderInfo)base.Items[e.Index]).Measure(e);
					}
					else if (e.Index == expandedIndex && measureExpanded != null)
					{
						e.ItemHeight = measureExpanded(this, e);
					}
					else
					{
						e.ItemHeight = measureUnexpanded(this, e);
					}
					if (e.ItemHeight > 255)
					{
						e.ItemHeight = 255;
					}
				}
			}
			catch (Exception exception)
			{
				exceptionCallback(exception);
			}
		}
	}
}
