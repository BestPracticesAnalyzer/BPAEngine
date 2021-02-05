namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPALinkOnlyScreen : BPAScreen
	{
		private MainGUI.Actions action = MainGUI.Actions.ShowCustomScreen;

		public BPALinkOnlyScreen(MainGUI.Actions action, string linkName)
		{
			this.action = action;
			base.LinkName = linkName;
		}

		public override bool Start()
		{
			mainGUI.TakeAction(action, null);
			return false;
		}
	}
}
