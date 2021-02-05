using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class NodeList : IEnumerable
	{
		private ArrayList nodeList;

		public int Count
		{
			get
			{
				return nodeList.Count;
			}
		}

		public Node this[int index]
		{
			get
			{
				return (Node)nodeList[index];
			}
		}

		public NodeList()
		{
			nodeList = new ArrayList();
		}

		public void Add(Node node)
		{
			nodeList.Add(node);
		}

		public IEnumerator GetEnumerator()
		{
			return nodeList.GetEnumerator();
		}
	}
}
