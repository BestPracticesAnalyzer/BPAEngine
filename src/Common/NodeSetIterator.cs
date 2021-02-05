using System;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class NodeSetIterator : XPathNodeIterator
	{
		private int position;

		private ArrayList navigatorList = new ArrayList();

		public override int Count
		{
			get
			{
				return navigatorList.Count;
			}
		}

		public override int CurrentPosition
		{
			get
			{
				return position;
			}
		}

		public override XPathNavigator Current
		{
			get
			{
				if (position == 0)
				{
					return null;
				}
				return (XPathNavigator)navigatorList[position - 1];
			}
		}

		public XPathNodeIterator ResetableIterator
		{
			get
			{
				Assembly assembly = typeof(XPathNodeIterator).Assembly;
				Type type = assembly.GetType("System.Xml.XPath.XPathArrayIterator");
				if (type == null)
				{
					return this;
				}
				return (XPathNodeIterator)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, new object[1]
				{
					navigatorList
				}, null);
			}
		}

		public override bool MoveNext()
		{
			return ++position <= navigatorList.Count;
		}

		public override XPathNodeIterator Clone()
		{
			NodeSetIterator nodeSetIterator = new NodeSetIterator();
			nodeSetIterator.navigatorList = navigatorList;
			nodeSetIterator.position = position;
			return nodeSetIterator;
		}

		public void Add(XPathNavigator navigator)
		{
			navigatorList.Add(navigator);
		}

		public void Add(XmlNode element)
		{
			navigatorList.Add(element.CreateNavigator());
		}
	}
}
