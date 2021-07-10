using Hypercooled.Shared.Solids;
using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MeshInfoHeader // 0x00134900
	{
		public long Padding0;
		public short Version;
		public byte ChunksLoaded;
		public byte Pad;
		public MeshEntryFlags Flags;
		public int NumSubMeshes;
		public long Padding1;
		public long Padding2;
		public int NumPolygons;
		public long Padding3;
		public int Padding4;
		public int NumVertices;
		public long Padding5;
		public int Padding6;

		public const int SizeOf = 0x44;
	}
}
