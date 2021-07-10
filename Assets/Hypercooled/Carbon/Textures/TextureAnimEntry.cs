using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TextureAnimEntry // 0x33312001
	{
		public long Padding1;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
		public string Name;

		public uint Key;
		public byte NumFrames;
		public byte FramesPerSecond;
		public byte TimeBase;
		public byte Empty;

		public int Padding2;
		public long Padding3;

		public const int SizeOf = 0x2C;
	}
}
