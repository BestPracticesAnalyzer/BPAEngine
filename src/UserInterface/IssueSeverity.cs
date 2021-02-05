using System;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	[Flags]
	public enum IssueSeverity
	{
		Unknown = 0x1,
		Info = 0x2,
		NonDefault = 0x4,
		RecentChange = 0x8,
		Baseline = 0x10,
		BestPractice = 0x20,
		Warning = 0x40,
		Error = 0x80
	}
}
