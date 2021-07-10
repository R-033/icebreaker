using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TexturePackBuffer
	{
		public long Padding;
		public int IsBuffer;
		public uint Key;
		public long Pointer;

		public const int SizeOf = 0x18;
	}
}
