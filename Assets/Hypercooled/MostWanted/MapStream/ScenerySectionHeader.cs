using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ScenerySectionHeader // 0x00034101
	{
		public long Pointer1;
		public int Pointer2;
		public int SectionNumber;
		public int Pointer3;
		public long Pointer4;
		public long Pointer5;
		public long Pointer6;
		public long Pointer7;
		public long Pointer8;

		public const int SizeOf = 0x3C;
	}
}
