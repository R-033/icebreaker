using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackPathPoint // 0x00034148
	{
		public Vector2 Position;
		public short LaneIndex;
		public sbyte SByte0x0A;
		public sbyte SByte0x0B;
		public short NeighborPoint1;
		public short NeighborPoint2;
		public short NeighborPoint3;
		public short Unused;
		public float Distance;

		public const int SizeOf = 0x18;
	}
}
