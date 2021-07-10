using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WorldAnimNodeInfo // 0x00037260
	{
		public int NumNodes;

		public const int SizeOf = 0x04;
	}
}
