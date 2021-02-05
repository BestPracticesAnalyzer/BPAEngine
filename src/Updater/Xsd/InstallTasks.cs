using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.VSPowerToys.Updater.Xsd
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[XmlType(Namespace = "urn:schemas-microsoft-com:VSPowerToy:manifest")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class InstallTasks
	{
		private object[] itemsField;

		[XmlElement("FileMove", typeof(FileMoveTask))]
		[XmlElement("FileUnzip", typeof(FileUnzipTask))]
		public object[] Items
		{
			get
			{
				return itemsField;
			}
			set
			{
				itemsField = value;
			}
		}
	}
}
