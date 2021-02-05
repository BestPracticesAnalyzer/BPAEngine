using System;
using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataObject
	{
		private SortedList children = new SortedList();

		public ArrayList Objects
		{
			get
			{
				return new ArrayList(children.Keys);
			}
		}

		public DataObjectLink GetObject(string targetName)
		{
			if (children.Contains(targetName))
			{
				return (DataObjectLink)children[targetName];
			}
			throw new ArgumentException();
		}

		public bool HasObject(string name)
		{
			return children.Contains(name);
		}

		public DataObjectLink CreateObject(string sourceName, string targetName)
		{
			DataObjectLink dataObjectLink;
			if (children.Contains(targetName))
			{
				dataObjectLink = (DataObjectLink)children[targetName];
				if (dataObjectLink.Backlink.Name != sourceName)
				{
					throw new Exception("Tried to change backlink on existing link");
				}
			}
			else
			{
				DataObject dataObject = new DataObject();
				dataObjectLink = new DataObjectLink(this, sourceName, dataObject, targetName);
				children[targetName] = dataObjectLink;
				dataObject.children[sourceName] = dataObjectLink.Backlink;
			}
			return dataObjectLink;
		}

		public DataObjectLink LinkObject(string sourceName, string targetName, DataObject target)
		{
			DataObjectLink dataObjectLink;
			if (children.Contains(targetName))
			{
				dataObjectLink = (DataObjectLink)children[targetName];
				if (dataObjectLink.Backlink.Name != sourceName)
				{
					throw new Exception("Tried to change backlink on existing link");
				}
				if (dataObjectLink.Target != target)
				{
					throw new Exception("Tried to change target on existing link");
				}
			}
			else
			{
				if (target.children.Contains(sourceName))
				{
					throw new Exception("Tried to reset an existing backlink");
				}
				dataObjectLink = new DataObjectLink(this, sourceName, target, targetName);
				children[targetName] = dataObjectLink;
				target.children[sourceName] = dataObjectLink.Backlink;
			}
			return dataObjectLink;
		}
	}
}
