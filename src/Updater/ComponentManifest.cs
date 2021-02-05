using System;
using Microsoft.VSPowerToys.Updater.Xsd;

namespace Microsoft.VSPowerToys.Updater
{
	public class ComponentManifest
	{
		public enum State
		{
			New,
			Update,
			Installed
		}

		private ManifestComponent component;

		private FilesCollection filesToUpdate;

		private State updateState;

		public State UpdateState
		{
			get
			{
				return updateState;
			}
			set
			{
				updateState = value;
			}
		}

		public string Name
		{
			get
			{
				return component.Name;
			}
		}

		public string Description
		{
			get
			{
				return component.Description;
			}
		}

		public DateTime LastUpdated
		{
			get
			{
				return component.Updated;
			}
		}

		public FilesCollection Files
		{
			get
			{
				if (filesToUpdate == null)
				{
					filesToUpdate = new FilesCollection(component.Files);
				}
				return filesToUpdate;
			}
		}

		public ComponentManifest(ManifestComponent Component)
		{
			component = Component;
		}
	}
}
