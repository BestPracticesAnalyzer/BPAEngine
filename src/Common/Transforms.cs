using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class Transforms
	{
		private static Hashtable hash;

		private static bool initialized;

		public static XslCompiledTransform Get(string name)
		{
			if (!initialized)
			{
				Initialize();
			}
			return (XslCompiledTransform)hash[name];
		}

		public static XmlDocument Apply(XmlDocument doc, string transformName, XsltArgumentList args)
		{
			if (!initialized)
			{
				Initialize();
			}
			XslCompiledTransform xslCompiledTransform = Get(transformName);
			XmlUrlResolver xmlUrlResolver = new XmlUrlResolver();
			xmlUrlResolver.Credentials = CredentialCache.DefaultCredentials;
			XmlReader input;
			if (string.IsNullOrEmpty(doc.BaseURI))
			{
				MemoryStream memoryStream = new MemoryStream();
				XmlSerializer xmlSerializer = new XmlSerializer(doc.GetType());
				XmlWriter xmlWriter = XmlWriter.Create(memoryStream);
				xmlSerializer.Serialize(xmlWriter, doc);
				memoryStream.Position = 0L;
				input = new XmlTextReader(memoryStream);
			}
			else
			{
				input = new XmlTextReader(doc.BaseURI);
			}
			XmlDocument xmlDocument = new XmlDocument();
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xslCompiledTransform.Transform(input, args, xmlTextWriter, xmlUrlResolver);
			xmlTextWriter.Flush();
			stringWriter.Flush();
			xmlDocument.LoadXml(stringWriter.ToString());
			return xmlDocument;
		}

		private static void Initialize()
		{
			hash = new Hashtable();
			XmlUrlResolver xmlUrlResolver = new XmlUrlResolver();
			xmlUrlResolver.Credentials = CredentialCache.DefaultCredentials;
			string[] manifestResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
			string[] array = manifestResourceNames;
			foreach (string text in array)
			{
				if (text.EndsWith(".xsl"))
				{
					XmlDocument xmlDocument = new XmlDocument();
					XslCompiledTransform xslCompiledTransform = new XslCompiledTransform();
					xmlDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(text));
					xslCompiledTransform.Load(xmlDocument.CreateNavigator(), new XsltSettings(false, true), xmlUrlResolver);
					hash[text.Substring(0, text.Length - 4)] = xslCompiledTransform;
				}
			}
			initialized = true;
		}
	}
}
