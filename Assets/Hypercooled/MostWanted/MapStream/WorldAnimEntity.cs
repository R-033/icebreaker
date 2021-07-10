using System.Numerics;
using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WorldAnimEntity // 0x00037080
	{
		public int TypeID;
		public uint Key;
		public uint ParentKey;
		public uint ModelKey;
		public uint LodBKey;
		public uint LodZKey;
		public int ParentEntityInfo;
		public int Padding;
		public uint AnimTreeKey;
		public uint AnimNameKey;
		public int AnimContextFlags;
		public int ParentIndex;

		public Matrix4x4 Transform;

		public const int SizeOf = 0x70;
	}
}
