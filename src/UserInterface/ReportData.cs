using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class ReportData
	{
		private ArrayList issueList;

		private ArrayList logList;

		protected NodeInfo mdetailNodes;

		protected NodeInfo msummaryNodes;

		public ArrayList IssueList
		{
			get
			{
				return issueList;
			}
		}

		public ArrayList LogList
		{
			get
			{
				return logList;
			}
		}

		public NodeInfo DetailNodes
		{
			get
			{
				return mdetailNodes;
			}
		}

		public NodeInfo SummaryNodes
		{
			get
			{
				return msummaryNodes;
			}
		}

		public ReportData()
		{
			issueList = new ArrayList();
			logList = new ArrayList();
			mdetailNodes = new NodeInfo();
			msummaryNodes = new NodeInfo();
		}
	}
}
