using System;
using System.Runtime.InteropServices;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	[CLSCompliant(false)]
	public class DnsApi
	{
		public enum DnsFreeType
		{
			DnsFreeFlat,
			DnsFreeRecordList,
			DnsFreeParsedMessageFields
		}

		public enum DnsStatus
		{
			Success
		}

		public enum DnsRecordType : ushort
		{
			A = 1,
			NS = 2,
			CNAME = 5,
			SOA = 6,
			PTR = 12,
			MX = 0xF,
			TXT = 0x10,
			AAAA = 28,
			SRV = 33
		}

		[Flags]
		public enum DnsQueryOptions
		{
			Standard = 0x0,
			AcceptTruncatedResponse = 0x1,
			UseTcpOnly = 0x2,
			NoRecursion = 0x4,
			ByPassCache = 0x8,
			TreatAsFqdn = 0x1000
		}

		public enum DNSErrorCodes
		{
			DNS_ERROR_RESPONSE_CODES_BASE = 9000,
			DNS_ERROR_RCODE_FORMAT_ERROR,
			DNS_ERROR_RCODE_SERVER_FAILURE,
			DNS_ERROR_RCODE_NAME_ERROR
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_MX_DATA
		{
			public string pNameExchange;

			private ushort wPreference;

			private ushort Pad;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_SRV_DATA
		{
			public string pNameTarget;

			private ushort wPriority;

			private ushort wWeight;

			public ushort wPort;

			private ushort Pad;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_BASE
		{
			public readonly IntPtr pNext;

			public string pName;

			public ushort wType;

			public ushort wDataLength;

			public uint Flags;

			public uint dwTtl;

			public uint dwReserved;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_A
		{
			public readonly IntPtr pNext;

			public string pName;

			public ushort wType;

			public ushort wDataLength;

			public uint Flags;

			public uint dwTtl;

			public uint dwReserved;

			public uint A;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_PTR
		{
			public readonly IntPtr pNext;

			public string pName;

			public ushort wType;

			public ushort wDataLength;

			public uint Flags;

			public uint dwTtl;

			public uint dwReserved;

			public string PTR;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_MX
		{
			public readonly IntPtr pNext;

			public string pName;

			public ushort wType;

			public ushort wDataLength;

			public uint Flags;

			public uint dwTtl;

			public uint dwReserved;

			public DNS_MX_DATA MX;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct DNS_RECORD_SRV
		{
			public readonly IntPtr pNext;

			public string pName;

			public ushort wType;

			public ushort wDataLength;

			public uint Flags;

			public uint dwTtl;

			public uint dwReserved;

			public DNS_SRV_DATA SRV;
		}

		[DllImport("dnsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DnsQuery_W")]
		public static extern int DnsQuery(string name, DnsRecordType type, DnsQueryOptions options, IntPtr dnsServers, ref IntPtr QueryResultSet, IntPtr reserved);

		[DllImport("dnsapi.dll", CharSet = CharSet.Unicode)]
		public static extern void DnsRecordListFree(IntPtr ptrRecords, DnsFreeType freeType);
	}
}
