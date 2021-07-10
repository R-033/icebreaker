using System.Numerics;
using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct WorldAnimMatrix // 0x00037240
	{
		public Matrix4x4 Transform;

		public const int SizeOf = 0x40;
	}
}
