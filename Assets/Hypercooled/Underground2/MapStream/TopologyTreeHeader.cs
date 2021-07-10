using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TopologyTreeHeader // 0x00034131
	{
		public ushort SectionNumber;
		public byte Flag1;
		public byte Flag2;
		public ushort Width;
		public ushort Height;
		public Vector2 Pivot;
		public Vector4 BBoxMin;
		public Vector4 BBoxMax;

		public const int SizeOf = 0x30;
	}
}
