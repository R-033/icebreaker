using System.Runtime.InteropServices;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Hypercooled.Carbon.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SolidInfo  // 0x00134011
	{
		public long Padding0;
		public int Padding1;
		public byte Version;
		public byte EndianSwapped;
		public Shared.Solids.SolidInfoFlags Flags;
		public uint Key;
		public short NumPolys;
		public short NumVerts;
		public byte NumBones;
		public byte NumTexEntries;
		public byte NumMaterials;
		public byte NumMarkers;
		public int RefFrameCounter;
		public Vector3 AABBMin;
		public int Pointer1;
		public Vector3 AABBMax;
		public int Pointer2;
		public Matrix4x4 Pivot;
		public long Padding2;
		public long Padding3;
		public long Pointer3;
		public float Volume;
		public float Density;

		public const int SizeOf = 0xA0;
	}
}
