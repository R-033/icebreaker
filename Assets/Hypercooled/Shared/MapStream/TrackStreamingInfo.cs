using System.Runtime.InteropServices;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackStreamingInfo // 0x00034111
	{
		public int FilePointer1;
		public int FilePointer2;

		public const int SizeOf = 0x08;
	}
}
