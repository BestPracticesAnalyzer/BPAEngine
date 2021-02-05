using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	public class Ntdsapi
	{
		[DllImport("Ntdsapi.dll", CharSet = CharSet.Unicode)]
		public static extern int DsMakeSpn(string serviceClass, string serviceName, string instanceName, ushort instancePort, string referrer, ref int cbSpnLength, StringBuilder spnName);
	}
}
