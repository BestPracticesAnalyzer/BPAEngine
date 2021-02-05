using System.Collections;
using Microsoft.VSPowerToys.Updater.Xsd;

namespace Microsoft.VSPowerToys.Updater
{
	public class FilesCollection : CollectionBase
	{
		private string basePath;

		private Hashtable st = new Hashtable();

		public string Base
		{
			get
			{
				return basePath;
			}
		}

		public FileManifest this[int index]
		{
			get
			{
				return (FileManifest)base.List[index];
			}
			set
			{
				base.List[index] = value;
				st.Add(value.Source, base.List.IndexOf(value));
			}
		}

		public FileManifest this[string fileName]
		{
			get
			{
				int index = (int)st[fileName];
				return (FileManifest)base.List[index];
			}
			set
			{
				base.List.Add(value);
				st.Add(value.Source, base.List.IndexOf(value));
			}
		}

		public FilesCollection(ManifestFiles files)
		{
			basePath = files.@base;
			if (files != null && files.Files != null && files.Files.Length > 0)
			{
				for (int i = 0; i < files.Files.Length; i++)
				{
					FileManifest fileManifest = new FileManifest(files.Files[i]);
					base.List.Add(fileManifest);
					st.Add(fileManifest.Source, base.List.IndexOf(fileManifest));
				}
			}
		}

		public bool Contains(FileManifest value)
		{
			return base.List.Contains(value);
		}

		public bool Contains(string value)
		{
			return st.Contains(value);
		}

		public void Add(FileManifest value)
		{
			base.List.Add(value);
			st.Add(value.Source, base.List.IndexOf(value));
		}

		public void Remove(FileManifest value)
		{
			base.List.Remove(value);
			st.Remove(value.Source);
			foreach (object item in base.List)
			{
				FileManifest fileManifest = item as FileManifest;
				st[fileManifest.Source] = base.List.IndexOf(fileManifest);
			}
		}

		public void Insert(int index, FileManifest value)
		{
			base.List.Insert(index, value);
			st.Add(value.Source, index);
		}
	}
}
