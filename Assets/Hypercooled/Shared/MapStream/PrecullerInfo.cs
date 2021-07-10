using System.Runtime.InteropServices;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct PrecullerInfo // 0x00034106 (U2) | 0x00034107 (MW & C)
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x80)]
		public byte[] VisibilityBits;

		public const int SizeOf = 0x80;
	}
}
