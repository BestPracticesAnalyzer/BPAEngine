using Microsoft.Win32;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public abstract class CustomRegistrySettings
	{
		protected RegistryKey bpaKey;

		public virtual void Initialize(RegistryKey bpaKey)
		{
			this.bpaKey = bpaKey;
		}
	}
}
