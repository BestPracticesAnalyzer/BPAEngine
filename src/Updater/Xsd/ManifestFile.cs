using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.VSPowerToys.Updater.Xsd
{
	[Serializable]
	[XmlType(Namespace = "urn:schemas-microsoft-com:VSPowerToy:manifest")]
	[GeneratedCode("xsd", "2.0.50727.42")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	public class ManifestFile
	{
		private InstallTasks installTasksField;

		private string sourceField;

		private string hashField;

		private string queryField;

		private XmlAttribute[] anyAttrField;

		public InstallTasks InstallTasks
		{
			get
			{
				return installTasksField;
			}
			set
			{
				installTasksField = value;
			}
		}

		[XmlAttribute]
		public string Source
		{
			get
			{
				return sourceField;
			}
			set
			{
				sourceField = value;
			}
		}

		[XmlAttribute]
		public string Hash
		{
			get
			{
				return hashField;
			}
			set
			{
				hashField = value;
			}
		}

		[XmlAttribute]
		public string query
		{
			get
			{
				return queryField;
			}
			set
			{
				queryField = value;
			}
		}

		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttr
		{
			get
			{
				return anyAttrField;
			}
			set
			{
				anyAttrField = value;
			}
		}
	}
}
