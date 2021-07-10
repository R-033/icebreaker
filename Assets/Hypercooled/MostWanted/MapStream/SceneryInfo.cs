using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryInfo // 0x00034102
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x18)]
		public string Name;

		public uint SolidMeshKey1;
		public uint SolidMeshKey2;
		public uint SolidMeshKey3;
		public uint SolidMeshKey4;
		public long Padding1;
		public long Padding2;
		public float Radius;
		public uint MeshChecksum;
		public uint HierarchyKey;
		public int Padding;

		public const int SizeOf = 0x48;
	}
}
