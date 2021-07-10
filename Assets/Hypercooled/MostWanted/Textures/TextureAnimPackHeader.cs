using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TextureAnimPackHeader // 0x30300101
	{
		public int IsValid;
		public int Pointer1;
		public int Pointer2;
		public int Pointer3;

		public const int SizeOf = 0x10;
	}
}
