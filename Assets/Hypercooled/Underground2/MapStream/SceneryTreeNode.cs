using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryTreeNode // 0x00034104
	{
		public Vector3 BBoxMin;
		public Vector3 BBoxMax;
		public short NumChildren;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x05)]
		public ushort[] ChildCodes;

		public const int SizeOf = 0x24;

		public static SceneryTreeNode CreateNew()
		{
			return new SceneryTreeNode() { ChildCodes = new ushort[5] };
		}
	}
}
