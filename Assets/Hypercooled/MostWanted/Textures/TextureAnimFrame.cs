using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TextureAnimFrame // 0x30300103
	{
		public uint Key;
		public int Padding1;
		public int Padding2;
		public int Padding3;

		public const int SizeOf = 0x10;
	}
}
