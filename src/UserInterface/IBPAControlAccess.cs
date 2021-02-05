using System.Windows.Forms;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public interface IBPAControlAccess
	{
		Control EffectivePositioningControl
		{
			get;
		}

		Control EffectiveContainmentControl
		{
			get;
		}
	}
}
