using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CompressionSlot
	{
		public int Padding1;
		public long Padding2;
		public uint CompType;
		public long Padding3;

		public const int SizeOf = 0x18;
	}
}
