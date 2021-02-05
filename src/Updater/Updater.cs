using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VSPowerToys.Updater
{
	[DebuggerNonUserCode]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
	[CompilerGenerated]
	internal class Updater
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(resourceMan, null))
				{
					ResourceManager resourceManager = (resourceMan = new ResourceManager("Microsoft.VSPowerToys.Updater.Updater", typeof(Updater).Assembly));
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static string CannotFindTool
		{
			get
			{
				return ResourceManager.GetString("CannotFindTool", resourceCulture);
			}
		}

		internal static string destFile
		{
			get
			{
				return ResourceManager.GetString("destFile", resourceCulture);
			}
		}

		internal static string DirectoryNotFound
		{
			get
			{
				return ResourceManager.GetString("DirectoryNotFound", resourceCulture);
			}
		}

		internal static string Done
		{
			get
			{
				return ResourceManager.GetString("Done", resourceCulture);
			}
		}

		internal static string DownloadFailed
		{
			get
			{
				return ResourceManager.GetString("DownloadFailed", resourceCulture);
			}
		}

		internal static string Downloading
		{
			get
			{
				return ResourceManager.GetString("Downloading", resourceCulture);
			}
		}

		internal static string DownloadManifestFailure
		{
			get
			{
				return ResourceManager.GetString("DownloadManifestFailure", resourceCulture);
			}
		}

		internal static string ErrorCaption
		{
			get
			{
				return ResourceManager.GetString("ErrorCaption", resourceCulture);
			}
		}

		internal static string fileName
		{
			get
			{
				return ResourceManager.GetString("fileName", resourceCulture);
			}
		}

		internal static string from
		{
			get
			{
				return ResourceManager.GetString("from", resourceCulture);
			}
		}

		internal static string ManifestDownloaded
		{
			get
			{
				return ResourceManager.GetString("ManifestDownloaded", resourceCulture);
			}
		}

		internal static string Pending
		{
			get
			{
				return ResourceManager.GetString("Pending", resourceCulture);
			}
		}

		internal static string srcFile
		{
			get
			{
				return ResourceManager.GetString("srcFile", resourceCulture);
			}
		}

		internal static string UnzipFailed
		{
			get
			{
				return ResourceManager.GetString("UnzipFailed", resourceCulture);
			}
		}

		internal Updater()
		{
		}
	}
}
