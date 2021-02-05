using System.Collections;
using System.Collections.ObjectModel;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.UserInterface
{
	public class AnalyzerCollection : CollectionBase
	{
		private Hashtable st = new Hashtable();

		public Analyzer this[int index]
		{
			get
			{
				return (Analyzer)base.List[index];
			}
			set
			{
				base.List[index] = value;
				st.Add(value.ToString(), base.List.IndexOf(value));
			}
		}

		public Analyzer this[string name]
		{
			get
			{
				int index = (int)st[name];
				return (Analyzer)base.List[index];
			}
			set
			{
				base.List.Add(value);
				st.Add(value.ToString(), base.List.IndexOf(value));
			}
		}

		public AnalyzerCollection(ReadOnlyCollection<Analyzer> plugins)
		{
			foreach (Analyzer plugin in plugins)
			{
				Add(plugin);
			}
		}

		public bool Contains(Analyzer value)
		{
			return base.List.Contains(value);
		}

		public void Add(Analyzer value)
		{
			base.List.Add(value);
			st.Add(value.ToString(), base.List.IndexOf(value));
		}

		public void Remove(Analyzer value)
		{
			base.List.Remove(value);
			st.Remove(value.ToString());
			foreach (object item in base.List)
			{
				Analyzer analyzer = item as Analyzer;
				st[analyzer.ToString()] = base.List.IndexOf(analyzer);
			}
		}
	}
}
