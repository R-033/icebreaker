using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SmoothVertexPlat
	{
		public uint VertexKey;
		public uint SmoothingGroup;
		public uint VertexOffset;

		public const int SizeOf = 0x0C;
	}
}
