using System;
using System.Collections;
using System.DirectoryServices;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class ScanOptions : BPAScreen
	{
		private class RestrictionInfo
		{
			private ArrayList optionNames;

			private BPAComboBox restrictionOptions;

			public ArrayList OptionNames
			{
				get
				{
					return optionNames;
				}
			}

			public BPAComboBox RestrictionOptions
			{
				get
				{
					return restrictionOptions;
				}
			}

			public RestrictionInfo(BPAComboBox restrictionOptions)
			{
				optionNames = new ArrayList();
				this.restrictionOptions = restrictionOptions;
			}
		}

		private class SubstitutionInfo
		{
			private BPALabel name;

			private BPATextBox val;

			public BPALabel Name
			{
				get
				{
					return name;
				}
			}

			public BPATextBox Value
			{
				get
				{
					return val;
				}
			}

			public SubstitutionInfo(BPALabel name, BPATextBox val)
			{
				this.name = name;
				this.val = val;
			}
		}

		private class CredentialsInfo
		{
			private string context;

			private Panel credControls;

			public string Context
			{
				get
				{
					return context;
				}
			}

			public Panel CredControls
			{
				get
				{
					return credControls;
				}
			}

			public CredentialsInfo(string context, Panel credControls)
			{
				this.context = context;
				this.credControls = credControls;
			}
		}

		private const string NO_RESTRICTION = "<all>";

		private Analyzer analyzer;

		private BPALink linkStart;

		private BPATextBox label;

		private BPATextBox adServer;

		private int nextTabIndex;

		private RestrictionInfo[] restrictionInfos;

		private SubstitutionInfo[] substitutionInfos;

		private CredentialsInfo[] credentialsInfos;

		public string Label
		{
			get
			{
				return label.Text;
			}
		}

		public string ADServer
		{
			get
			{
				if (adServer == null)
				{
					return "";
				}
				return adServer.Text;
			}
		}

		public ArrayList RestrictionOptions
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				if (restrictionInfos != null)
				{
					RestrictionInfo[] array = restrictionInfos;
					foreach (RestrictionInfo restrictionInfo in array)
					{
						if (restrictionInfo.RestrictionOptions.SelectedItem.ToString() != "<all>")
						{
							arrayList.Add(restrictionInfo.OptionNames[restrictionInfo.RestrictionOptions.SelectedIndex].ToString());
						}
					}
				}
				return arrayList;
			}
		}

		public SortedList Substitutions
		{
			get
			{
				SortedList sortedList = new SortedList();
				if (substitutionInfos != null)
				{
					SubstitutionInfo[] array = substitutionInfos;
					foreach (SubstitutionInfo substitutionInfo in array)
					{
						sortedList.Add(substitutionInfo.Name.Text, substitutionInfo.Value.Text);
					}
				}
				return sortedList;
			}
		}

		public Credentials[] CredentialsList
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				if (credentialsInfos != null)
				{
					CredentialsInfo[] array = credentialsInfos;
					foreach (CredentialsInfo credentialsInfo in array)
					{
						if (((BPACheckBox)credentialsInfo.CredControls.Controls[1]).Checked)
						{
							Credentials value = new Credentials(credentialsInfo.Context, credentialsInfo.CredControls.Controls[3].Text, credentialsInfo.CredControls.Controls[7].Text, credentialsInfo.CredControls.Controls[5].Text);
							arrayList.Add(value);
						}
					}
				}
				return (Credentials[])arrayList.ToArray(typeof(Credentials));
			}
		}

		public ScanOptions(Analyzer analyzer)
		{
			Dock = DockStyle.Fill;
			AutoScroll = true;
			BackColor = Color.White;
			this.analyzer = analyzer;
			base.LinkName = analyzer.StartScan;
		}

		public override void Initialize(MainGUI mainGUI)
		{
			base.Controls.Clear();
			linkStart = null;
			label = null;
			adServer = null;
			restrictionInfos = null;
			credentialsInfos = null;
			substitutionInfos = null;
			nextTabIndex = 0;
			base.Initialize(mainGUI);
			nextTabIndex = mainGUI.StartingTabIndex;
			Point borderCornerPoint = MainGUI.BorderCornerPoint;
			BPATitle bPATitle = new BPATitle(analyzer.SSTitle, borderCornerPoint, analyzer.CommonGUI.FullWidth, this);
			bPATitle.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.Below(bPATitle, 1f);
			BPALabel bPALabel = new BPALabel(analyzer.SSLabel, borderCornerPoint, 250, this);
			bPALabel.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.NextTo(bPALabel);
			label = new BPATextBox(borderCornerPoint, 2f, this);
			label.TabIndex = nextTabIndex++;
			borderCornerPoint = Navigate.Below(bPALabel);
			if (mainGUI.ExecInterface.Options.Configuration.GetNode("//Object[@Type='Directory' and not(@Key1)]") != null)
			{
				BPALabel bPALabel2 = new BPALabel(analyzer.SSADServer, borderCornerPoint, 250, this);
				bPALabel2.TabIndex = nextTabIndex++;
				borderCornerPoint = Navigate.NextTo(bPALabel2);
				adServer = new BPATextBox(borderCornerPoint, 2f, this);
				if (adServer.Width < label.Right)
				{
					adServer.Left = label.Right - adServer.Width;
					adServer.SetOrigRect();
				}
				adServer.TabIndex = nextTabIndex++;
				adServer.Text = GetNearestGC();
				borderCornerPoint = Navigate.Below(bPALabel2);
			}
			Node[] nodes = mainGUI.ExecInterface.Options.Configuration.GetNodes("/*/Configuration/Substitution");
			if (nodes != null && nodes.Length > 0)
			{
				BPAGradiatedBox control = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, borderCornerPoint, new Size(analyzer.CommonGUI.FullWidth, 1), this);
				borderCornerPoint = Navigate.Below(control);
				substitutionInfos = new SubstitutionInfo[nodes.Length];
				int num = 0;
				Node[] array = nodes;
				foreach (Node node in array)
				{
					string attribute = node.GetAttribute("Name");
					BPALabel bPALabel3 = new BPALabel(attribute, borderCornerPoint, 250, this);
					bPALabel3.TabIndex = nextTabIndex++;
					borderCornerPoint = Navigate.NextTo(bPALabel3);
					borderCornerPoint.X = label.Left;
					BPATextBox bPATextBox = new BPATextBox(borderCornerPoint, 2f, this);
					if (bPATextBox.Width < label.Right)
					{
						bPATextBox.Left = label.Right - bPATextBox.Width;
						bPATextBox.SetOrigRect();
					}
					bPATextBox.TabIndex = nextTabIndex++;
					bPATextBox.Text = node.Value;
					borderCornerPoint = Navigate.Below(bPALabel3);
					SubstitutionInfo substitutionInfo = new SubstitutionInfo(bPALabel3, bPATextBox);
					substitutionInfos[num++] = substitutionInfo;
				}
			}
			Node[] nodes2 = mainGUI.ExecInterface.Options.Configuration.GetNodes("/*/Configuration/RestrictionType");
			if (nodes2 != null && nodes2.Length > 0)
			{
				BPAGradiatedBox control2 = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, borderCornerPoint, new Size(analyzer.CommonGUI.FullWidth, 1), this);
				borderCornerPoint = Navigate.Below(control2);
				restrictionInfos = new RestrictionInfo[nodes2.Length];
				int num2 = 0;
				Node[] array2 = nodes2;
				foreach (Node node2 in array2)
				{
					string attribute2 = node2.GetAttribute("Display");
					if (attribute2.Length == 0)
					{
						attribute2 = node2.GetAttribute("Name");
					}
					BPALabel bPALabel4 = new BPALabel(attribute2, borderCornerPoint, 250, this);
					bPALabel4.TabIndex = nextTabIndex++;
					borderCornerPoint = Navigate.NextTo(bPALabel4);
					borderCornerPoint.X = label.Left;
					RestrictionInfo restrictionInfo = new RestrictionInfo(new BPAComboBox(borderCornerPoint, 1f, this));
					restrictionInfo.RestrictionOptions.TabIndex = nextTabIndex + 1;
					restrictionInfo.RestrictionOptions.Tag = bPALabel4.Text;
					Node[] nodes3 = node2.GetNodes("Option");
					Node[] array3 = nodes3;
					foreach (Node node3 in array3)
					{
						string attribute3 = node3.GetAttribute("Display");
						if (attribute3.Length == 0)
						{
							attribute3 = node3.GetAttribute("Name");
						}
						restrictionInfo.RestrictionOptions.Items.Add(attribute3);
						restrictionInfo.OptionNames.Add(node3.GetAttribute("Name"));
					}
					restrictionInfo.RestrictionOptions.SelectedIndex = 0;
					restrictionInfos[num2++] = restrictionInfo;
					borderCornerPoint = Navigate.Below(bPALabel4);
				}
			}
			Node[] nodes4 = mainGUI.ExecInterface.Options.Configuration.GetNodes("//Object[@SecurityContext]");
			if (nodes4 != null && nodes4.Length > 0)
			{
				BPAGradiatedBox control3 = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, borderCornerPoint, new Size(analyzer.CommonGUI.FullWidth, 1), this);
				borderCornerPoint = Navigate.Below(control3);
				SortedList sortedList = new SortedList();
				Node[] array4 = nodes4;
				foreach (Node node4 in array4)
				{
					sortedList[node4.GetAttribute("SecurityContext")] = 1;
				}
				credentialsInfos = new CredentialsInfo[sortedList.Count];
				int num3 = 0;
				foreach (string key in sortedList.Keys)
				{
					int tabIndex = nextTabIndex++;
					Panel panel = CreateCredentialsPanel(key);
					BPAShowHide bPAShowHide = new BPAShowHide(analyzer.CommonGUI, "Show Credentials Fields for " + key, "Hide Credentials Fields for " + key, panel, borderCornerPoint, analyzer.CommonGUI.FullWidth - borderCornerPoint.X, this);
					bPAShowHide.SetTabIndex(tabIndex);
					borderCornerPoint = Navigate.Below(bPAShowHide);
					CredentialsInfo credentialsInfo = new CredentialsInfo(key, panel);
					credentialsInfos[num3++] = credentialsInfo;
				}
			}
			BPAGradiatedBox control4 = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, borderCornerPoint, new Size(analyzer.CommonGUI.FullWidth, 1), this);
			borderCornerPoint = Navigate.Below(control4);
			borderCornerPoint = Navigate.Indent(borderCornerPoint);
			linkStart = new BPALink(analyzer.CommonGUI, MainGUI.Actions.StartScan, false, BPALoc.LinkLabel_SSStartScan, analyzer.CommonGUI.ArrowPic, borderCornerPoint, 0, this);
			linkStart.SetTabIndex(nextTabIndex++);
			borderCornerPoint = Navigate.Below(linkStart);
		}

		public override bool Start()
		{
			return true;
		}

		private string GetNearestGC()
		{
			string text = "";
			try
			{
				string str = "GC://";
				DirectoryEntry directoryEntry = new DirectoryEntry(str + "rootDSE");
				directoryEntry.AuthenticationType = AuthenticationTypes.ReadonlyServer;
				text = directoryEntry.Properties["serverName"][0].ToString();
				text = text.Remove(0, 3);
				return text.Substring(0, text.IndexOf(","));
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
				return "";
			}
		}

		private Panel CreateCredentialsPanel(string context)
		{
			Point location = new Point(0, 10);
			BPAPanel bPAPanel = new BPAPanel();
			bPAPanel.BorderStyle = BorderStyle.None;
			BPAGradiatedBox control = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, location, new Size(analyzer.CommonGUI.FullWidth, 1), bPAPanel);
			location = Navigate.Below(control, 0.5f);
			BPACheckBox bPACheckBox = new BPACheckBox("Enable credentials for context " + context, location, analyzer.CommonGUI.FullWidth - 10, bPAPanel);
			bPACheckBox.Checked = false;
			bPACheckBox.Tag = bPAPanel;
			bPACheckBox.CheckedChanged += CredsChecked;
			bPACheckBox.TabIndex = nextTabIndex++;
			location = Navigate.Below(bPACheckBox);
			BPALabel bPALabel = new BPALabel("User name:", location, analyzer.CommonGUI.FullWidth / 3, bPAPanel);
			bPALabel.TabIndex = nextTabIndex++;
			location = Navigate.NextTo(bPALabel);
			BPATextBox bPATextBox = new BPATextBox(location, 1f, bPAPanel);
			bPATextBox.Enabled = false;
			bPATextBox.TabIndex = nextTabIndex++;
			location = Navigate.Below(bPALabel);
			BPALabel bPALabel2 = new BPALabel("Password:", location, analyzer.CommonGUI.FullWidth / 3, bPAPanel);
			bPALabel2.TabIndex = nextTabIndex++;
			location = Navigate.NextTo(bPALabel2);
			BPATextBox bPATextBox2 = new BPATextBox(location, 1f, bPAPanel);
			bPATextBox2.Enabled = false;
			bPATextBox2.PasswordChar = '*';
			bPATextBox2.TabIndex = nextTabIndex++;
			location = Navigate.Below(bPALabel2);
			BPALabel bPALabel3 = new BPALabel("Domain:", location, analyzer.CommonGUI.FullWidth / 3, bPAPanel);
			bPALabel3.TabIndex = nextTabIndex++;
			location = Navigate.NextTo(bPALabel3);
			BPATextBox bPATextBox3 = new BPATextBox(location, 1f, bPAPanel);
			bPATextBox3.Enabled = false;
			bPATextBox3.TabIndex = nextTabIndex++;
			location = Navigate.Below(bPALabel3);
			BPAGradiatedBox control2 = new BPAGradiatedBox(MainGUI.DarkGray, MainGUI.LightGray, location, new Size(analyzer.CommonGUI.FullWidth, 1), bPAPanel);
			location = Navigate.Below(control2);
			return bPAPanel;
		}

		private void CredsChecked(object sender, EventArgs e)
		{
			try
			{
				CheckBox checkBox = (CheckBox)sender;
				((Panel)checkBox.Tag).Controls[3].Enabled = checkBox.Checked;
				((Panel)checkBox.Tag).Controls[5].Enabled = checkBox.Checked;
				((Panel)checkBox.Tag).Controls[7].Enabled = checkBox.Checked;
			}
			catch (Exception exception)
			{
				analyzer.CommonGUI.TraceError(exception);
			}
		}
	}
}
