using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WorldAnimHeader // 0x00037220
	{
		public int NumAnimBanks;
		public int NumInstanceNodes;
		public int SectionNumber;
		public int NumTrees;

		public const int SizeOf = 0x10;
	}
}
