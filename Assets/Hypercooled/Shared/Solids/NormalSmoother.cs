using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct NormalSmoother // 0x00134017
	{
		public int SmoothVertexTablePtr;
		public int SmoothVertexPlatTablePtr;
		public short NumSmoothVertex;
		public short NumSmoothVertexPlat;

		public const int SizeOf = 0x0C;
	}
}
