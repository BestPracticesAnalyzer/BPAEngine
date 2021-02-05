using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.VSPowerToys.Updater.Xsd
{
	[Serializable]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "urn:schemas-microsoft-com:VSPowerToy:manifest")]
	public class ManifestComponent
	{
		private string descriptionField;

		private ManifestFiles filesField;

		private string nameField;

		private DateTime updatedField;

		public string Description
		{
			get
			{
				return descriptionField;
			}
			set
			{
				descriptionField = value;
			}
		}

		public ManifestFiles Files
		{
			get
			{
				return filesField;
			}
			set
			{
				filesField = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return nameField;
			}
			set
			{
				nameField = value;
			}
		}

		[XmlAttribute(DataType = "date")]
		public DateTime Updated
		{
			get
			{
				return updatedField;
			}
			set
			{
				updatedField = value;
			}
		}
	}
}
