using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VisibleSectionManageInfo // 0x00034151
	{
		public int LODOffset;
		public int NumDrivableSections;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x200)]
		public ushort[] DrivableSections;

		public const int SizeOf = 0x408;

		public static VisibleSectionManageInfo CreateNew()
		{
			return new VisibleSectionManageInfo() { DrivableSections = new ushort[0x200] };
		}
	}
}
