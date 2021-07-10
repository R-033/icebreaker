using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CVHeader
	{
		public uint Key;
		public int NumBounds;
		public int IsResolved;
		public int Padding;

		public const int SizeOf = 0x10;
	}
}
