using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPALink : Control
	{
		public class LinkInfo
		{
			private Keys key;

			private object tag;

			public Keys Key
			{
				get
				{
					return key;
				}
				set
				{
					key = value;
				}
			}

			public object Tag
			{
				get
				{
					return tag;
				}
				set
				{
					tag = value;
				}
			}

			public LinkInfo()
			{
				key = Keys.None;
				tag = null;
			}
		}

		public delegate void ActionCallback(int action, BPAScreen customScreen, object actionInfo, string message);

		protected ExceptionCallback exceptionCallback;

		protected ActionCallback mactionCallback;

		private int action;

		private bool isNavigational;

		private BPAScreen customScreen;

		protected LinkLabel link = new LinkLabel();

		protected PictureBox image = new PictureBox();

		protected Image enabledImage;

		protected Image disabledImage;

		private bool highlighted;

		private Rectangle origRect = new Rectangle(0, 0, 0, 0);

		private int border;

		private int lineHeight = MainGUI.DefaultFont.Height;

		private LinkInfo linkInfo;

		public bool Highlighted
		{
			get
			{
				return highlighted;
			}
			set
			{
				highlighted = value;
				if (highlighted)
				{
					link.BackColor = MainGUI.HiglightBlue;
					image.BackColor = MainGUI.HiglightBlue;
					BackColor = MainGUI.HiglightBlue;
				}
				else
				{
					link.BackColor = base.Parent.BackColor;
					image.BackColor = base.Parent.BackColor;
					BackColor = base.Parent.BackColor;
				}
			}
		}

		public string LinkLabelText
		{
			get
			{
				return link.Text;
			}
			set
			{
				link.Text = value;
			}
		}

		public new object Tag
		{
			get
			{
				return link.Tag;
			}
			set
			{
				link.Tag = value;
				linkInfo.Tag = value;
			}
		}

		public Rectangle OrigRect
		{
			get
			{
				return origRect;
			}
		}

		public BPAScreen CustomScreen
		{
			get
			{
				return customScreen;
			}
			set
			{
				customScreen = value;
			}
		}

		public Color LinkColor
		{
			get
			{
				return link.LinkColor;
			}
			set
			{
				link.LinkColor = value;
			}
		}

		public BPALink(MainGUI mainGUI, MainGUI.Actions action, bool showBorder, string text, Image image, Point location, int width, Control parent)
			: this(text, location, width, parent, image, (image == mainGUI.ArrowPic) ? mainGUI.ArrowDisabledPic : ((image == mainGUI.NavBoxPic) ? mainGUI.NavBoxDisabledPic : null), showBorder, image == mainGUI.NavBoxPic || image == mainGUI.NavBoxDisabledPic, mainGUI.TraceError, mainGUI.TakeAction, (int)action)
		{
		}

		public BPALink(string text, Point location, int width, Control parent, Image enabledImage, Image disabledImage, bool showBorder, bool isNavigational, ExceptionCallback exceptionCallback, ActionCallback actionCallback, int action)
		{
			this.exceptionCallback = exceptionCallback;
			mactionCallback = actionCallback;
			this.action = action;
			linkInfo = new LinkInfo();
			if (showBorder)
			{
				border = 4;
			}
			this.enabledImage = enabledImage;
			this.disabledImage = disabledImage;
			this.isNavigational = isNavigational;
			image.Image = enabledImage;
			image.Location = new Point(border, border);
			image.Cursor = Cursors.Hand;
			image.Size = image.Image.Size;
			if (image.Image.Height > lineHeight)
			{
				lineHeight = image.Image.Height;
			}
			image.Click += LinkClicked;
			image.KeyDown += LinkKeyDown;
			image.KeyUp += LinkKeyUp;
			image.TabStop = false;
			base.Controls.Add(image);
			link.Font = MainGUI.DefaultFont;
			link.FlatStyle = FlatStyle.System;
			link.LinkBehavior = LinkBehavior.HoverUnderline;
			if (isNavigational)
			{
				link.LinkColor = SystemColors.ControlText;
			}
			if (text.Length > 0)
			{
				link.Location = new Point(image.Right + 4, image.Top);
				link.Text = text;
			}
			else
			{
				link.Location = image.Location;
				link.Text = " ";
			}
			link.UseMnemonic = true;
			link.Click += LinkClicked;
			link.KeyDown += LinkKeyDown;
			link.KeyUp += LinkKeyUp;
			base.Controls.Add(link);
			Font = MainGUI.DefaultFont;
			Cursor = Cursors.Hand;
			base.Width = width;
			base.Size = GetSizeToFit();
			LinkResized(null, null);
			base.Location = location;
			SetOrigRect();
			base.Click += LinkClicked;
			base.Resize += LinkResized;
			base.BackColorChanged += LinkBackColorChanged;
			base.EnabledChanged += LinkEnabledChanged;
			base.TabStop = false;
			base.KeyDown += LinkKeyDown;
			base.KeyUp += LinkKeyUp;
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		public virtual Size GetSizeToFit()
		{
			return GetSizeToFitSize(base.Size);
		}

		public virtual Size GetSizeToFitSize(Size size)
		{
			Graphics graphics = link.CreateGraphics();
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			graphics.PageUnit = GraphicsUnit.Pixel;
			if (size.Width == 0)
			{
				size = Size.Ceiling(graphics.MeasureString(link.Text, link.Font));
				size.Width += image.Width + 4 + border * 2;
			}
			else
			{
				StringFormat format = new StringFormat(StringFormat.GenericTypographic.FormatFlags | StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip);
				size.Height = Size.Ceiling(graphics.MeasureString(link.Text, link.Font, size.Width - image.Width - 4 - border * 2, format)).Height;
			}
			size.Height += size.Height / link.Font.Height;
			if (size.Height > link.Font.Height)
			{
				size.Height += 3;
			}
			if (size.Height < lineHeight + border * 2)
			{
				size.Height = lineHeight + border * 2;
			}
			graphics.Dispose();
			return size;
		}

		public void SetFocus()
		{
			link.Focus();
			link.Select();
		}

		public void SetOrigRect()
		{
			origRect.Y = base.Top;
			origRect.X = base.Left;
			origRect.Width = base.Width;
			origRect.Height = base.Height;
		}

		public virtual void SetTabIndex(int nextTabIndex)
		{
			base.TabIndex = nextTabIndex;
			link.TabIndex = nextTabIndex;
		}

		public void AddClickEvent(EventHandler clickEvent)
		{
			link.Click += clickEvent;
			image.Click += clickEvent;
			base.Click += clickEvent;
		}

		public void RemoveActionCallback()
		{
			EventHandler value = LinkClicked;
			link.Click -= value;
			image.Click -= value;
			base.Click -= value;
		}

		public void ExecuteActionCallback()
		{
			mactionCallback(action, customScreen, linkInfo, "");
		}

		private void LinkBackColorChanged(object sender, EventArgs e)
		{
			try
			{
				link.BackColor = BackColor;
				image.BackColor = BackColor;
			}
			catch (Exception exception)
			{
				exceptionCallback(exception);
			}
		}

		private void LinkClicked(object sender, EventArgs e)
		{
			try
			{
				mactionCallback(action, customScreen, linkInfo, "");
			}
			catch (Exception exception)
			{
				exceptionCallback(exception);
			}
		}

		private void LinkKeyDown(object sender, KeyEventArgs e)
		{
			linkInfo.Key = e.KeyCode;
			if (e.KeyCode == Keys.Space)
			{
				LinkClicked(sender, null);
			}
		}

		private void LinkKeyUp(object sender, KeyEventArgs e)
		{
			linkInfo.Key = Keys.None;
		}

		private void LinkResized(object sender, EventArgs e)
		{
			try
			{
				Graphics graphics = link.CreateGraphics();
				graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				graphics.PageUnit = GraphicsUnit.Pixel;
				StringFormat format = new StringFormat(StringFormat.GenericTypographic.FormatFlags | StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip);
				int num = Size.Ceiling(graphics.MeasureString(link.Text, link.Font, base.Width - image.Width - 4 - border * 2, format)).Height;
				if (num > link.Font.Height)
				{
					num += 3;
				}
				int width = base.Width - image.Width - 4 - border * 2;
				link.Size = new Size(width, num);
				if (isNavigational)
				{
					link.Top = 0;
					image.Top = link.Font.Height - 1 - image.Height;
				}
				else if (image.Height >= link.Height - 2)
				{
					image.Top = 0;
					link.Top = image.Top + (image.Height - (link.Height - 2)) / 2;
				}
				else
				{
					link.Top = 0;
					image.Top = link.Top + (link.Height - 2 - image.Height) / 2;
				}
				graphics.Dispose();
			}
			catch (Exception exception)
			{
				exceptionCallback(exception);
			}
		}

		private void LinkEnabledChanged(object sender, EventArgs e)
		{
			link.Enabled = base.Enabled;
			image.Enabled = base.Enabled;
			base.Enabled = base.Enabled;
			if (image.Enabled)
			{
				image.Image = enabledImage;
			}
			else if (disabledImage != null)
			{
				image.Image = disabledImage;
			}
		}
	}
}
