using System.Numerics;
using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WorldAnimTreeMarker // 0x00037110
	{
		public uint Key;
		public int Padding1;
		public int Padding2;
		public int Padding3;
		public Matrix4x4 Padding;

		public const int SizeOf = 0x50;
	}
}
