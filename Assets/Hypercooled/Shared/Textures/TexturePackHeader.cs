using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Textures
{
	public enum TexturePackVersion : int
	{
		Underground1 = 4,
		Underground2 = 5,
		MostWanted = 5,
		Carbon = 8,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TexturePackHeader
	{
		public TexturePackVersion Version;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x1C)]
		public string Descriptor;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
		public string PipelinePath;

		public uint Key;
		public long Pointer1;
		public long Pointer2;
		public long Pointer3;

		public const int SizeOf = 0x7C;
	}
}
