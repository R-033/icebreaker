using System.Runtime.InteropServices;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WeathermanHeader // 0x00034250
	{
		public long Padding;
		public int Version;
		public int NumRegions;

		public const int SizeOf = 0x10;
	}
}
