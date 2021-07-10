using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryTreeNode // 0x00034105
	{
		public Vector3 BBoxMin;
		public ushort NumChildren;
		public ushort Padding;
		public Vector3 BBoxMax;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0A)]
		public ushort[] ChildCodes;

		public const int SizeOf = 0x30;

		public static SceneryTreeNode CreateNew()
		{
			return new SceneryTreeNode() { ChildCodes = new ushort[10] };
		}
	}
}
