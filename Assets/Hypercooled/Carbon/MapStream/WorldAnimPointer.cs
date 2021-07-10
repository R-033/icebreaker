using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WorldAnimPointer // 0x00037270
	{
		public int Pointer; // 0x69696969

		public const int SizeOf = 0x04;
	}
}
