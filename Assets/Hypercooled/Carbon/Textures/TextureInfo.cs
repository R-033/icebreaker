using Hypercooled.Shared.Textures;
using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.Textures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TextureInfo
	{
		public int Padding1;
		public long Padding2;
		public uint Key;
		public uint ClassName;
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
		public byte Padding3;
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
		public byte Padding4;
		public short ScrollTimeStep;
		public short ScrollSpeedS;
		public short ScrollSpeedT;
		public short OffsetS;
		public short OffsetT;
		public short ScaleS;
		public short ScaleT;
		public long Padding5;
		public int Padding6;

		public const int SizeOf = 0x58;
	}
}
