using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPATitle : BPALabel
	{
		public BPATitle(string text, Point location, int width, Control parent)
			: this(text, location, width, Color.Empty, MainGUI.DarkGray, MainGUI.TitleFont, parent)
		{
		}

		public BPATitle(string text, Point location, int width, Color backColor, Color foreColor, Font font, Control parent)
			: base(text, location, width, backColor, foreColor, font, parent)
		{
			Font = font;
			ForeColor = foreColor;
			BackColor = backColor;
			Text = text;
			base.Width = width;
			base.Size = GetSizeToFit();
			SetOrigRect();
		}
	}
}
