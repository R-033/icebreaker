using System.Runtime.InteropServices;


namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct HeliSection // 0x00034159
	{
		public long Padding;
		public int SectionNumber;
		public int NumPolies;
		public int EndianSwapped;
		public int Pointer;

		public const int SizeOf = 0x18;
	}
}
