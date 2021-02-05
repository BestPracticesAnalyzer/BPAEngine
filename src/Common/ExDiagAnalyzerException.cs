using System;
using System.Runtime.Serialization;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ExDiagAnalyzerException : ApplicationException, ISerializable
	{
		public ExDiagAnalyzerException(string message)
			: base(message)
		{
		}
	}
}
