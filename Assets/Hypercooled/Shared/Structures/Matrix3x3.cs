using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Structures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Matrix3x3
	{
		public float Value11;
		public float Value12;
		public float Value13;
		public float Value21;
		public float Value22;
		public float Value23;
		public float Value31;
		public float Value32;
		public float Value33;

		public const int SizeOf = 0x24;
	}
}
