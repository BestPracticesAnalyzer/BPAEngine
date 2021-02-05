using System;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class AboutBox : Form
	{
		private MainGUI mainGUI;

		public AboutBox(MainGUI mainGUI)
		{
			this.mainGUI = mainGUI;
			base.Icon = mainGUI.MainForm.Icon;
			Text = mainGUI.Customizations.ShortName;
			base.ShowInTaskbar = false;
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.MinimizeBox = false;
			base.MaximizeBox = false;
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			base.Size = mainGUI.AboutPic.Size;
			base.Paint += PaintAbout;
			Button button = new Button();
			button.FlatStyle = FlatStyle.System;
			button.Text = BPALoc.Button_OK;
			button.Width = 100;
			button.Left = base.Width - button.Width - 20;
			button.Top = base.Height - button.Height - 40;
			button.Click += OkClicked;
			base.Controls.Add(button);
		}

		private void OkClicked(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.OK;
		}

		private void PaintAbout(object sender, PaintEventArgs e)
		{
			try
			{
				Control control = (Control)sender;
				Point location = new Point(0, 0);
				Rectangle rectangle = new Rectangle(location, mainGUI.AboutPic.Size);
				e.Graphics.DrawImage(mainGUI.AboutPic, rectangle, rectangle, GraphicsUnit.Pixel);
				Point point = new Point(65, 170);
				Font font = new Font("Microsoft Sans Serif", 16f, FontStyle.Regular, GraphicsUnit.Point, 0);
				e.Graphics.DrawString(mainGUI.Customizations.LongNameStart, font, new SolidBrush(Color.Black), point);
				point.Y += font.Height;
				e.Graphics.DrawString(mainGUI.Customizations.LongNameEnd, font, new SolidBrush(Color.Red), point);
				point.Y += font.Height + 15;
				font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular, GraphicsUnit.Point, 0);
				e.Graphics.DrawString(BPALoc.Label_AAppVersion + " " + mainGUI.ExecInterface.EngineVersion.ToString(), font, new SolidBrush(Color.Black), point);
				point.Y += font.Height + 15;
				e.Graphics.DrawString(BPALoc.Label_AConfigVersion + " " + mainGUI.ConfigInfo.ConfigVersion.ToString(), font, new SolidBrush(Color.Black), point);
				point.Y += font.Height + 15;
				point.X -= 10;
				font = new Font("Microsoft Sans Serif", 8f, FontStyle.Regular, GraphicsUnit.Point, 0);
				e.Graphics.DrawString("©2006 Microsoft Corporation. All rights reserved.", font, new SolidBrush(Color.Black), point);
				point.X += 10;
				e.Graphics.DrawImage(mainGUI.MSLogoDarkPic, base.Width - mainGUI.MSLogoDarkPic.Width - 20, point.Y);
				point.Y += mainGUI.BPALogoPic.Height + 10;
				e.Graphics.DrawLine(new Pen(Color.Black), point, new Point(base.Width - 20, point.Y));
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
		}
	}
}
