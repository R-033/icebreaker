using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SmoothVertex // 0x00134018
	{
		public uint VertexKey;
		public byte SmoothingGroupNumber;
		public sbyte NX;
		public sbyte NY;
		public sbyte NZ;

		public const int SizeOf = 0x08;
	}
}
