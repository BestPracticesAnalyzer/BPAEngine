using System;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public interface IBPADataControl
	{
		string DataValue
		{
			get;
			set;
		}

		event EventHandler DataChanged;

		object[] Setting(Node node);

		void Highlight(bool highlight);
	}
}
