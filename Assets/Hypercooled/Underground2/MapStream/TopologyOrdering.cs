using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TopologyOrdering // 0x00034137
	{
		public ushort StartIndex;
		public ushort Count;

		public TopologyOrdering(int start, int count)
		{
			StartIndex = (ushort)start;
			Count = (ushort)count;
		}

		public const int SizeOf = 0x04;
	}
}
