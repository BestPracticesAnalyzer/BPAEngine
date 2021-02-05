using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class CustomContext : XsltContext
	{
		private XsltArgumentList argList;

		public override bool Whitespace
		{
			get
			{
				return true;
			}
		}

		public CustomContext()
			: base(new NameTable())
		{
			argList = new XsltArgumentList();
		}

		public object GetParam(string name)
		{
			return argList.GetParam(name, null);
		}

		public void AddParam(string name, object parameter)
		{
			argList.AddParam(name, "", parameter);
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			return new ExtVariable(name);
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
		{
			ExtFunction extFunction = ExtFunction.Get(name);
			if (extFunction == null)
			{
				throw new ExDiagRuleFormatException(string.Format("Can't find function '{0}'", name));
			}
			return extFunction;
		}

		public override int CompareDocument(string baseUri, string nextBaseUri)
		{
			return 0;
		}

		public override bool PreserveWhitespace(XPathNavigator node)
		{
			return true;
		}
	}
}
