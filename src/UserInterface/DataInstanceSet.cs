using System;
using System.Collections;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class DataInstanceSet : IEnumerable
	{
		private ArrayList instances = new ArrayList();

		private DataObject dataObject;

		private string finalName;

		public DataObject DataObject
		{
			get
			{
				return dataObject;
			}
		}

		public int Count
		{
			get
			{
				return instances.Count;
			}
		}

		public string FinalName
		{
			get
			{
				return finalName;
			}
			set
			{
				finalName = value;
			}
		}

		public DataInstance this[int index]
		{
			get
			{
				return (DataInstance)instances[index];
			}
		}

		public DataInstanceSet()
		{
		}

		public DataInstanceSet(DataObject dataObject)
		{
			this.dataObject = dataObject;
		}

		public DataInstanceSet(DataInstance instance)
		{
			dataObject = instance.DataObject;
			instances.Add(instance);
		}

		public IEnumerator GetEnumerator()
		{
			return instances.GetEnumerator();
		}

		public int IndexOf(DataInstance instance)
		{
			return instances.IndexOf(instance);
		}

		public void Add(DataInstance instance)
		{
			if (instance.DataObject != dataObject)
			{
				throw new Exception("Tried to add incompatible instance to set.");
			}
			if (!instances.Contains(instance))
			{
				instances.Add(instance);
			}
		}

		public void Clear()
		{
			instances.Clear();
		}

		public object[] ToArray()
		{
			return instances.ToArray();
		}

		public bool Contains(string name)
		{
			foreach (DataInstance instance in instances)
			{
				if (instance.Name == name)
				{
					return true;
				}
			}
			return false;
		}

		public void Remove(string name)
		{
			ArrayList arrayList = new ArrayList();
			foreach (DataInstance instance in instances)
			{
				if (instance.Name == name)
				{
					arrayList.Add(instance);
				}
			}
			foreach (DataInstance item in arrayList)
			{
				instances.Remove(item);
			}
		}

		public void Merge(DataInstanceSet dataInstanceSet)
		{
			if (dataObject != dataInstanceSet.dataObject)
			{
				throw new Exception("Trying to merge incompatible instance sets.");
			}
			if (dataInstanceSet == null || dataInstanceSet.instances.Count == 0)
			{
				return;
			}
			foreach (DataInstance instance in dataInstanceSet.instances)
			{
				if (!instances.Contains(instance))
				{
					instances.Add(instance);
				}
			}
		}

		public ArrayList ConvertToStrings()
		{
			ArrayList arrayList = new ArrayList();
			foreach (object instance in instances)
			{
				arrayList.Add(instance.ToString());
			}
			return arrayList;
		}
	}
}
