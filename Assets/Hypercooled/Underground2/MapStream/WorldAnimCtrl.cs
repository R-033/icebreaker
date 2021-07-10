using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WorldAnimCtrl // 0x00037150
	{
		public int UniqueID;
		public uint AnimTreeKey;
		public int SectionNumber;
		public int Padding;

		public const int SizeOf = 0x10;
	}
}
