using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackSpecificSection // 0x00034154
	{
		public long Padding;
		public int TrackID;
		public int NumSections;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
		public ushort[] SectionIndeces;

		public const int SizeOf = 0x20;

		public static TrackSpecificSection CreateNew()
		{
			return new TrackSpecificSection() { SectionIndeces = new ushort[8] };
		}
	}
}
