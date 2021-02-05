using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ValidateConfigurations
	{
		private string validationError;

		private SortedList processedConfigs = new SortedList();

		private ExecutionInterface executionInterface;

		public string ValidationError
		{
			get
			{
				return validationError;
			}
		}

		public ValidateConfigurations(ExecutionInterface executionInterface)
		{
			this.executionInterface = executionInterface;
		}

		public bool Validate(string mainConfigFileName)
		{
			processedConfigs.Clear();
			return ValidateConfigAndReferences(null, mainConfigFileName);
		}

		public bool Validate(Document mainConfig)
		{
			processedConfigs.Clear();
			return ValidateConfigAndReferences(mainConfig, mainConfig.FileName);
		}

		public bool ValidateOneFile(XmlDocument doc, string fileName)
		{
			if (executionInterface.Options.CheckConfigurationSignature && !ValidateSignature(doc, fileName))
			{
				return false;
			}
			return true;
		}

		private bool ValidateConfigAndReferences(Document config, string fileName)
		{
			if (processedConfigs.Contains(fileName))
			{
				return true;
			}
			processedConfigs.Add(fileName, true);
			if (config == null)
			{
				try
				{
					config = new BPADocument();
					((XmlDocument)config.UnderlyingDocument).PreserveWhitespace = true;
					config.FileName = fileName;
					config.Load();
				}
				catch (XmlException ex)
				{
					validationError = CommonLoc.Error_ConfigXml(fileName, ex.Message);
					return false;
				}
				catch (FileNotFoundException)
				{
					validationError = CommonLoc.Error_ConfigNotFound(fileName);
					return false;
				}
			}
			try
			{
				if (!ValidateOneFile((XmlDocument)config.UnderlyingDocument, config.FileName))
				{
					return false;
				}
			}
			catch (Exception ex3)
			{
				validationError = CommonLoc.Error_ConfigMisc(fileName, ex3.Message);
				return false;
			}
			string attribute = config.ConfigurationNode.GetAttribute("ReferencedFiles");
			if (attribute.Length > 0)
			{
				string[] array = attribute.Split(',');
				string[] array2 = array;
				foreach (string arg in array2)
				{
					if (!ValidateConfigAndReferences(null, string.Format("{0}\\{1}", Directory.GetParent(fileName), arg)))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool ValidateSignature(XmlDocument doc, string fileName)
		{
			SignedXml signedXml = new SignedXml(doc);
			XmlNodeList elementsByTagName = doc.GetElementsByTagName("Signature");
			if (elementsByTagName != null && elementsByTagName.Count > 0)
			{
				signedXml.LoadXml((XmlElement)elementsByTagName[0]);
			}
			RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
			rSACryptoServiceProvider.FromXmlString(executionInterface.Options.ConfigurationPublicKey);
			try
			{
				if (!signedXml.CheckSignature(rSACryptoServiceProvider))
				{
					validationError = CommonLoc.Error_ConfigSignature(fileName);
					return false;
				}
			}
			catch
			{
				validationError = CommonLoc.Error_ConfigSignature(fileName);
				return false;
			}
			return true;
		}
	}
}
