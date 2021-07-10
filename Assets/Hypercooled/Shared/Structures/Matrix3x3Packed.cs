using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Structures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Matrix3x3Packed
	{
		public short Value11;
		public short Value12;
		public short Value13;
		public short Value21;
		public short Value22;
		public short Value23;
		public short Value31;
		public short Value32;
		public short Value33;

		public const int SizeOf = 0x12;
	}
}
