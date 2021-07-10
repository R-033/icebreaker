using Hypercooled.Shared.Textures;
using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TextureInfo
	{
		public int Pointer1;
		public int Pointer2;
		public int Pointer3;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x18)]
		public string Name;

		public uint Key;
		public uint ClassName;
		public int Padding1;
		public uint Offset;
		public uint PaletteOffset;
		public int Size;
		public int PaletteSize;
		public int Area;
		public ushort Width;
		public ushort Height;
		public byte ShiftW;
		public byte ShiftH;
		public TextureCompressionType Compression;
		public byte Padding2;
		public short NumPaletteEntries;
		public byte NumMipMapLevels;
		public TextureTileableType TileableUV;
		public byte BiasLevel;
		public byte RenderingOrder;
		public TextureScrollType ScrollType;
		public byte UsedFlag;
		public byte ApplyAlphaSorting;
		public TextureAlphaUsageType AlphaUsageType;
		public TextureAlphaBlendType AlphaBlendType;
		public byte Flags;
		public TextureMipmapBiasType MipmapBiasType;
		public byte Padding3;
		public short ScrollTimeStep;
		public short ScrollSpeedS;
		public short ScrollSpeedT;
		public short OffsetS;
		public short OffsetT;
		public short ScaleS;
		public short ScaleT;
		public int Pointer4;
		public int Pointer5;
		public int Pointer6;
		public int Pointer7;
		public int Pointer8;

		public const int SizeOf = 0x7C;
	}
}
