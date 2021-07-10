using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MeshInfoHeader  // 0x00134900
	{
		public long Padding0;
		public short Version;
		public byte ChunksLoaded;
		public byte Pad;
		public Shared.Solids.MeshEntryFlags Flags;
		public int NumSubMeshes;
		public int Padding1;
		public int NumVertexBuffers;
		public long Padding2;
		public long Padding3;
		public int NumPolygons;

		public const int SizeOf = 0x30;
	}
}
