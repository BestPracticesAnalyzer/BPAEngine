using System;
using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	internal class LoadScanAction
	{
		public delegate void DoneDelegate();

		private DataInfo dataInfo;

		private LoadScan loadScan;

		private MainGUI mainGUI;

		private int startingPct;

		public LoadScanAction(LoadScan loadScan, MainGUI mainGUI, DataInfo dataInfo, int startingPct)
		{
			this.loadScan = loadScan;
			this.mainGUI = mainGUI;
			this.dataInfo = dataInfo;
			this.startingPct = startingPct;
		}

		public void Start()
		{
			try
			{
				mainGUI.ExecInterface.Parameters.ProcessParameters();
				foreach (BPAReport customReport in mainGUI.Customizations.CustomReports)
				{
					customReport.Build(mainGUI.ExecInterface.Options.Data);
				}
				mainGUI.ExecInterface.Options.Data.ClearDocument();
				dataInfo.GenerateDataLists(mainGUI.Customizations.CustomReports, startingPct, mainGUI.RegSettings.MsgSuppress, loadScan.LoadStatus);
			}
			catch (Exception exception)
			{
				mainGUI.TraceError(exception);
			}
			loadScan.LoadDone();
			Application.ExitThread();
		}
	}
}
