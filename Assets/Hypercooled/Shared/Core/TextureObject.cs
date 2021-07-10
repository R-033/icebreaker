using Hypercooled.Shared.Textures;
using Hypercooled.Utils;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public abstract class TextureObject
	{
		private Texture2D m_unityTexture;

		public abstract string CollectionName { get; set; }
		public abstract uint Key { get; }

		public abstract byte[] Data { get; set; }
		public abstract TextureCompressionType CompressionType { get; }
		public abstract int Size { get; }
		public abstract int Width { get; }
		public abstract int Height { get; }
		public abstract int Mipmaps { get; }

		public abstract bool ApplyAlphaSorting { get; }
		public abstract TextureAlphaBlendType AlphaBlendType { get; }
		public abstract TextureAlphaUsageType AlphaUsageType { get; }
		public abstract TextureMipmapBiasType MipmapBiasType { get; }
		public abstract TextureTileableType TileableUV { get; }
		public abstract byte BiasLevel { get; }
		public abstract byte RenderingOrder { get; }

		public bool HasPalette
		{
			get
			{
				switch (this.CompressionType)
				{
					case TextureCompressionType.TEXCOMP_4BIT:
					case TextureCompressionType.TEXCOMP_4BIT_IA8:
					case TextureCompressionType.TEXCOMP_4BIT_RGB16_A8:
					case TextureCompressionType.TEXCOMP_4BIT_RGB24_A8:
					case TextureCompressionType.TEXCOMP_8BIT:
					case TextureCompressionType.TEXCOMP_8BIT_16:
					case TextureCompressionType.TEXCOMP_8BIT_64:
					case TextureCompressionType.TEXCOMP_8BIT_IA8:
					case TextureCompressionType.TEXCOMP_8BIT_RGB16_A8:
					case TextureCompressionType.TEXCOMP_8BIT_RGB24_A8:
					case TextureCompressionType.TEXCOMP_16BIT:
					case TextureCompressionType.TEXCOMP_16BIT_1555:
					case TextureCompressionType.TEXCOMP_16BIT_3555:
					case TextureCompressionType.TEXCOMP_16BIT_565:
						return true;

					default:
						return false;
				}
			}
		}

		public Texture2D GetUnityTexture()
		{
			if (this.m_unityTexture == null)
			{
				var converter = new TextureConverter();
				this.m_unityTexture = converter.Convert(this);
				this.m_unityTexture.name = this.CollectionName;
			}

			return this.m_unityTexture;
		}
		public void DestroyUnityTexture()
		{
			Object.Destroy(this.m_unityTexture);
		}
	}
}
