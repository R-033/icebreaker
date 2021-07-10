using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TextureAnimEntry // 0x30300102
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x18)]
		public string Name;

		public uint Key;
		public int NumFrames;
		public int FramePerSecond;
		public int TimeBase;
		public int Padding1;
		public int Padding2;
		public int Padding3;

		public const int SizeOf = 0x34;
	}
}
