using System;
using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataDataPostprocessor : DataPostprocessor
	{
		public DataDataPostprocessor(ExecutionInterface executionInterface, Document data)
			: base(executionInterface, data)
		{
		}

		public override void ProcessData()
		{
			DataInstance dataInstance = null;
			StepID stepID = new StepID("1");
			Node[] nodes = data.GetNodes("//*[name()='Setting' or name()='Object']/Rule[Data]");
			Node[] array = nodes;
			foreach (Node node in array)
			{
				string attribute = node.GetAttribute("DataID");
				dataInstance = ((attribute.Length != 0) ? DataInstance.GetInstanceFromID(attribute) : DataObjectProcessor.GetRootInstance());
				Node[] nodes2 = node.GetNodes("Data");
				Node[] array2 = nodes2;
				foreach (Node node2 in array2)
				{
					try
					{
						dataInstance.ProcessNode(node2, stepID, executionInterface);
					}
					catch (Exception ex)
					{
						if (executionInterface.Trace)
						{
							executionInterface.LogText("Error Processing node: {0}", ex.Message);
						}
					}
				}
			}
		}
	}
}
