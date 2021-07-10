using Hypercooled.Shared.Structures;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CVBound
	{
		public Vector4Packed Rotation;
		public Vector3Packed Position;
		public BoundFlags Flags;
		public Vector3Packed HalfDimension;
		public byte NumChildren;
		public sbyte CloudIndex;
		public Vector3Packed Pivot;
		public short ChildIndex;
		public uint AttributeKey;
		public uint SurfaceKey;
		public uint NameKey;
		public int Padding;

		public const int SizeOf = 0x30;
	}
}
