using Hypercooled.Shared.Structures;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.MostWanted.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryInstance // 0x00034103
	{
		public Vector3 BBoxMin;
		public Vector3 BBoxMax;
		public Shared.MapStream.SceneryInstanceFlags InstanceFlags;
		public ushort PrecullerInfoIndex;
		public short LightingContextNumber;
		public Vector3 Position;
		public Matrix3x3Packed Rotation;
		public ushort SceneryInfoNumber;

		public const int SizeOf = 0x40;
	}
}
