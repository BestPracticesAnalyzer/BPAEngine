using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.VSPowerToys.Updater.Xsd
{
	[Serializable]
	[XmlRoot(Namespace = "urn:schemas-microsoft-com:VSPowerToy:manifest", IsNullable = true)]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "urn:schemas-microsoft-com:VSPowerToy:manifest")]
	public class Manifest
	{
		private AppType applicationField;

		private string descriptionField;

		private ManifestComponents componentsField;

		private string manifestIdField;

		public AppType Application
		{
			get
			{
				return applicationField;
			}
			set
			{
				applicationField = value;
			}
		}

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

		public ManifestComponents Components
		{
			get
			{
				return componentsField;
			}
			set
			{
				componentsField = value;
			}
		}

		[XmlAttribute]
		public string ManifestId
		{
			get
			{
				return manifestIdField;
			}
			set
			{
				manifestIdField = value;
			}
		}
	}
}
