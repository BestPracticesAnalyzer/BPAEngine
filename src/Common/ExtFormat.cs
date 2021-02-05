using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class ExtFormat
	{
		public delegate string ConvertMethod(object val);

		public delegate void DisplayMethod(TreeNodeInfo node, string val);

		public static ExecutionInterface ExecutionInterface;

		private static Hashtable convertHash;

		private static Hashtable displayHash;

		static ExtFormat()
		{
			Reset();
		}

		public static void Reset()
		{
			convertHash = new Hashtable();
			displayHash = new Hashtable();
			Add("SecurityDescriptor", ConvertBinaryToString, NTSD.Display);
			Add("Sid", NTSD.ConvertToStringSid, null);
		}

		internal static void Add(string name, Node node, LoadedProcessor loadedProcessor)
		{
			string attribute = node.GetAttribute("Convert");
			ConvertMethod convertMethod = null;
			if (attribute != null && attribute.Length > 0)
			{
				convertMethod = (ConvertMethod)Delegate.CreateDelegate(typeof(ConvertMethod), loadedProcessor.ProcessorType, attribute);
			}
			string attribute2 = node.GetAttribute("Display");
			DisplayMethod displayMethod = null;
			if (attribute2 != null && attribute2.Length > 0)
			{
				displayMethod = (DisplayMethod)Delegate.CreateDelegate(typeof(DisplayMethod), loadedProcessor.ProcessorType, attribute2);
			}
			Add(name, convertMethod, displayMethod);
		}

		public static void Add(string name, ConvertMethod convertMethod, DisplayMethod displayMethod)
		{
			if (convertMethod != null)
			{
				convertHash[name] = convertMethod;
			}
			if (displayMethod != null)
			{
				displayHash[name] = displayMethod;
			}
		}

		public static byte[] ByteArrayFromString(string valString)
		{
			byte[] array = new byte[valString.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToByte(valString.Substring(2 * i, 2), 16);
			}
			return array;
		}

		public static void DisplayValue(TreeNodeInfo node, string val, string format)
		{
			try
			{
				if (displayHash.Contains(format))
				{
					((DisplayMethod)displayHash[format])(node, val);
				}
				else
				{
					node.Text += val;
				}
			}
			catch
			{
				node.Text = node.Text + "<" + format + " (error)>";
			}
		}

		public static string ConvertBinaryToString(object val)
		{
			Type type = val.GetType();
			string text = "";
			StringBuilder stringBuilder = new StringBuilder();
			if (type == typeof(string))
			{
				val = ((string)val).ToCharArray();
				text = "";
				char[] array = (char[])val;
				foreach (char c in array)
				{
					string str = text;
					int num = c;
					text = str + num.ToString("X4");
				}
			}
			else if (type == typeof(byte[]))
			{
				stringBuilder.Capacity = 2 * ((byte[])val).Length + 1;
				byte[] array2 = (byte[])val;
				foreach (byte b in array2)
				{
					stringBuilder.Append(b.ToString("X2"));
				}
				text = stringBuilder.ToString();
			}
			return text;
		}

		public static string ConvertValueToString(object val, string format)
		{
			if (val == null)
			{
				return string.Empty;
			}
			string text = "";
			try
			{
				if (convertHash.Contains(format))
				{
					return ((ConvertMethod)convertHash[format])(val);
				}
				Type type = val.GetType();
				IFormattable formattable = val as IFormattable;
				text = ((formattable == null) ? val.ToString() : formattable.ToString(null, CultureInfo.InvariantCulture));
				switch (format)
				{
				case "System.Guid":
					if (type == typeof(byte[]))
					{
						return new Guid((byte[])val).ToString();
					}
					return text;
				case "System.Byte[]":
					return ConvertBinaryToString(val);
				case "System.DateTime.Local":
					if (val is DateTime)
					{
						return ((DateTime)val).ToUniversalTime().ToString(CultureInfo.InvariantCulture) + " (GMT)";
					}
					return text;
				case "System.DateTime":
					if (val is DateTime)
					{
						return ((DateTime)val).ToString(CultureInfo.InvariantCulture) + " (GMT)";
					}
					return text;
				case "":
				case "XML":
					if (type == typeof(byte[]))
					{
						text = "";
						byte[] array = (byte[])val;
						foreach (byte b in array)
						{
							text += b.ToString("X2");
						}
						return text;
					}
					if (val is XmlElement)
					{
						return ((XmlElement)val).OuterXml;
					}
					if (val is XmlAttribute)
					{
						return ((XmlAttribute)val).InnerXml;
					}
					return text;
				default:
				{
					int num = format.ToString().IndexOf("----");
					if (num != -1)
					{
						string text2 = "";
						string replacement = "";
						text2 = format.ToString().Substring(0, num);
						if (format.ToString().Length > num + 4)
						{
							replacement = format.ToString().Substring(num + 4);
						}
						return Regex.Replace(val.ToString(), text2, replacement, RegexOptions.Singleline);
					}
					return val.ToString();
				}
				}
			}
			catch (Exception ex)
			{
				ExecutionInterface.LogException(CommonLoc.Error_DataConversion(ex.Message, format.ToString()), ex);
				return val.ToString();
			}
		}

		public static string ConvertToIntegerString(object val)
		{
			string empty = string.Empty;
			try
			{
				return ((int)val).ToString();
			}
			catch (InvalidCastException)
			{
				return val.ToString();
			}
		}

		public static void AddValueToNode(object val, string format, Node valNode)
		{
			if (format == "XML" && val is XmlElement)
			{
				BPANode srcNode = new BPANode(valNode.OwnerDocument, (XmlElement)val);
				valNode.Add(valNode.OwnerDocument.ImportNode(srcNode, true));
				return;
			}
			string text = ConvertValueToString(val, format);
			if (Common.IsValidXmlString(text))
			{
				valNode.Value = text;
			}
		}
	}
}
