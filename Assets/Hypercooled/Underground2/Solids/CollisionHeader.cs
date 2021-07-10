using System.Runtime.InteropServices;
using Hypercooled.Utils;

namespace Hypercooled.Underground2.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CollisionHeader // 0x00039200
	{
		public long Integer3;
		public long Padding0;
		public int Integer1;
		public int NumberOfVertices; // in 0x00039201
		public uint Key;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
		public string Name;

		public short NumberOfIndeces; // in 0x00039202
		public short Padding1;
		public long Padding2;
		public long Padding3;
		public long Padding4;

		public const int SizeOf = 0x58;
	}
}
