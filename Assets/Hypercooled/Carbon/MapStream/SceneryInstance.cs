using Hypercooled.Shared.Structures;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryInstance // 0x00034103
	{
		public Vector3 BBoxMin;
		public Shared.MapStream.SceneryInstanceFlags InstanceFlags;
		public Vector3 BBoxMax;
		public ushort PrecullerInfoIndex;
		public short LightingContextNumber;
		public Vector3 Position;
		public Matrix3x3 Rotation;
		public uint GUID;
		public ushort SceneryInfoNumber;
		public byte LODLevel;
		public byte CustomFlags;
		public long Padding;

		public const int SizeOf = 0x60;
	}
}
