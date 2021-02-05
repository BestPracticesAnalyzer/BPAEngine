using System;
using System.Runtime.Serialization;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ExDiagRuleFormatException : ApplicationException, ISerializable
	{
		public ExDiagRuleFormatException(string message)
			: base(message)
		{
		}
	}
}
