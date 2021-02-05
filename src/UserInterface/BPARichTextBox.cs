using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPARichTextBox : RichTextBox
	{
		private struct RECT
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;
		}

		private struct CHARRANGE
		{
			public int cpMin;

			public int cpMax;
		}

		private struct FORMATRANGE
		{
			public IntPtr hdc;

			public IntPtr hdcTarget;

			public RECT rc;

			public RECT rcPage;

			public CHARRANGE chrg;
		}

		private const double anInch = 14.4;

		private const int WM_USER = 1024;

		private const int EM_FORMATRANGE = 1081;

		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		private bool fitAllContents = true;

		private int title;

		private int startPosition;

		private PrintDocument printDocument = new PrintDocument();

		private PrintDialog printDialog = new PrintDialog();

		public Rectangle OrigRect
		{
			get
			{
				return origRect;
			}
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

		public BPARichTextBox(Point location, Size size, bool fitAllContents, int title, Control parent)
		{
			Font = MainGUI.DefaultFont;
			base.Location = new Point(location.X, location.Y);
			base.Size = size;
			this.fitAllContents = fitAllContents;
			this.title = title;
			base.LinkClicked += LinkClickedProgram;
			MenuItem[] array = new MenuItem[4]
			{
				new MenuItem(BPALoc.ContextMenu_Copy),
				new MenuItem(BPALoc.ContextMenu_Save),
				new MenuItem(BPALoc.ContextMenu_SelectAll),
				new MenuItem(BPALoc.ContextMenu_Print)
			};
			array[0].Shortcut = Shortcut.CtrlC;
			array[0].ShowShortcut = true;
			array[0].Click += CopyEventProgram;
			array[1].Shortcut = Shortcut.CtrlS;
			array[1].ShowShortcut = true;
			array[1].Click += SaveEventProgram;
			array[2].Shortcut = Shortcut.CtrlA;
			array[2].ShowShortcut = true;
			array[2].Click += SelectAllEventProgram;
			array[3].Shortcut = Shortcut.CtrlP;
			array[3].ShowShortcut = true;
			array[3].Click += PrintEventProgram;
			ContextMenu contextMenu2 = (ContextMenu = new ContextMenu(array));
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		public object[] Setting(Node node)
		{
			return new object[1]
			{
				Text
			};
		}

		public void SetOrigRect()
		{
			origRect.Y = base.Top;
			origRect.X = base.Left;
			origRect.Width = base.Width;
			origRect.Height = base.Height;
		}

		public Size GetSizeToFit()
		{
			return GetSizeToFitSize(base.Size);
		}

		public Size GetSizeToFitSize(Size size)
		{
			if (base.Lines.Length > 0 && !fitAllContents)
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
					size.Height = 0;
					int num = 0;
					StringFormat format = new StringFormat(StringFormat.GenericTypographic.FormatFlags | StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip);
					string[] lines = base.Lines;
					foreach (string text in lines)
					{
						int num2 = 0;
						Font font = ((num >= title) ? Font : MainGUI.TitleFont);
						num2 = Size.Ceiling(graphics.MeasureString(text, font, size.Width - SystemInformation.SmallIconSize.Width - 4, format)).Height;
						if (num2 < font.Height)
						{
							num2 = font.Height;
						}
						size.Height += num2;
						num++;
					}
				}
				graphics.Dispose();
			}
			return size;
		}

		private void LinkClickedProgram(object sender, LinkClickedEventArgs e)
		{
			if (e.LinkText.Length > 0)
			{
				CommonData.BrowseURL(e.LinkText);
			}
		}

		private void CopyEventProgram(object sender, EventArgs e)
		{
			if (SelectionLength > 0)
			{
				Copy();
			}
		}

		private void SaveEventProgram(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.DefaultExt = "*.rtf";
			saveFileDialog.Filter = BPALoc.File_RTFFiles + "|*.rtf |" + BPALoc.File_TextFiles + "|*.txt";
			if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName.Length > 0)
			{
				if (saveFileDialog.FileName.EndsWith(".txt"))
				{
					SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
				}
				else
				{
					SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.RichText);
				}
			}
		}

		private void SelectAllEventProgram(object sender, EventArgs e)
		{
			SelectAll();
		}

		private void PrintEventProgram(object sender, EventArgs e)
		{
			startPosition = 0;
			printDocument.PrintPage += PrintPage;
			printDialog.Document = printDocument;
			printDialog.AllowSomePages = true;
			printDialog.ShowHelp = true;
			if (printDialog.ShowDialog() == DialogResult.OK)
			{
				printDocument.Print();
			}
		}

		private void PrintPage(object sender, PrintPageEventArgs e)
		{
			startPosition = Print(startPosition, Text.Length, e);
			e.HasMorePages = startPosition < Text.Length;
		}

		private int Print(int charFrom, int charTo, PrintPageEventArgs e)
		{
			RECT rc = default(RECT);
			rc.Top = (int)((double)e.MarginBounds.Top * 14.4);
			rc.Bottom = (int)((double)e.MarginBounds.Bottom * 14.4);
			rc.Left = (int)((double)e.MarginBounds.Left * 14.4);
			rc.Right = (int)((double)e.MarginBounds.Right * 14.4);
			RECT rcPage = default(RECT);
			rcPage.Top = (int)((double)e.PageBounds.Top * 14.4);
			rcPage.Bottom = (int)((double)e.PageBounds.Bottom * 14.4);
			rcPage.Left = (int)((double)e.PageBounds.Left * 14.4);
			rcPage.Right = (int)((double)e.PageBounds.Right * 14.4);
			IntPtr hdc = e.Graphics.GetHdc();
			FORMATRANGE fORMATRANGE = default(FORMATRANGE);
			fORMATRANGE.chrg.cpMax = charTo;
			fORMATRANGE.chrg.cpMin = charFrom;
			fORMATRANGE.hdc = hdc;
			fORMATRANGE.hdcTarget = hdc;
			fORMATRANGE.rc = rc;
			fORMATRANGE.rcPage = rcPage;
			IntPtr intPtr = IntPtr.Zero;
			IntPtr wp = new IntPtr(1);
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				intPtr2 = Marshal.AllocCoTaskMem(Marshal.SizeOf(fORMATRANGE));
				Marshal.StructureToPtr(fORMATRANGE, intPtr2, false);
				intPtr = SendMessage(base.Handle, 1081, wp, intPtr2);
			}
			finally
			{
				Marshal.FreeCoTaskMem(intPtr2);
				e.Graphics.ReleaseHdc(hdc);
			}
			return intPtr.ToInt32();
		}
	}
}
