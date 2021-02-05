using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Microsoft.VSPowerToys.BestPracticesAnalyzer.Common
{
	[CLSCompliant(false)]
	public class NTSD : CollectionBase
	{
		public struct Win32ACL
		{
			public byte AclRevision;

			public byte Sbz1;

			public ushort AclSize;

			public ushort AceCount;

			public ushort Sbz2;
		}

		public struct Win32ACE
		{
			public byte AceType;

			public byte AceFlags;

			public ushort AceSize;

			public uint Mask;
		}

		public struct Win32ObjectACE
		{
			public byte AceType;

			public byte AceFlags;

			public ushort AceSize;

			public uint Mask;

			public uint Flags;
		}

		public enum ACEType : byte
		{
			AccessAllowedAce = 0,
			AccessDeniedAce = 1,
			SystemAuditAce = 2,
			SystemAlarmAce = 3,
			AccessAllowedCompoundAce = 4,
			AccessAllowedObjectAce = 5,
			AccessDeniedObjectAce = 6,
			SystemAuditObjectAce = 7,
			SystemAlarmObjectAce = 8,
			AccessAllowedCallbackAce = 9,
			AccessDeniedCallbackAce = 10,
			AccessAllowedCallbackObjectAce = 11,
			AccessDeniedCallbackObjectAce = 12,
			SystemAuditCallbackAce = 13,
			SystemAlarmCallbackAce = 14,
			SystemAuditCallbackObjectAce = 0xF,
			SystemAlarmCallbackObjectAce = 0x10,
			Unknown = byte.MaxValue
		}

		[Flags]
		public enum ACEFlags : byte
		{
			SubitemInheritable = 0x1,
			SubcontainerInheritable = 0x2,
			NopropagateInherit = 0x4,
			InheritOnly = 0x8,
			Inherited = 0x10,
			Reserved = 0x20,
			SuccessfulAccess = 0x40,
			FailedAccess = 0x80
		}

		[Flags]
		public enum ACEMask : uint
		{
			Delete = 0x10000u,
			ReadControl = 0x20000u,
			WriteDac = 0x40000u,
			WriteOwner = 0x80000u,
			Synchronize = 0x100000u,
			Reserved21 = 0x200000u,
			Reserved22 = 0x400000u,
			Reserved23 = 0x800000u,
			AccessSystemSecurity = 0x1000000u,
			Reserved25 = 0x2000000u,
			Reserved26 = 0x4000000u,
			Reserved27 = 0x8000000u,
			GenericAll = 0x10000000u,
			GenericExecute = 0x20000000u,
			GenericWrite = 0x40000000u,
			GenericRead = 0x80000000u
		}

		public class ObjectGuidHash
		{
			private static Hashtable hash;

			static ObjectGuidHash()
			{
				hash = new Hashtable();
				hash.Add(new Guid("ab721a54-1e2f-11d0-9819-00aa0040529b"), "Send As");
				hash.Add(new Guid("ab721a56-1e2f-11d0-9819-00aa0040529b"), "Receive As");
				hash.Add(new Guid("cf0b3dc8-afe6-11d2-aa04-00c04f8eedd8"), "Create Public Folder");
				hash.Add(new Guid("d74a8766-22b9-11d3-aa62-00c04f8eedd8"), "Create Named Properties");
				hash.Add(new Guid("e48d0154-bcf8-11d1-8702-00c04fb96050"), "Public Information");
				hash.Add(new Guid("77b5b886-944a-11d1-aebd-0000f80367c1"), "Personal Information");
				hash.Add(new Guid("36145cf4-a982-11d2-a9ff-00c04f8eedd8"), "msExchPrivateMDB");
				hash.Add(new Guid("3568b3a4-a982-11d2-a9ff-00c04f8eedd8"), "msExchPublicMDB");
				hash.Add(new Guid("a8df74d9-c5ea-11d1-bbcb-0080c76670c0"), "siteAddressing");
				hash.Add(new Guid("cf4b9d46-afe6-11d2-aa04-00c04f8eedd8"), "Create Top Level Public Folder");
				hash.Add(new Guid("be013017-13a1-41ad-a058-f156504cb617"), "Read metabase properties");
				hash.Add(new Guid("d74a8762-22b9-11d3-aa62-00c04f8eedd8"), "Administer Information Store");
				hash.Add(new Guid("d74a875e-22b9-11d3-aa62-00c04f8eedd8"), "View Information Store Status");
				hash.Add(new Guid("a8df74a7-c5ea-11d1-bbcb-0080c76670c0"), "MTA");
			}

			public static string StringGuid(Guid guid)
			{
				string text = guid.ToString();
				if (hash.Contains(guid))
				{
					object obj = text;
					text = string.Concat(obj, " (", hash[guid], ")");
				}
				return text;
			}
		}

		public class ACE
		{
			public ACEType Type;

			public ACEFlags Flags;

			public ushort AceSize;

			public uint Mask;

			public uint ObjectFlags;

			public Guid ObjectType;

			public Guid InheritedObjectType;

			public string Sid;

			public ACE()
			{
			}

			public override bool Equals(object o)
			{
				ACE aCE = (ACE)o;
				if (Sid == aCE.Sid && Type == aCE.Type && Flags == aCE.Flags && Mask == aCE.Mask && ObjectFlags == aCE.ObjectFlags && ObjectType == aCE.ObjectType)
				{
					return InheritedObjectType == aCE.InheritedObjectType;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (int)Type | (int)((uint)Flags << 8) | (int)Mask;
			}

			public ACE(IntPtr acePtr)
			{
				Win32ACE win32ACE = (Win32ACE)Marshal.PtrToStructure(acePtr, typeof(Win32ACE));
				Type = (ACEType)win32ACE.AceType;
				Flags = (ACEFlags)win32ACE.AceFlags;
				AceSize = win32ACE.AceSize;
				Mask = win32ACE.Mask;
				IntPtr sidPtr = IntPtr.Zero;
				int num = Marshal.SizeOf(typeof(Win32ACE));
				switch (win32ACE.AceType)
				{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 9:
				case 10:
				case 13:
				case 14:
					sidPtr = (IntPtr)((long)acePtr + num);
					break;
				case 5:
				case 6:
				case 7:
				case 8:
				case 11:
				case 12:
				case 15:
				case 16:
				{
					Win32ObjectACE win32ObjectACE = (Win32ObjectACE)Marshal.PtrToStructure(acePtr, typeof(Win32ObjectACE));
					ObjectFlags = win32ObjectACE.Flags;
					num += 4;
					if ((win32ObjectACE.Flags & (true ? 1u : 0u)) != 0)
					{
						ObjectType = (Guid)Marshal.PtrToStructure((IntPtr)((long)acePtr + num), typeof(Guid));
						num += Marshal.SizeOf(typeof(Guid));
					}
					if ((win32ObjectACE.Flags & 2u) != 0)
					{
						InheritedObjectType = (Guid)Marshal.PtrToStructure((IntPtr)((long)acePtr + num), typeof(Guid));
						num += Marshal.SizeOf(typeof(Guid));
					}
					sidPtr = (IntPtr)((long)acePtr + num);
					break;
				}
				}
				Sid = ConvertToStringSid(sidPtr);
			}
		}

		private int revision;

		private Advapi32.SECURITY_DESCRIPTOR_CONTROL control;

		private string owner;

		private bool ownerDefaulted;

		private string group;

		private bool groupDefaulted;

		private int daclRevision;

		private bool daclDefaulted;

		private bool daclProtected;

		private bool daclAutoInherited;

		private ExecutionInterface executionInterface;

		private Hashtable sidNameHash;

		private static SortedList SidFromNameCache = new SortedList();

		public ACE this[int index]
		{
			get
			{
				return (ACE)base.List[index];
			}
			set
			{
				base.List[index] = value;
			}
		}

		public Advapi32.SECURITY_DESCRIPTOR_CONTROL Control
		{
			get
			{
				return control;
			}
		}

		public bool DACLAutoInherited
		{
			get
			{
				return daclAutoInherited;
			}
			set
			{
				daclAutoInherited = value;
			}
		}

		public bool DACLDefaulted
		{
			get
			{
				return daclDefaulted;
			}
			set
			{
				daclDefaulted = value;
			}
		}

		public bool DACLProtected
		{
			get
			{
				return daclProtected;
			}
			set
			{
				daclProtected = value;
			}
		}

		public int DACLRevision
		{
			get
			{
				return daclRevision;
			}
			set
			{
				daclRevision = value;
			}
		}

		public string Group
		{
			get
			{
				return group;
			}
			set
			{
				group = value;
			}
		}

		public bool GroupDefaulted
		{
			get
			{
				return ownerDefaulted;
			}
			set
			{
				ownerDefaulted = value;
			}
		}

		public string Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
			}
		}

		public bool OwnerDefaulted
		{
			get
			{
				return ownerDefaulted;
			}
			set
			{
				ownerDefaulted = value;
			}
		}

		public int Revision
		{
			get
			{
				return revision;
			}
			set
			{
				revision = value;
			}
		}

		public NTSD(ExecutionInterface executionInterface, byte[] binarySD)
		{
			this.executionInterface = executionInterface;
			if (executionInterface.Parameters.Hash.Contains("Sid"))
			{
				sidNameHash = (Hashtable)executionInterface.Parameters.Hash["Sid"];
			}
			else
			{
				sidNameHash = new Hashtable();
			}
			GCHandle gCHandle = GCHandle.Alloc(binarySD, GCHandleType.Pinned);
			IntPtr pSecurityDescriptor = gCHandle.AddrOfPinnedObject();
			try
			{
				Advapi32.GetSecurityDescriptorControl(pSecurityDescriptor, out control, out revision);
				IntPtr pOwner = IntPtr.Zero;
				Advapi32.GetSecurityDescriptorOwner(pSecurityDescriptor, out pOwner, out ownerDefaulted);
				owner = ConvertToStringSid(pOwner);
				IntPtr pGroup = IntPtr.Zero;
				Advapi32.GetSecurityDescriptorGroup(pSecurityDescriptor, out pGroup, out groupDefaulted);
				group = ConvertToStringSid(pGroup);
				IntPtr pDacl = IntPtr.Zero;
				bool lpbDaclPresent = false;
				Advapi32.GetSecurityDescriptorDacl(pSecurityDescriptor, out lpbDaclPresent, ref pDacl, out daclDefaulted);
				Win32ACL win32ACL = (Win32ACL)Marshal.PtrToStructure(pDacl, typeof(Win32ACL));
				daclRevision = win32ACL.AclRevision;
				IntPtr intPtr = (IntPtr)((long)pDacl + Marshal.SizeOf(typeof(Win32ACL)));
				for (int i = 0; i < win32ACL.AceCount; i++)
				{
					ACE aCE = new ACE(intPtr);
					base.List.Add(aCE);
					intPtr = (IntPtr)((long)intPtr + aCE.AceSize);
				}
			}
			finally
			{
				gCHandle.Free();
			}
		}

		public static string ConvertToStringSid(object val)
		{
			if (val.GetType() == typeof(byte[]))
			{
				return ConvertToStringSid((byte[])val);
			}
			return val.ToString();
		}

		public static string ConvertToStringSid(byte[] sid)
		{
			GCHandle gCHandle = GCHandle.Alloc(sid, GCHandleType.Pinned);
			IntPtr sid2 = gCHandle.AddrOfPinnedObject();
			string pStringSid = null;
			try
			{
				Advapi32.ConvertSidToStringSid(sid2, ref pStringSid);
				return pStringSid;
			}
			finally
			{
				gCHandle.Free();
			}
		}

		public static string ConvertToStringSid(IntPtr sidPtr)
		{
			string pStringSid = null;
			if (sidPtr != IntPtr.Zero)
			{
				Advapi32.ConvertSidToStringSid(sidPtr, ref pStringSid);
			}
			return pStringSid;
		}

		public static string GetSidFromName(ExecutionInterface executionInterface, string dcName, string accountName)
		{
			string text = null;
			lock (SidFromNameCache)
			{
				if (SidFromNameCache.Contains(accountName))
				{
					return (string)SidFromNameCache[accountName];
				}
				IntPtr intPtr = IntPtr.Zero;
				try
				{
					int cbSid = 0;
					int cbDomainName = 0;
					int peUse = 0;
					if (executionInterface.Trace)
					{
						executionInterface.LogText("Looking up account name {0}.", accountName);
					}
					Advapi32.LookupAccountName(dcName, accountName, IntPtr.Zero, ref cbSid, null, ref cbDomainName, out peUse);
					StringBuilder domainName = new StringBuilder(cbDomainName);
					intPtr = Marshal.AllocHGlobal(cbSid);
					Advapi32.LookupAccountName(dcName, accountName, intPtr, ref cbSid, domainName, ref cbDomainName, out peUse);
					if (executionInterface.Trace)
					{
						executionInterface.LogText("Lookup account name completed.");
					}
					text = ConvertToStringSid(intPtr);
					SidFromNameCache.Add(accountName, text);
					return text;
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(intPtr);
					}
				}
			}
		}

		public string DisplaySid(string sid)
		{
			if (sidNameHash.Contains(sid))
			{
				sid = sid + " (" + sidNameHash[sid].ToString() + ")";
			}
			return sid;
		}

		public static void Display(TreeNodeInfo node, string val)
		{
			byte[] binarySD = ExtFormat.ByteArrayFromString(val);
			NTSD nTSD = new NTSD(ExtFormat.ExecutionInterface, binarySD);
			nTSD.Display(node);
			node.Text += "<SecurityDescriptor>";
		}

		public void Display(TreeNodeInfo parentNode)
		{
			parentNode.Add("Revision: " + revision);
			parentNode.Add("Control: " + control);
			parentNode.Add("Owner: " + DisplaySid(owner));
			parentNode.Add("Group: " + DisplaySid(group));
			TreeNodeInfo treeNodeInfo = parentNode.Add("DACL");
			for (int i = 0; i < base.Count; i++)
			{
				ACE aCE = this[i];
				TreeNodeInfo treeNodeInfo2 = treeNodeInfo.Add("Ace[" + i + "]");
				treeNodeInfo2.Add("type: " + aCE.Type);
				treeNodeInfo2.Add("flags: " + aCE.Flags);
				string text = string.Format("{0,8:X8}", aCE.Mask);
				if ((aCE.Mask & 0xFFFF0000u) != 0)
				{
					text = text + " (" + ((ACEMask)(aCE.Mask & 0xFFFF0000u)).ToString() + ")";
				}
				treeNodeInfo2.Add("accessMask: " + text);
				treeNodeInfo2.Add("Sid: " + DisplaySid(aCE.Sid));
				if (aCE.ObjectFlags != 0)
				{
					treeNodeInfo2.Add("ObjectFlags: " + aCE.ObjectFlags);
				}
				if (aCE.ObjectType != Guid.Empty)
				{
					treeNodeInfo2.Add("ObjectType: " + ObjectGuidHash.StringGuid(aCE.ObjectType));
				}
				if (aCE.InheritedObjectType != Guid.Empty)
				{
					treeNodeInfo2.Add("InheritedObjectType: " + ObjectGuidHash.StringGuid(aCE.InheritedObjectType));
				}
			}
		}

		public static object Sdcheck(XsltContext context, object[] args, XPathNavigator nav)
		{
			ACE aCE = new ACE();
			int num = 0;
			string valString = ExtFunction.Stringify(args[0]);
			string[] array = ExtFunction.Arrayify(args[1]);
			aCE.Type = (ACEType)ExtFunction.Numify(args[2]);
			aCE.Flags = (ACEFlags)ExtFunction.Numify(args[3]);
			aCE.Mask = ExtFunction.Numify(args[4]);
			if (args.Length > 5)
			{
				aCE.ObjectFlags = ExtFunction.Numify(args[5]);
			}
			if (args.Length > 6)
			{
				aCE.ObjectType = ExtFunction.Guidify(args[6]);
			}
			if (args.Length > 7)
			{
				aCE.InheritedObjectType = ExtFunction.Guidify(args[7]);
			}
			byte[] binarySD = ExtFormat.ByteArrayFromString(valString);
			NTSD nTSD = new NTSD(ExtFunction.ExecutionInterface, binarySD);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = (aCE.Sid = array2[i]);
				if (ExtFunction.ExecutionInterface.Trace)
				{
					ExtFunction.ExecutionInterface.LogText("SD check: sid={0} mask=0x{1:X}", aCE.Sid, aCE.Mask);
				}
				foreach (ACE item in nTSD)
				{
					if (item.Equals(aCE))
					{
						num++;
						break;
					}
				}
			}
			return num;
		}

		public static object SdGet(XsltContext context, object[] args, XPathNavigator nav)
		{
			string text = ExtFunction.Stringify(args[0]);
			string valString = ExtFunction.Stringify(args[1]);
			byte[] binarySD = ExtFormat.ByteArrayFromString(valString);
			NTSD nTSD = new NTSD(ExtFunction.ExecutionInterface, binarySD);
			object result = null;
			switch (text)
			{
			case "Control":
				result = (int)nTSD.Control;
				break;
			case "Owner":
				result = nTSD.Owner;
				break;
			case "Group":
				result = nTSD.Group;
				break;
			}
			return result;
		}
	}
}
