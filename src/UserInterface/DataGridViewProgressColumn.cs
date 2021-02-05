using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataGridViewProgressColumn : DataGridViewLinkColumn
	{
		public DataGridViewProgressColumn()
		{
			CellTemplate = new DataGridViewProgressCell();
		}
	}
}
