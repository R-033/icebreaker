using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TextureAnimFrame // 0x33312002
	{
		public uint Key;
		public long Padding;

		public const int SizeOf = 0x0C;
	}
}
