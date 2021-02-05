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
	public class AppType
	{
		private MainExeType mainExeField;

		private string appDirField;

		private string applicationIdField;

		public MainExeType MainExe
		{
			get
			{
				return mainExeField;
			}
			set
			{
				mainExeField = value;
			}
		}

		public string AppDir
		{
			get
			{
				return appDirField;
			}
			set
			{
				appDirField = value;
			}
		}

		[XmlAttribute]
		public string ApplicationId
		{
			get
			{
				return applicationIdField;
			}
			set
			{
				applicationIdField = value;
			}
		}
	}
}
