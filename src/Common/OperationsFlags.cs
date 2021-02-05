using System;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	[Flags]
	public enum OperationsFlags
	{
		[Obsolete("The value OperationsFlags.Preprocess is no longer used.  Preprocessors are always executed if configured.")]
		Preprocess = 0x1,
		Collect = 0x2,
		Analyze = 0x4,
		Report = 0x8,
		Export = 0x10
	}
}
