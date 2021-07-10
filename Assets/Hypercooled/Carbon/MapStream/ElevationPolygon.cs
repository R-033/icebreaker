using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ElevationPolygon // 0x00034156
	{
		public uint DepthName;
		public int Padding1;
		public int Padding2;
		public int Padding3;
		public Vector3 Vertex1;
		public int Padding4;
		public Vector3 Vertex2;
		public int Padding5;
		public Vector3 Vertex3;
		public int Padding6;

		public const int SizeOf = 0x40;
	}
}
