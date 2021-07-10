using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TopologyIndexation // 0x00034136
	{
		public ushort CoordinateIndex;
		public ushort DebugFlag;

		public TopologyIndexation(int index)
		{
			CoordinateIndex = (ushort)index;
			DebugFlag = 0xFFFF;
		}

		public const int SizeOf = 0x04;
	}
}
