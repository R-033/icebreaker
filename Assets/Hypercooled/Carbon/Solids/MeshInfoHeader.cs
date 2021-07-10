using Hypercooled.Shared.Solids;
using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MeshInfoHeader  // 0x00134900
	{
		public long Padding0;
		public short Version;
		public byte ChunksLoaded;
		public byte Pad;
		public MeshEntryFlags Flags;
		public int NumSubMeshes;
		public int Padding1;
		public int NumVertexBuffers;
		public long Padding2;
		public long Padding3;
		public int NumPolygons;
		public int NumVertices;

		public const int SizeOf = 0x34;
	}
}
