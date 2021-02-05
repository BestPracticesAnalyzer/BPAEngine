using System.Collections;
using System.Collections.Generic;
using Microsoft.VSPowerToys.Updater.Xsd;

namespace Microsoft.VSPowerToys.Updater
{
	public class ComponentsCollection : CollectionBase
	{
		private Hashtable st = new Hashtable();

		private string basePath;

		public string Base
		{
			get
			{
				return basePath;
			}
		}

		public ComponentManifest this[int index]
		{
			get
			{
				return (ComponentManifest)base.List[index];
			}
			set
			{
				base.List[index] = value;
				st.Add(value.Name, base.List.IndexOf(value));
			}
		}

		public ComponentManifest this[string name]
		{
			get
			{
				if (st.ContainsKey(name))
				{
					int index = (int)st[name];
					return (ComponentManifest)base.List[index];
				}
				return null;
			}
			set
			{
				base.List.Add(value);
				st.Add(value.Name, base.List.IndexOf(value));
			}
		}

		public ComponentsCollection(ManifestComponents components)
		{
			basePath = components.@base;
			if (components != null && components.Components != null && components.Components.Length > 0)
			{
				for (int i = 0; i < components.Components.Length; i++)
				{
					ComponentManifest value = new ComponentManifest(components.Components[i]);
					base.List.Add(value);
					st.Add(components.Components[i].Name, base.List.IndexOf(value));
				}
			}
		}

		public bool Contains(ComponentManifest value)
		{
			return base.List.Contains(value);
		}

		public void Add(ComponentManifest value)
		{
			base.List.Add(value);
			st.Add(value.Name, base.List.IndexOf(value));
		}

		public void Remove(ComponentManifest value)
		{
			if (!st.ContainsKey(value.Name))
			{
				return;
			}
			int num = (int)st[value.Name];
			List<string> list = new List<string>();
			foreach (string key in st.Keys)
			{
				if ((int)st[key] > num)
				{
					list.Add(key);
				}
			}
			foreach (string item in list)
			{
				st[item] = (int)st[item] - 1;
			}
			st.Remove(value.Name);
			base.List.RemoveAt(num);
		}

		public void Insert(int index, ComponentManifest value)
		{
			base.List.Insert(index, value);
			st.Add(value.Name, index);
		}
	}
}
