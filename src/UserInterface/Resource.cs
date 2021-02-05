using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

[DebuggerNonUserCode]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
[CompilerGenerated]
internal class Resource
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
				ResourceManager resourceManager = (resourceMan = new ResourceManager("Resource", typeof(Resource).Assembly));
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

	internal static string ActionColumnLabel
	{
		get
		{
			return ResourceManager.GetString("ActionColumnLabel", resourceCulture);
		}
	}

	internal static string ActionLabelNull
	{
		get
		{
			return ResourceManager.GetString("ActionLabelNull", resourceCulture);
		}
	}

	internal static string AnalyzerCountFormatString
	{
		get
		{
			return ResourceManager.GetString("AnalyzerCountFormatString", resourceCulture);
		}
	}

	internal static string AnalyzerNameNull
	{
		get
		{
			return ResourceManager.GetString("AnalyzerNameNull", resourceCulture);
		}
	}

	internal static string AvailableAnalyzerFormatString
	{
		get
		{
			return ResourceManager.GetString("AvailableAnalyzerFormatString", resourceCulture);
		}
	}

	internal static string AvailableAnalyzerString
	{
		get
		{
			return ResourceManager.GetString("AvailableAnalyzerString", resourceCulture);
		}
	}

	internal static string DownloadingString
	{
		get
		{
			return ResourceManager.GetString("DownloadingString", resourceCulture);
		}
	}

	internal static string FooterLabel
	{
		get
		{
			return ResourceManager.GetString("FooterLabel", resourceCulture);
		}
	}

	internal static string ManifestDownloadFailed
	{
		get
		{
			return ResourceManager.GetString("ManifestDownloadFailed", resourceCulture);
		}
	}

	internal static string ManifestUpdating
	{
		get
		{
			return ResourceManager.GetString("ManifestUpdating", resourceCulture);
		}
	}

	internal static string NameColumnLabel
	{
		get
		{
			return ResourceManager.GetString("NameColumnLabel", resourceCulture);
		}
	}

	internal static string PluginNameNull
	{
		get
		{
			return ResourceManager.GetString("PluginNameNull", resourceCulture);
		}
	}

	internal static string PluginStatusTextInstalled
	{
		get
		{
			return ResourceManager.GetString("PluginStatusTextInstalled", resourceCulture);
		}
	}

	internal static string PluginStatusTextNew
	{
		get
		{
			return ResourceManager.GetString("PluginStatusTextNew", resourceCulture);
		}
	}

	internal static string PluginStatusTextUpdate
	{
		get
		{
			return ResourceManager.GetString("PluginStatusTextUpdate", resourceCulture);
		}
	}

	internal static string ProgressColumnLabel
	{
		get
		{
			return ResourceManager.GetString("ProgressColumnLabel", resourceCulture);
		}
	}

	internal static string RunAnalyzerLabel
	{
		get
		{
			return ResourceManager.GetString("RunAnalyzerLabel", resourceCulture);
		}
	}

	internal static string StatusColumnLabel
	{
		get
		{
			return ResourceManager.GetString("StatusColumnLabel", resourceCulture);
		}
	}

	internal static string UpdateInProgressMsg
	{
		get
		{
			return ResourceManager.GetString("UpdateInProgressMsg", resourceCulture);
		}
	}

	internal Resource()
	{
	}
}
