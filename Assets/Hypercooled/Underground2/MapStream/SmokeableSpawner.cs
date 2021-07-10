using System.Runtime.InteropServices;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SmokeableSpawnHeader
	{
		public long Padding;
		public ushort SectionNumber;
		public ushort StartIndex;
		public int NumSpawners;

		public const int SizeOf = 0x10;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SmokeableSpawner // 0x00034027
	{
		public uint SmokeableInfoKey;
		public int SceneryOverrideIndex;
		public uint SolidMeshKey;
		public int Padding0;
		public Vector3 BoundsMin;
		public int Padding1;
		public Vector3 BoundsMax;
		public int Padding2;
		public Matrix4x4 Transform;

		public const int SizeOf = 0x70;
	}
}
