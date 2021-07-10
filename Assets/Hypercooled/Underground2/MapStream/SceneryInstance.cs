using Hypercooled.Shared.Structures;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryInstance // 0x00034103
	{
		public Vector3 BBoxMin;
		public Vector3 BBoxMax;
		public ushort SceneryInfoNumber;
		public ushort InstanceFlags;
		public int PrecullerInfoIndex;
		public Vector3 Position;
		public Matrix3x3Packed Rotation;
		public ushort Padding;

		public const int SizeOf = 0x40;
	}
}
