using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAAnalyzerDataGridView : DataGridView
	{
		private DataSet ds;

		private int updateRowIndex;

		private bool updateInProgress;

		private object syncObject = new object();

		private DataGridViewCellStyle dataGridViewAlternateRowStyle = new DataGridViewCellStyle();

		private DataGridViewCellStyle dataGridViewColumnHeaderStyle = new DataGridViewCellStyle();

		private DataGridViewCellStyle dataGridViewRowHeaderStyle = new DataGridViewCellStyle();

		private DataGridViewCellStyle dataGridViewRowStyle = new DataGridViewCellStyle();

		public bool UpdateInProgress
		{
			get
			{
				lock (this)
				{
					return updateInProgress;
				}
			}
			set
			{
				lock (this)
				{
					updateInProgress = value;
				}
			}
		}

		public event EventHandler<RunAnalyzerEventArgs> RunAnalyzerEvent;

		public event EventHandler<StatusButtonClickEventArgs> StatusButtonClickEvent;

		public virtual void OnRunAnalyzerEvent(RunAnalyzerEventArgs e)
		{
			if (this.RunAnalyzerEvent != null)
			{
				this.RunAnalyzerEvent(this, e);
			}
		}

		public virtual void OnStatusButtonClickEvent(StatusButtonClickEventArgs e)
		{
			if (this.StatusButtonClickEvent != null)
			{
				this.StatusButtonClickEvent(this, e);
			}
		}

		public void Initialize()
		{
			lock (syncObject)
			{
				SetupDataSet();
				SetupDataGridView();
			}
		}

		private void SetupDataSet()
		{
			ds = new DataSet("Analyzers");
			ds.Tables.Add("Main");
			ds.Tables["Main"].Columns.Add("Name");
			ds.Tables["Main"].Columns.Add("Action");
			ds.Tables["Main"].Columns.Add("Status");
			ds.Tables["Main"].Columns.Add("Progress");
			ds.Tables["Main"].Columns["Progress"].DataType = typeof(int);
			ds.Tables["Main"].Columns["Status"].DataType = typeof(PluginStatus);
			DataColumn[] primaryKey = new DataColumn[1]
			{
				ds.Tables["Main"].Columns["Name"]
			};
			ds.Tables["Main"].PrimaryKey = primaryKey;
		}

		public void AddNewDataSetRow(string name, string action, PluginStatus status)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(Resource.PluginNameNull);
			}
			if (string.IsNullOrEmpty(action))
			{
				throw new ArgumentNullException(Resource.ActionLabelNull);
			}
			DataRow dataRow = ds.Tables["Main"].NewRow();
			dataRow["Name"] = name;
			dataRow["Action"] = action;
			dataRow["Status"] = status;
			dataRow["Progress"] = 0;
			lock (syncObject)
			{
				ds.Tables["Main"].Rows.Add(dataRow);
			}
		}

		public void UpdateStatus(string pluginName, PluginStatus status)
		{
			lock (syncObject)
			{
				DataRow dataRow = ds.Tables["Main"].Rows.Find(pluginName);
				if (dataRow != null)
				{
					dataRow["Status"] = status;
				}
				switch (status)
				{
				case PluginStatus.Downloading:
					UpdateInProgress = true;
					dataRow["Progress"] = 0;
					break;
				case PluginStatus.Installed:
					UpdateInProgress = false;
					dataRow["Progress"] = 0;
					break;
				}
			}
			Invalidate();
		}

		public void UpdateProgress(string pluginName, int value)
		{
			lock (syncObject)
			{
				DataRow dataRow = ds.Tables["Main"].Rows.Find(pluginName);
				if (dataRow != null)
				{
					dataRow["Progress"] = value;
				}
			}
			Invalidate();
		}

		private void BPAAnalyzerDataGridView_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
		{
			e.Row.Cells["Name"].Value = string.Empty;
			e.Row.Cells["Action"].Value = string.Empty;
			e.Row.Cells["Status"].Value = PluginStatus.Installed;
			e.Row.Cells["Progress"].Value = 0;
		}

		private void SetupDataGridView()
		{
			base.AutoGenerateColumns = false;
			DataGridViewTextBoxColumn dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn.Name = "Name";
			dataGridViewTextBoxColumn.HeaderText = Resource.NameColumnLabel;
			dataGridViewTextBoxColumn.DataPropertyName = "Name";
			DataGridViewLinkColumn dataGridViewLinkColumn = new DataGridViewLinkColumn();
			dataGridViewLinkColumn.Name = "Action";
			dataGridViewLinkColumn.HeaderText = Resource.ActionColumnLabel;
			dataGridViewLinkColumn.DataPropertyName = "Action";
			DataGridViewTextBoxColumn dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
			dataGridViewTextBoxColumn2.Name = "Status";
			dataGridViewTextBoxColumn2.HeaderText = string.Empty;
			dataGridViewTextBoxColumn2.DataPropertyName = "Status";
			dataGridViewTextBoxColumn2.Visible = false;
			DataGridViewProgressColumn dataGridViewProgressColumn = new DataGridViewProgressColumn();
			dataGridViewProgressColumn.Name = "Progress";
			dataGridViewProgressColumn.HeaderText = Resource.ProgressColumnLabel;
			dataGridViewProgressColumn.DataPropertyName = "Progress";
			DataGridViewProgressCell dataGridViewProgressCell = dataGridViewProgressColumn.CellTemplate as DataGridViewProgressCell;
			if (dataGridViewProgressCell != null)
			{
				dataGridViewProgressCell.PrefixStringColumnName = "Status";
			}
			base.Columns.Add(dataGridViewTextBoxColumn);
			base.Columns.Add(dataGridViewLinkColumn);
			base.Columns.Add(dataGridViewTextBoxColumn2);
			base.Columns.Add(dataGridViewProgressColumn);
			base.CellClick += BPAAnalyzerDataGridView_CellClick;
			base.CurrentCellDirtyStateChanged += BPAAnalyzerDataGridView_CurrentCellDirtyStateChanged;
			base.CellDoubleClick += BPAAnalyzerDataGridView_CellDoubleClick;
			base.DefaultValuesNeeded += BPAAnalyzerDataGridView_DefaultValuesNeeded;
			base.CellFormatting += BPAAnalyzerDataGridView_CellFormatting;
			base.CellPainting += BPAAnalyzerDataGridView_CellPainting;
			base.DataSource = ds;
			base.DataMember = "Main";
			dataGridViewAlternateRowStyle.Font = SystemFonts.DefaultFont;
			dataGridViewAlternateRowStyle.ForeColor = SystemColors.WindowText;
			dataGridViewAlternateRowStyle.BackColor = SystemColors.Window;
			dataGridViewAlternateRowStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewAlternateRowStyle.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewColumnHeaderStyle.Font = SystemFonts.DefaultFont;
			dataGridViewColumnHeaderStyle.ForeColor = SystemColors.WindowText;
			dataGridViewColumnHeaderStyle.BackColor = SystemColors.GradientActiveCaption;
			dataGridViewColumnHeaderStyle.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewColumnHeaderStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewRowHeaderStyle.Font = SystemFonts.DefaultFont;
			dataGridViewRowHeaderStyle.ForeColor = SystemColors.WindowText;
			dataGridViewRowHeaderStyle.BackColor = SystemColors.Window;
			dataGridViewRowHeaderStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewRowHeaderStyle.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewRowStyle.Font = SystemFonts.DefaultFont;
			dataGridViewRowStyle.ForeColor = SystemColors.WindowText;
			dataGridViewRowStyle.BackColor = SystemColors.Window;
			dataGridViewRowStyle.SelectionBackColor = SystemColors.Highlight;
			dataGridViewRowStyle.SelectionForeColor = SystemColors.HighlightText;
			base.AlternatingRowsDefaultCellStyle = dataGridViewAlternateRowStyle;
			base.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			base.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
			base.BackgroundColor = Color.White;
			dataGridViewColumnHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewColumnHeaderStyle.WrapMode = DataGridViewTriState.True;
			base.ColumnHeadersDefaultCellStyle = dataGridViewColumnHeaderStyle;
			base.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			Dock = DockStyle.Fill;
			base.GridColor = SystemColors.GradientActiveCaption;
			base.Location = new Point(0, 0);
			base.MultiSelect = false;
			base.Name = "dataGridView1";
			base.ReadOnly = true;
			dataGridViewRowHeaderStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewRowHeaderStyle.WrapMode = DataGridViewTriState.True;
			base.RowHeadersDefaultCellStyle = dataGridViewRowHeaderStyle;
			base.RowsDefaultCellStyle = dataGridViewRowStyle;
			base.AllowUserToAddRows = false;
			base.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			base.Size = new Size(579, 353);
		}

		private void BPAAnalyzerDataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex < 0)
			{
				return;
			}
			if (e.ColumnIndex == base.Columns["Status"].Index)
			{
				DataGridViewLinkCell dataGridViewLinkCell = (DataGridViewLinkCell)base.Rows[e.RowIndex].Cells[e.ColumnIndex];
				switch ((PluginStatus)e.Value)
				{
				case PluginStatus.UpdatesAvailable:
					dataGridViewLinkCell.LinkColor = Color.Yellow;
					break;
				case PluginStatus.New:
					dataGridViewLinkCell.LinkColor = Color.Red;
					break;
				default:
					dataGridViewLinkCell.LinkColor = Color.Green;
					break;
				}
			}
			if (base.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewLinkCell)
			{
				DataGridViewLinkCell dataGridViewLinkCell2 = (DataGridViewLinkCell)base.Rows[e.RowIndex].Cells[e.ColumnIndex];
				bool selected = base.Rows[e.RowIndex].Selected;
				dataGridViewLinkCell2.LinkColor = (selected ? SystemColors.HighlightText : SystemColors.WindowText);
			}
		}

		private void BPAAnalyzerDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.ColumnIndex == base.Columns["Status"].Index)
			{
				e.Value = e.Value.ToString();
				e.FormattingApplied = true;
			}
		}

		private void BPAAnalyzerDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0)
			{
				if (!UpdateInProgress)
				{
					RunAnalyzerEventArgs runAnalyzerEventArgs = new RunAnalyzerEventArgs();
					runAnalyzerEventArgs.AnalyzerName = base.Rows[e.RowIndex].Cells["Name"].Value.ToString();
					OnRunAnalyzerEvent(runAnalyzerEventArgs);
				}
				else
				{
					MessageBox.Show(Resource.UpdateInProgressMsg + base.Rows[e.RowIndex].Cells["Name"].Value.ToString());
				}
			}
		}

		private void BPAAnalyzerDataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (base.IsCurrentCellDirty)
			{
				CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
		}

		private void BPAAnalyzerDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0)
			{
				return;
			}
			if (e.ColumnIndex == base.Columns["Progress"].Index)
			{
				DataGridViewCell dataGridViewCell = base.Rows[e.RowIndex].Cells["Action"];
				if (!UpdateInProgress)
				{
					updateRowIndex = e.RowIndex;
					StatusButtonClickEventArgs statusButtonClickEventArgs = new StatusButtonClickEventArgs();
					statusButtonClickEventArgs.AnalyzerName = base.Rows[e.RowIndex].Cells["Name"].Value.ToString();
					OnStatusButtonClickEvent(statusButtonClickEventArgs);
				}
			}
			else if (e.ColumnIndex == base.Columns["Action"].Index)
			{
				if (!UpdateInProgress)
				{
					RunAnalyzerEventArgs runAnalyzerEventArgs = new RunAnalyzerEventArgs();
					runAnalyzerEventArgs.AnalyzerName = base.Rows[e.RowIndex].Cells["Name"].Value.ToString();
					OnRunAnalyzerEvent(runAnalyzerEventArgs);
				}
				else
				{
					MessageBox.Show(Resource.UpdateInProgressMsg + base.Rows[e.RowIndex].Cells["Name"].Value.ToString());
				}
			}
		}
	}
}
