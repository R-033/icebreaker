using System.Runtime.InteropServices;

namespace Hypercooled.Shared.World
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UppleUHeader // 0x0003B801 & 0x0003B802
	{
		public int Size;
		public int SectionNumber;
		public int EndianSwapped;
		public int Padding;

		public const int SizeOf = 0x10;
	}
}
