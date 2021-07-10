using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CompressionSlot
	{
		public long Padding1;
		public int Value1;
		public int Value2;
		public int Value3;
		public uint CompType;
		public long Padding2;

		public const int SizeOf = 0x20;
	}
}
