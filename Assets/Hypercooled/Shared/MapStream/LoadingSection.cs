using System.Runtime.InteropServices;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LoadingSection // 0x00034155
	{
		public long Padding;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x0F)]
		public string Name;

		public byte DefaultFlag;
		public short NumDrivableSections;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
		public ushort[] DrivableSections;

		public short NumExtraSections;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x08)]
		public ushort[] ExtraSections;

		public const int SizeOf = 0x4C;
	}
}
