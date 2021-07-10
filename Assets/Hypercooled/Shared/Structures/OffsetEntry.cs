using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Structures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct OffsetEntry
	{
		public uint Key;
		public uint AbsoluteOffset;
		public int EncodedSize;
		public int DecodedSize;
		public byte UserFlags;
		public byte CompressionFlags;
		public short ReferenceCount;
		public int Pointer;

		public const int SizeOf = 0x18;
	}
}
