using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryInfo // 0x00034102
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
		public string Name;

		public uint SolidMeshKey1;
		public uint SolidMeshKey2;
		public uint SolidMeshKey3;
		public ushort SomeFlag1;
		public ushort SomeFlag2;
		public int Pointer1;
		public int Pointer2;
		public int Pointer3;
		public float Radius;
		public uint MeshChecksum;

		public const int SizeOf = 0x44;
	}
}
