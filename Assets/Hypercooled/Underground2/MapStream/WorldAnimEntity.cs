using System.Numerics;
using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
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
		public int Padding1;
		public int Padding2;
		public uint AnimTreeKey;
		public uint AnimNameKey;
		public byte AnimContextFlags; // mostly 3 or 7
		public byte ParentIndex;
		public short Padding3;
		public int Padding4;

		public Matrix4x4 Transform;

		public const int SizeOf = 0x70;
	}
}
