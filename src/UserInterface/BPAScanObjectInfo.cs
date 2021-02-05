namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class BPAScanObjectInfo
	{
		private string name = "";

		private string group = "";

		private int pctDone;

		private MainGUI.ScanStatus scanStatus;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public int PctDone
		{
			get
			{
				return pctDone;
			}
			set
			{
				pctDone = value;
			}
		}

		public MainGUI.ScanStatus ScanStatus
		{
			get
			{
				return scanStatus;
			}
			set
			{
				scanStatus = value;
			}
		}

		public BPAScanObjectInfo(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			string text = name + " ";
			switch (scanStatus)
			{
			case MainGUI.ScanStatus.Pending:
				text += BPALoc.Label_IPServerStatusPending;
				break;
			case MainGUI.ScanStatus.InProgress:
				text += BPALoc.Label_IPServerStatusInProgress(pctDone);
				break;
			case MainGUI.ScanStatus.CompletedOk:
				text += BPALoc.Label_IPServerStatusCompleted;
				break;
			case MainGUI.ScanStatus.CompletedWithWarning:
				text += BPALoc.Label_IPServerStatusCompletedWithWarning;
				break;
			case MainGUI.ScanStatus.CompletedWithError:
				text += BPALoc.Label_IPServerStatusCompletedWithError;
				break;
			case MainGUI.ScanStatus.Aborted:
				text += BPALoc.Label_IPServerStatusAborted;
				break;
			case MainGUI.ScanStatus.NotStarted:
				text += BPALoc.Label_IPServerStatusNotStarted;
				break;
			}
			return text;
		}
	}
}
