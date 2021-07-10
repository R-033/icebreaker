using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CollisionVertex // 0x00039201
	{
		public long Padding3;
		public ushort NumCloseVerts;
		public ushort TableOffset;
		public int Index;
		public Vector4 Position;
		public long Padding0;
		public long Padding1;

		public const int SizeOf = 0x30;
	}
}
