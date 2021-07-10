using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VisibleSectionManageInfo // 0x00034151
	{
		public int LODOffset;
		public int NumDrivableSections;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x190)]
		public ushort[] DrivableSections;

		public const int SizeOf = 0x328;

		public static VisibleSectionManageInfo CreateNew()
		{
			return new VisibleSectionManageInfo() { DrivableSections = new ushort[0x190] };
		}
	}
}
