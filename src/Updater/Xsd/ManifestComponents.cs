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
	public class ManifestComponents
	{
		private ManifestComponent[] componentField;

		private string baseField;

		[XmlElement("Component")]
		public ManifestComponent[] Components
		{
			get
			{
				return componentField;
			}
			set
			{
				componentField = value;
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
