using System.Numerics;
using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WorldAnimRTNode // 0x00037250
	{
		public Matrix4x4 Transform;
		public uint MeshNameKey;

		public byte ParentIndex;
		public byte Speed;
		public byte Properties;
		public byte TypeFlags;

		public uint LodAKey;
		public uint LodBKey;
		public uint LodCKey;

		public uint SmackableID;
		public uint SceneryGUID;

		public ushort RotateSpeedX;
		public ushort RotateSpeedZ;
		public ushort RotateSpeedY;
		public ushort SectionNumber;

		public int NumMatrices;

		public short NumMatrixDuplicate; // ???
		public short ShiftAngle;

		public int SomeNumber;
		public int Padding;

		public const int SizeOf = 0x74;
	}
}
