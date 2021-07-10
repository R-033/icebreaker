using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.MostWanted.MapStream
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
		public Quaternion Rotation;
		public Vector4 Position;
		public uint Model;
		public uint CollisionKey;
		public uint Attributes;
		public int SceneryOverrideInfoNumber;
		public uint UniqueID; // basically index
		public uint ExcludeFlags;
		public uint SpawnFlags;
		public int Padding;

		public const int SizeOf = 0x40;
	}
}
