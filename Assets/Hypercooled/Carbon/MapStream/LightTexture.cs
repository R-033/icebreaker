using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.MapStream
{
	public struct LightTextureEntry
	{
		public uint Key;
		public int Padding;

		public const int SizeOf = 0x08;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LightTextureCollection // 0x0003410D
	{
		public LightTextureEntry LightTexture1;
		public LightTextureEntry LightTexture2;
		public LightTextureEntry LightTexture3;
		public LightTextureEntry DirectionalMap;

		public const int SizeOf = 0x20;
	}
}
