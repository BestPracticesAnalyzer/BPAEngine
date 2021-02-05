using System;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ExtFunction : IXsltContextFunction
	{
		public delegate object ExtDelegate(XsltContext context, object[] args, XPathNavigator nav);

		private string name;

		private int minargs;

		private int maxargs;

		private XPathResultType[] argTypes;

		private XPathResultType returnType;

		private ExtDelegate function;

		public static ExecutionInterface ExecutionInterface;

		private static Hashtable hash;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public int Minargs
		{
			get
			{
				return minargs;
			}
		}

		public int Maxargs
		{
			get
			{
				return maxargs;
			}
		}

		public XPathResultType[] ArgTypes
		{
			get
			{
				return argTypes;
			}
		}

		public XPathResultType ReturnType
		{
			get
			{
				return returnType;
			}
		}

		public static ExtFunction Get(string name)
		{
			return (ExtFunction)hash[name];
		}

		private static XPathResultType XType(string type)
		{
			switch (type)
			{
			case "Any":
				return XPathResultType.Any;
			case "Boolean":
				return XPathResultType.Boolean;
			case "Navigator":
				return XPathResultType.String;
			case "NodeSet":
				return XPathResultType.NodeSet;
			case "Number":
				return XPathResultType.Number;
			case "String":
				return XPathResultType.String;
			default:
				return XPathResultType.Error;
			}
		}

		internal static void Add(string name, Node node, LoadedProcessor loadedProcessor)
		{
			string attribute = node.GetAttribute("Function");
			string attribute2 = node.GetAttribute("Parameters");
			string attribute3 = node.GetAttribute("Returns");
			ExtDelegate extDelegate = (ExtDelegate)Delegate.CreateDelegate(typeof(ExtDelegate), loadedProcessor.ProcessorType, attribute);
			Add(name, attribute3, attribute2, extDelegate);
		}

		public static void Add(string name, string returns, string parameters, ExtDelegate function)
		{
			int num = 0;
			int num2 = 0;
			XPathResultType xPathResultType = XType(returns);
			string text = parameters;
			int num3 = parameters.IndexOf("[");
			if (num3 != -1)
			{
				text = parameters.Substring(0, num3);
			}
			string[] array = text.Split(',');
			string[] array2 = parameters.Split(',');
			XPathResultType[] array3 = new XPathResultType[array.Length];
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i] = XType(array[i]);
			}
			num = array3.Length;
			num2 = array2.Length;
			hash[name] = new ExtFunction(name, num, num2, array3, xPathResultType, function);
		}

		static ExtFunction()
		{
			Reset();
		}

		public static void Reset()
		{
			hash = new Hashtable();
			Add("upper-case", "String", "String", UpperCase);
			Add("lower-case", "String", "String", LowerCase);
			Add("hex", "Number", "String", Hex);
			Add("matches", "Boolean", "String,String", Matches);
			Add("replace", "String", "String,String,String", Replace);
			Add("join", "String", "String,Any[,Any,Any,Any]", Join);
			Add("slice", "String", "String,String,Number", Slice);
			Add("bitwise-and", "Number", "Number,Number", BitwiseAnd);
			Add("bitwise-or", "Number", "Number,Number", BitwiseOr);
			Add("if", "NodeSet", "Boolean,Any[,Any]", If);
			Add("date-difference", "String", "String,String", DateDifference);
			Add("date-add", "String", "String,String", DateAdd);
			Add("get-days", "Number", "String", GetDays);
			Add("get-seconds", "Number", "String", GetSeconds);
			Add("date-parse", "String", "String,String", DateParse);
			Add("get-timespan", "String", "Number", GetTimeSpan);
			Add("sdcheck", "Number", "String,Any,Number,Number,Number[,Number,[String,[String]]]", NTSD.Sdcheck);
			Add("version-compare", "Number", "String,String", VersionCompare);
		}

		public static double Numberify(object obj)
		{
			if (obj is double)
			{
				return (double)obj;
			}
			if (obj is bool)
			{
				return ((bool)obj) ? 1.0 : 0.0;
			}
			try
			{
				return double.Parse(Stringify(obj));
			}
			catch
			{
				return double.NaN;
			}
		}

		public static string Stringify(object obj)
		{
			if (obj is XPathNodeIterator)
			{
				((XPathNodeIterator)obj).MoveNext();
				if (((XPathNodeIterator)obj).Current != null)
				{
					return ((XPathNodeIterator)obj).Current.Value;
				}
				return "";
			}
			if (obj is double)
			{
				return ((double)obj).ToString("0.####################", CultureInfo.InvariantCulture);
			}
			if (obj == null)
			{
				return string.Empty;
			}
			return obj.ToString();
		}

		public static string[] Arrayify(object obj)
		{
			if (obj is XPathNodeIterator)
			{
				ArrayList arrayList = new ArrayList();
				XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)obj;
				while (xPathNodeIterator.MoveNext())
				{
					arrayList.Add(Stringify(xPathNodeIterator.Current.Value));
				}
				string[] array = new string[arrayList.Count];
				arrayList.CopyTo(array, 0);
				return array;
			}
			return new string[1]
			{
				Stringify(obj)
			};
		}

		public static uint Numify(object obj)
		{
			string text = null;
			try
			{
				if (obj is string || obj is XPathNodeIterator)
				{
					text = Stringify(obj);
					return (uint)Convert.ToInt64(text);
				}
				return (uint)Convert.ToInt64(obj);
			}
			catch (Exception ex)
			{
				if (ExecutionInterface.Trace)
				{
					ExecutionInterface.LogText(string.Concat("Conversion failure on ", obj, ":", ex.Message));
				}
				return 0u;
			}
		}

		public static Guid Guidify(object obj)
		{
			string g = Stringify(obj);
			try
			{
				return new Guid(g);
			}
			catch (Exception ex)
			{
				if (ExecutionInterface.Trace)
				{
					ExecutionInterface.LogText(string.Concat("Conversion failure on ", obj, ":", ex.Message));
				}
				return Guid.Empty;
			}
		}

		public static bool Boolify(object obj)
		{
			if (obj is XPathNodeIterator)
			{
				return ((XPathNodeIterator)obj).Count != 0;
			}
			if (obj is string)
			{
				return ((string)obj).Length != 0;
			}
			if (obj is double)
			{
				if ((double)obj != 0.0)
				{
					return !double.IsNaN((double)obj);
				}
				return false;
			}
			return (bool)obj;
		}

		public static DateTime DateTimify(object obj)
		{
			string text = Stringify(obj);
			if (text.EndsWith(" (GMT)"))
			{
				text = text.Substring(0, text.Length - 6);
			}
			if (ExecutionInterface.Trace)
			{
				ExecutionInterface.LogText("converting {0} to DateTime", text);
			}
			return DateTime.Parse(text, CultureInfo.InvariantCulture);
		}

		public static TimeSpan TimeSpanify(object obj)
		{
			string text = Stringify(obj);
			if (ExecutionInterface.Trace)
			{
				ExecutionInterface.LogText("converting {0} to TimeSpan", text);
			}
			return TimeSpan.Parse(text);
		}

		private static object UpperCase(XsltContext context, object[] args, XPathNavigator nav)
		{
			return Stringify(args[0]).ToUpper(CultureInfo.InvariantCulture);
		}

		private static object LowerCase(XsltContext context, object[] args, XPathNavigator nav)
		{
			return Stringify(args[0]).ToLower(CultureInfo.InvariantCulture);
		}

		private static object Hex(XsltContext context, object[] args, XPathNavigator nav)
		{
			if (ExecutionInterface.Trace)
			{
				ExecutionInterface.LogText("Hex conversion {0} {1} {2}", args[0], Stringify(args[0]), Convert.ToInt64(Stringify(args[0]), 16));
			}
			try
			{
				return (uint)Convert.ToInt64(Stringify(args[0]), 16);
			}
			catch (Exception ex)
			{
				if (ExecutionInterface.Trace)
				{
					ExecutionInterface.LogText(string.Concat("Hex conversion failure on ", args[0], ":", ex.Message));
				}
				return 0;
			}
		}

		private static object Matches(XsltContext context, object[] args, XPathNavigator nav)
		{
			Regex regex = new Regex(Stringify(args[1]));
			return regex.Match(Stringify(args[0])).Success;
		}

		private static object Replace(XsltContext context, object[] args, XPathNavigator nav)
		{
			Regex regex = new Regex(Stringify(args[1]));
			return regex.Replace(Stringify(args[0]), Stringify(args[2]));
		}

		private static object Join(XsltContext context, object[] args, XPathNavigator nav)
		{
			string separator = Stringify(args[0]);
			ArrayList arrayList = new ArrayList();
			for (int i = 1; i < args.Length; i++)
			{
				string[] array = Arrayify(args[i]);
				string[] array2 = array;
				foreach (string value in array2)
				{
					arrayList.Add(value);
				}
			}
			return string.Join(separator, (string[])arrayList.ToArray(typeof(string)));
		}

		private static object Slice(XsltContext context, object[] args, XPathNavigator nav)
		{
			string text = Stringify(args[0]);
			char[] separator = Stringify(args[1]).ToCharArray();
			string[] array = text.Split(separator);
			int num = (int)Numify(args[2]);
			return array[num];
		}

		private static object BitwiseAnd(XsltContext context, object[] args, XPathNavigator nav)
		{
			return Numify(args[0]) & Numify(args[1]);
		}

		private static object BitwiseOr(XsltContext context, object[] args, XPathNavigator nav)
		{
			return Numify(args[0]) | Numify(args[1]);
		}

		public static object If(XsltContext context, object[] args, XPathNavigator nav)
		{
			object obj = null;
			obj = ((args.Length >= 3) ? (Boolify(args[0]) ? args[1] : args[2]) : (Boolify(args[0]) ? args[1] : ((object)false)));
			if (obj is XPathNodeIterator)
			{
				return obj;
			}
			NodeSetIterator nodeSetIterator = new NodeSetIterator();
			if (obj is bool)
			{
				if (!(bool)obj)
				{
					return nodeSetIterator.ResetableIterator;
				}
				obj = 1.0;
			}
			XmlDocument xmlDocument = (XmlDocument)ExecutionInterface.Options.Data.UnderlyingDocument;
			XmlDocumentFragment xmlDocumentFragment = xmlDocument.CreateDocumentFragment();
			XmlText xmlText = xmlDocument.CreateTextNode(Stringify(obj));
			xmlDocumentFragment.AppendChild(xmlText);
			nodeSetIterator.Add(xmlText);
			return nodeSetIterator.ResetableIterator;
		}

		private static object DateDifference(XsltContext context, object[] args, XPathNavigator nav)
		{
			DateTime d = DateTimify(args[0]);
			DateTime d2 = DateTimify(args[1]);
			return (d - d2).ToString();
		}

		private static object DateAdd(XsltContext context, object[] args, XPathNavigator nav)
		{
			DateTime d = DateTimify(args[0]);
			TimeSpan t = TimeSpanify(args[1]);
			return (d + t).ToString(CultureInfo.InvariantCulture);
		}

		private static object GetDays(XsltContext context, object[] args, XPathNavigator nav)
		{
			return TimeSpanify(args[0]).Days;
		}

		private static object GetSeconds(XsltContext context, object[] args, XPathNavigator nav)
		{
			return TimeSpanify(args[0]).TotalSeconds;
		}

		private static object DateParse(XsltContext context, object[] args, XPathNavigator nav)
		{
			return DateTime.ParseExact(Stringify(args[0]), Stringify(args[1]), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
		}

		private static object GetTimeSpan(XsltContext context, object[] args, XPathNavigator nav)
		{
			long ticks = (long)Numberify(args[0]) * 10000000;
			return new TimeSpan(ticks).ToString();
		}

		private static object VersionCompare(XsltContext context, object[] args, XPathNavigator nav)
		{
			Version version = new Version(Stringify(args[0]));
			Version value = new Version(Stringify(args[1]));
			return version.CompareTo(value);
		}

		public ExtFunction(string name, int minargs, int maxargs, XPathResultType[] argTypes, XPathResultType returnType, ExtDelegate function)
		{
			this.name = name;
			this.minargs = minargs;
			this.maxargs = maxargs;
			this.argTypes = argTypes;
			this.returnType = returnType;
			this.function = function;
		}

		public object Invoke(XsltContext context, object[] args, XPathNavigator nav)
		{
			if (ExecutionInterface.Trace)
			{
				ExecutionInterface.LogText("Invoking {0}: arg[0] type: {1}", name, args[0].GetType());
			}
			return function(context, args, nav);
		}

		public static object Evaluate(IXPathNavigable node, string expression)
		{
			XPathNavigator xPathNavigator = node.CreateNavigator();
			CustomContext context = new CustomContext();
			XPathExpression xPathExpression = xPathNavigator.Compile(expression);
			xPathExpression.SetContext(context);
			return xPathNavigator.Evaluate(xPathExpression);
		}
	}
}
