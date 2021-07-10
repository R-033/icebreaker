using System.Runtime.InteropServices;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct HeliPolygon // 0x00034159
	{
		public short X1;
		public short X2;
		public short X3;

		public short Y1;
		public short Y2;
		public short Y3;

		public short Z1;
		public short Z2;
		public short Z3;

		public const int SizeOf = 0x12;
	}
}
