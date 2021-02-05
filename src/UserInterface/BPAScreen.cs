using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public abstract class BPAScreen : Panel
	{
		protected MainGUI mainGUI;

		private BPALink link;

		private string linkName;

		private string helpId;

		public BPALink Link
		{
			get
			{
				return link;
			}
			set
			{
				link = value;
			}
		}

		public string LinkName
		{
			get
			{
				return linkName;
			}
			set
			{
				linkName = value;
			}
		}

		public string HelpId
		{
			get
			{
				if (helpId != null)
				{
					return helpId;
				}
				return mainGUI.Customizations.RootHelpId;
			}
			set
			{
				helpId = value;
			}
		}

		public BPAScreen()
		{
		}

		public BPAScreen(MainGUI mainGUI)
		{
			this.mainGUI = mainGUI;
		}

		public virtual bool Start()
		{
			return true;
		}

		public virtual void Initialize(MainGUI mainGUI)
		{
			this.mainGUI = mainGUI;
		}

		public virtual void EnableLink(bool enable)
		{
			if (link != null)
			{
				link.Enabled = enable;
			}
		}
	}
}
