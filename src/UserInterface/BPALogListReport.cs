using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPALogListReport : BPAListReport
	{
		private int lastLogPrinted = -1;

		public BPALogListReport(MainGUI mainGUI, BPAReportType reportType, string name)
			: base(mainGUI, reportType, name, false, false)
		{
		}

		public override void EnableItem(object itemInfo, bool enable, bool all)
		{
		}

		public override void ShowMoreInfo(object itemInfo)
		{
		}

		private int PrintHeader(PrintPageEventArgs e)
		{
			int y = e.MarginBounds.Y;
			Size size = e.Graphics.MeasureString(mname, MainGUI.TitleFont, e.MarginBounds.Width - 10).ToSize();
			Rectangle rectangle = new Rectangle(e.MarginBounds.X, y, e.MarginBounds.Width, size.Height + 20);
			e.Graphics.FillRectangle(new SolidBrush(MainGUI.DarkGray), rectangle);
			rectangle.X += 10;
			rectangle.Y += 10;
			rectangle.Width -= 10;
			rectangle.Height -= 10;
			e.Graphics.DrawString(mname, MainGUI.TitleFont, new SolidBrush(Color.White), rectangle);
			string text = BPALoc.Label_Page(mprintPage);
			Size size2 = e.Graphics.MeasureString(text, MainGUI.DefaultFont, 0).ToSize();
			rectangle.X += e.MarginBounds.Width - size2.Width - 20;
			rectangle.Size = size2;
			rectangle.Width += 20;
			e.Graphics.DrawString(text, MainGUI.DefaultFont, new SolidBrush(Color.White), rectangle);
			y += size.Height + 20 + 4;
			rectangle = new Rectangle(e.MarginBounds.X, y, e.MarginBounds.Width, MainGUI.TitleFont.Height);
			e.Graphics.DrawString((dataInfo.Label.Length == 0) ? dataInfo.RunTime.ToString() : dataInfo.Label, MainGUI.TitleFont, new SolidBrush(Color.Black), rectangle);
			return y + (rectangle.Height + 8);
		}

		private int PrintNextLog(int nextTop, PrintPageEventArgs e)
		{
			string text = (string)listBox.Items[lastLogPrinted + 1];
			Size size = e.Graphics.MeasureString(text, MainGUI.DefaultFont, e.MarginBounds.Width - CommonData.ErrorIcon.Width - 8).ToSize();
			if (nextTop + size.Height + MainGUI.DefaultFont.Height + 10 > e.MarginBounds.Bottom)
			{
				return e.MarginBounds.Bottom;
			}
			Rectangle rectangle = new Rectangle(e.MarginBounds.X, nextTop, e.MarginBounds.Width, 1);
			e.Graphics.FillRectangle(new LinearGradientBrush(rectangle, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rectangle);
			nextTop += rectangle.Height;
			rectangle = new Rectangle(e.MarginBounds.X + 8, nextTop, e.MarginBounds.Width - 8, size.Height);
			e.Graphics.DrawString(text, MainGUI.DefaultFont, new SolidBrush(Color.Black), rectangle);
			nextTop += size.Height;
			lastLogPrinted++;
			return nextTop;
		}

		protected override void PrintPage(PrintPageEventArgs e)
		{
			try
			{
				int num = PrintHeader(e);
				bool flag = false;
				e.HasMorePages = true;
				while (!flag)
				{
					num = PrintNextLog(num, e);
					if (lastLogPrinted + 1 >= listBox.Items.Count)
					{
						e.HasMorePages = false;
						lastLogPrinted = -1;
						flag = true;
					}
					if (num >= e.MarginBounds.Bottom || flag)
					{
						Rectangle rect = new Rectangle(e.MarginBounds.X, e.MarginBounds.Bottom - 8, e.MarginBounds.Width, 8);
						e.Graphics.FillRectangle(new SolidBrush(MainGUI.DarkGray), rect);
						flag = true;
					}
				}
				mprintPage++;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		protected override void PopulateArrangeByEntries()
		{
		}

		protected override void ShowReport()
		{
			mnumberOfEntries = 0;
			if (itemList == null)
			{
				return;
			}
			Regex regex = new Regex(findText.Text, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
			listBox.Items.Clear();
			for (int i = 0; i < itemList.Count; i++)
			{
				string text = (string)itemList[i];
				if (!findPanel.Visible || regex.Match(text).Success)
				{
					listBox.Items.Add(text);
				}
			}
			mnumberOfEntries = listBox.Items.Count;
			totalItems.Text = BPALoc.Label_VSTotalItems(listBox.Items.Count);
		}

		protected override void SaveAsCSV(string fileName)
		{
			StreamWriter streamWriter = null;
			try
			{
				streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
				streamWriter.WriteLine("\"Log Message\"");
				for (int i = 0; i < listBox.Items.Count; i++)
				{
					streamWriter.WriteLine("\"{0}\"", listBox.Items[i].ToString());
				}
			}
			catch (Exception arg)
			{
				mainGUI.TraceWrite(BPALoc.Label_VSExportError(arg));
			}
			finally
			{
				if (streamWriter != null)
				{
					streamWriter.Close();
				}
			}
		}

		protected override void SaveAsHTML(string fileName)
		{
			XmlTextWriter xmlTextWriter = null;
			try
			{
				xmlTextWriter = new XmlTextWriter(fileName, Encoding.UTF8);
				xmlTextWriter.Formatting = Formatting.Indented;
				xmlTextWriter.WriteStartElement("html");
				xmlTextWriter.WriteStartElement("head");
				xmlTextWriter.WriteElementString("title", mname);
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteStartElement("body");
				xmlTextWriter.WriteStartElement("table");
				xmlTextWriter.WriteAttributeString("width", "800");
				xmlTextWriter.WriteStartElement("tr");
				xmlTextWriter.WriteStartElement("td");
				xmlTextWriter.WriteAttributeString("bgcolor", "Gray");
				xmlTextWriter.WriteStartElement("font");
				xmlTextWriter.WriteAttributeString("size", "7");
				xmlTextWriter.WriteAttributeString("color", "white");
				xmlTextWriter.WriteString(mname);
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteStartElement("table");
				xmlTextWriter.WriteAttributeString("width", "800");
				xmlTextWriter.WriteAttributeString("border", "0");
				for (int i = 0; i < listBox.Items.Count; i++)
				{
					xmlTextWriter.WriteStartElement("tr");
					xmlTextWriter.WriteStartElement("td");
					xmlTextWriter.WriteAttributeString("width", "800");
					xmlTextWriter.WriteAttributeString("valign", "bottom");
					xmlTextWriter.WriteStartElement("font");
					xmlTextWriter.WriteAttributeString("color", "black");
					xmlTextWriter.WriteString(listBox.Items[i].ToString());
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteStartElement("tr");
					xmlTextWriter.WriteStartElement("td");
					if (i + 1 == listBox.Items.Count)
					{
						xmlTextWriter.WriteAttributeString("height", "16");
						xmlTextWriter.WriteAttributeString("bgcolor", "Gray");
					}
					else
					{
						xmlTextWriter.WriteAttributeString("height", "1");
						xmlTextWriter.WriteAttributeString("bgcolor", "black");
					}
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndElement();
				}
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
				xmlTextWriter.WriteEndElement();
			}
			catch (Exception arg)
			{
				mainGUI.TraceWrite(BPALoc.Label_VSExportError(arg));
			}
			finally
			{
				if (xmlTextWriter != null)
				{
					xmlTextWriter.Close();
				}
			}
		}

		protected override void DrawExpandedItem(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand)
		{
		}

		protected override void DrawUnexpandedItem(ListBox listBox, DrawItemEventArgs e, Rectangle bounds, bool toExpand)
		{
			try
			{
				Rectangle rectangle = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
				if (listBox.SelectedIndex == e.Index)
				{
					e.Graphics.FillRectangle(new SolidBrush(MainGUI.SelectColor), rectangle);
				}
				else
				{
					e.Graphics.FillRectangle(new SolidBrush(Color.White), rectangle);
				}
				string text = (string)listBox.Items[e.Index];
				e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				int num = (int)e.Graphics.MeasureString(text, MainGUI.DefaultFont, bounds.Width).Height + 8;
				if (num > bounds.Height - 2)
				{
					num = bounds.Height - 2;
				}
				rectangle = new Rectangle(bounds.X, bounds.Y + 2, bounds.Width, num);
				StringFormat stringFormat = new StringFormat();
				stringFormat.Trimming = StringTrimming.EllipsisWord;
				e.Graphics.DrawString(text, MainGUI.DefaultFont, new SolidBrush(SystemColors.ControlText), rectangle, stringFormat);
				rectangle = new Rectangle(bounds.X + 2, bounds.Y + bounds.Height - 1, bounds.Width - 2, 1);
				e.Graphics.FillRectangle(new LinearGradientBrush(rectangle, MainGUI.DarkGray, MainGUI.LightGray, 0f, false), rectangle);
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}

		protected override int MeasureExpandedItem(ListBox listBox, MeasureItemEventArgs e)
		{
			return 0;
		}

		protected override int MeasureUnexpandedItem(ListBox listBox, MeasureItemEventArgs e)
		{
			int result = 0;
			try
			{
				string text = (string)listBox.Items[e.Index];
				e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				result = (int)e.Graphics.MeasureString(text, MainGUI.DefaultFont, listBox.Width - 27 - 20).Height + 2 + 2;
				return result;
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
				return result;
			}
		}
	}
}
