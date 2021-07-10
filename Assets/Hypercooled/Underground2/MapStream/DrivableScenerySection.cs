using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DrivableScenerySection // 0x00034153
	{
		public long Padding;
		public ushort SectionNumber;
		public byte MostVisibleSections;
		public byte MaxVisibleSections;
		public int Pointer;
		public int NumVisibleSections;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x48)]
		public ushort[] VisibleSections;

		public const int SizeOf = 0xA4;

		public static DrivableScenerySection CreateNew()
		{
			return new DrivableScenerySection() { VisibleSections = new ushort[0x48] };
		}
	}
}
