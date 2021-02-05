using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.VSPowerToys.Updater.Xsd
{
	[Serializable]
	[XmlType(Namespace = "urn:schemas-microsoft-com:VSPowerToy:manifest")]
	[DesignerCategory("code")]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	public class FileMoveTask
	{
		private string srcFileField;

		private string destFileField;

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
		public string destFile
		{
			get
			{
				return destFileField;
			}
			set
			{
				destFileField = value;
			}
		}
	}
}
