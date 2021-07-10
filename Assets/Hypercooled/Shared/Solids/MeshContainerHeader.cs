using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MeshContainerHeader // 0x00134002
	{
		public long Padding0;
		public int Version;
		public int NumberOfMeshes;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x38)]
		public string PipelinePath;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
		public string Descriptor;

		public uint CVOffset;
		public int CVSize;
		public int MeshAlignment; // not valid in carbon anymore yikes?

		public long Padding1;
		public long Padding2;
		public long Padding3;
		public int Padding4;

		public const int SizeOf = 0x90;
	}
}
