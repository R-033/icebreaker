using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Structures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Vector3Packed
	{
		public short X;
		public short Y;
		public short Z;

		public const int SizeOf = 0x06;
	}
}