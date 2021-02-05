using System.Xml.XPath;
using System.Xml.Xsl;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ExtVariable : IXsltContextVariable
	{
		private string name;

		public bool IsLocal
		{
			get
			{
				return false;
			}
		}

		public bool IsParam
		{
			get
			{
				return false;
			}
		}

		public XPathResultType VariableType
		{
			get
			{
				return XPathResultType.Any;
			}
		}

		public ExtVariable(string name)
		{
			this.name = name;
		}

		public object Evaluate(XsltContext context)
		{
			return ((CustomContext)context).GetParam(name);
		}
	}
}
