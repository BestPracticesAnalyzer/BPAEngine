using System;
using System.Data;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPADataGrid : DataGrid, IBPADataControl
	{
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
				return string.Empty;
			}
			set
			{
			}
		}

		public event EventHandler DataChanged
		{
			add
			{
			}
			remove
			{
			}
		}

		public BPADataGrid(Point location, Size size, Control parent)
		{
			Font = MainGUI.DefaultFont;
			base.Location = location;
			base.Size = size;
			SetOrigRect();
			if (parent != null)
			{
				parent.Controls.Add(this);
			}
		}

		public void SetOrigRect()
		{
			origRect.Y = base.Top;
			origRect.X = base.Left;
			origRect.Width = base.Width;
			origRect.Height = base.Height;
		}

		public object[] Setting(Node node)
		{
			int currentRowIndex = base.CurrentRowIndex;
			string attribute = node.GetAttribute("Key1");
			object[] result = null;
			DataTable dataTable = base.DataSource as DataTable;
			if (dataTable != null)
			{
				DataRow dataRow = dataTable.Rows[currentRowIndex];
				object obj = dataRow[attribute];
				if (obj != null)
				{
					result = new object[1]
					{
						obj
					};
				}
			}
			return result;
		}

		public void Highlight(bool highlight)
		{
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keys)
		{
			return false;
		}
	}
}
