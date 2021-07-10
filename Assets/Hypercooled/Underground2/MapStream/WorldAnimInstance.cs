using System.Runtime.InteropServices;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WorldAnimInstance // 0x00037090
	{
		public int UniqueID;
		public uint AnimTreeKey;
		public int SectionNumber;
		public int PlayFlags;
		public float Speed;
		public float MasterDelay;
		public float LoopDelay;
		public short BeginRange;
		public short EndRange;
		public uint StartTriggerKey;
		public uint StopTriggerKey;
		public uint LodBKey;
		public uint LodZKey;
		public uint NamedRangeKey;
		public uint EventTrackKey;
		public int TrackNumber;
		public short TrackDirection;
		public short Padding0; // 0x6969
		public Matrix4x4 Transform;
		public Vector3 BBoxMin;
		public int Padding1;
		public Vector3 BBoxMax;
		public int Padding2;

		public const int SizeOf = 0xA0;
	}
}
