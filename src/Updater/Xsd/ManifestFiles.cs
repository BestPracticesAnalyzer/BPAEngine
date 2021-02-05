using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.VSPowerToys.Updater.Xsd
{
	[Serializable]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[XmlType(Namespace = "urn:schemas-microsoft-com:VSPowerToy:manifest")]
	public class ManifestFiles
	{
		private ManifestFile[] fileField;

		private string baseField;

		[XmlElement("file")]
		public ManifestFile[] Files
		{
			get
			{
				return fileField;
			}
			set
			{
				fileField = value;
			}
		}

		[XmlAttribute]
		public string @base
		{
			get
			{
				return baseField;
			}
			set
			{
				baseField = value;
			}
		}
	}
}
