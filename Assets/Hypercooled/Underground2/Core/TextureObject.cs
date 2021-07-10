using Hypercooled.Shared.Textures;
using Hypercooled.Underground2.Textures;
using Hypercooled.Utils;
using System;

namespace Hypercooled.Underground2.Core
{
	public class TextureObject : Shared.Core.TextureObject
	{
		private TextureInfo m_textureInfo;
		private CompressionSlot m_compSlot;
		private byte[] m_data;

		public override string CollectionName
		{
			get
			{
				return this.m_textureInfo.Name;
			}
			set
			{
				this.m_textureInfo.Name = value ?? String.Empty;
				this.m_textureInfo.Key = value.BinHash();
			}
		}
		public override uint Key => this.m_textureInfo.Key;

		public TextureInfo Info
		{
			get
			{
				return this.m_textureInfo;
			}
			set
			{
				this.m_textureInfo = value;
				this.m_textureInfo.Name = Hashing.ResolveBinEqual(value.Key, value.Name);
			}
		}
		public CompressionSlot CompSlot
		{
			get => this.m_compSlot;
			set => this.m_compSlot = value;
		}
		public override byte[] Data
		{
			get => this.m_data;
			set => this.m_data = value ?? new byte[0];
		}
		public override TextureCompressionType CompressionType => this.m_textureInfo.Compression;
		public override int Size => this.m_textureInfo.Size + this.m_textureInfo.PaletteSize;
		public override int Width => this.m_textureInfo.Width;
		public override int Height => this.m_textureInfo.Height;
		public override int Mipmaps => this.m_textureInfo.NumMipMapLevels;

		public override bool ApplyAlphaSorting => this.Info.ApplyAlphaSorting != 0;
		public override TextureAlphaBlendType AlphaBlendType => this.Info.AlphaBlendType;
		public override TextureAlphaUsageType AlphaUsageType => this.Info.AlphaUsageType;
		public override TextureMipmapBiasType MipmapBiasType => this.Info.MipmapBiasType;
		public override TextureTileableType TileableUV => this.Info.TileableUV;
		public override byte BiasLevel => this.Info.BiasLevel;
		public override byte RenderingOrder => this.Info.RenderingOrder;

		public TextureObject() : base()
		{
			this.m_data = new byte[0];
		}
	}
}
