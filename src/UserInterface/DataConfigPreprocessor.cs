using Microsoft.VSPowerToys.BestPracticesAnalyzer.Common;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataConfigPreprocessor : ConfigPreprocessor
	{
		public DataConfigPreprocessor(ExecutionInterface execInterface, Document config)
			: base(execInterface, config)
		{
		}

		public override void ProcessConfiguration()
		{
			Node node = config.GetNode("/*/Configuration");
			Node node2 = config.CreateNode("Substitution");
			node2.SetAttribute("Name", "__DATA_ID");
			node.Add(node2);
			Node[] nodes = config.GetNodes("//Object[@Type='Data']");
			Node[] array = nodes;
			foreach (Node node3 in array)
			{
				node3.SetAttribute("DataID", "%__DATA_ID%");
				Node node4 = config.CreateNode("Setting");
				node4.SetAttribute("Key1", "__DATA_ID");
				node4.SetAttribute("Substitution", "__DATA_ID");
				node4.SetAttribute("Display", "HideAll");
				Node node5 = node3.GetNode("*");
				if (node5 == null)
				{
					node3.Add(node4);
				}
				else
				{
					node3.InsertBefore(node4, node5);
				}
			}
			nodes = config.GetNodes("//Rule[@NextStep or Data]");
			Node[] array2 = nodes;
			foreach (Node node6 in array2)
			{
				node6.SetAttribute("DataID", "%__DATA_ID%");
			}
			nodes = config.GetNodes("//*[@*[contains(., 'data(')]]");
			Node[] array3 = nodes;
			foreach (Node node7 in array3)
			{
				node7.SetAttribute("DataID", "%__DATA_ID%");
			}
		}
	}
}
