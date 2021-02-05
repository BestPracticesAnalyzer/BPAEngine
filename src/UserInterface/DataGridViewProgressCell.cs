using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class DataGridViewProgressCell : DataGridViewLinkCell
	{
		private Color startColor = Color.FromArgb(0, 251, 0);

		private Color endColor = Color.FromArgb(0, 255, 0);

		private static string prefixStringColumnName = string.Empty;

		public string PrefixStringColumnName
		{
			get
			{
				return prefixStringColumnName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("PrefixStringDataPropertyName");
				}
				prefixStringColumnName = value;
			}
		}

		public override object DefaultNewRowValue
		{
			get
			{
				return 0;
			}
		}

		public Color GradientStartColor
		{
			get
			{
				return startColor;
			}
			set
			{
				startColor = value;
			}
		}

		public Color GradientEndColor
		{
			get
			{
				return endColor;
			}
			set
			{
				endColor = value;
			}
		}

		public DataGridViewProgressCell()
		{
			ValueType = typeof(int);
			Font font = new Font(Control.DefaultFont, FontStyle.Underline);
			base.Style.Font = font;
			base.LinkBehavior = LinkBehavior.AlwaysUnderline;
		}

		protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
		{
			DataGridViewRow dataGridViewRow = base.DataGridView.Rows[rowIndex];
			if (value != null && dataGridViewRow != null && dataGridViewRow.Cells[prefixStringColumnName].Value != null)
			{
				PluginStatus pluginStatus = (PluginStatus)dataGridViewRow.Cells[prefixStringColumnName].Value;
				if ((int)value > 0)
				{
					return pluginStatus.ToString() + " " + value.ToString();
				}
				return pluginStatus.ToString();
			}
			return " ";
		}

		protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			int num = (int)value;
			base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts & ~DataGridViewPaintParts.ContentForeground);
			DataGridViewRow dataGridViewRow = base.DataGridView.Rows[rowIndex];
			string text = string.Empty;
			if (!string.IsNullOrEmpty(prefixStringColumnName) && dataGridViewRow.Cells[prefixStringColumnName].Value != null)
			{
				PluginStatus pluginStatus = (PluginStatus)dataGridViewRow.Cells[prefixStringColumnName].Value;
				string text2 = null;
				switch (pluginStatus)
				{
				case PluginStatus.New:
					text2 = Resource.PluginStatusTextNew;
					break;
				case PluginStatus.UpdatesAvailable:
					text2 = Resource.PluginStatusTextUpdate;
					break;
				case PluginStatus.Installed:
					text2 = Resource.PluginStatusTextInstalled;
					break;
				default:
					text2 = pluginStatus.ToString();
					break;
				}
				text = text + text2.ToString() + " ";
			}
			if (num > 0)
			{
				Rectangle rect = new Rectangle(cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32(num * cellBounds.Width - 4), cellBounds.Height - 4);
				LinearGradientBrush brush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Vertical);
				g.FillRectangle(brush, rect);
				text = text + " " + num + "%";
			}
			base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, (object)text, (object)text, errorText, cellStyle, advancedBorderStyle, DataGridViewPaintParts.ContentForeground);
		}
	}
}
