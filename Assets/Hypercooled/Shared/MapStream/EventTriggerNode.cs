using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct EventTriggerTree // 0x00036002
	{
		public int NodeArrayPointer;
		public short NumLeafNodes;
		public short NumParentNodes;
		public short TotalNodes;
		public short Depth;
		public int Padding;

		public const int SizeOf = 0x10;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct EventTriggerNode // 0x00036002
	{
		public Vector3 Position;
		public short ParentIndex;
		public short NumChildren;
		public Vector3 Extent;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0A)]
		public short[] ChildCodes;

		public const int SizeOf = 0x30;
	}
}
