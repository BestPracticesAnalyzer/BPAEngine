using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPANumericInput : BPATextBox, IBPADataControl
	{
		private int minValue;

		private int maxValue;

		private int defaultValue;

		public BPANumericInput(Point location, float scale, int minValue, int maxValue, int defaultValue, Control parent)
			: base(location, scale, parent)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
			this.defaultValue = defaultValue;
			base.Validated += VerifyInput;
			Text = defaultValue.ToString();
		}

		public new object[] Setting(Node node)
		{
			int num = defaultValue;
			try
			{
				num = Convert.ToInt32(Text);
			}
			catch (FormatException)
			{
			}
			return new object[1]
			{
				num
			};
		}

		private void VerifyInput(object sender, EventArgs e)
		{
			int num = defaultValue;
			try
			{
				num = Convert.ToInt32(Text);
			}
			catch (FormatException)
			{
			}
			if (num < minValue || num > maxValue)
			{
				num = defaultValue;
			}
			Text = num.ToString();
		}
	}
}
