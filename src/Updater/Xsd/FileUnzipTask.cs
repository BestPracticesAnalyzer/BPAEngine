using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.VSPowerToys.Updater.Xsd
{
	[Serializable]
	[XmlType(Namespace = "urn:schemas-microsoft-com:VSPowerToy:manifest")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.42")]
	public class FileUnzipTask
	{
		private string srcFileField;

		private string destDirField;

		[XmlAttribute]
		public string srcFile
		{
			get
			{
				return srcFileField;
			}
			set
			{
				srcFileField = value;
			}
		}

		[XmlAttribute]
		public string destDir
		{
			get
			{
				return destDirField;
			}
			set
			{
				destDirField = value;
			}
		}
	}
}
