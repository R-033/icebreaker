using CoreExtensions.IO;
using Hypercooled.Shared.Textures;
using Hypercooled.Shared.Core;
using System;
using System.IO;



namespace Hypercooled.Utils.DDS
{
	/// <summary>
	/// A class with DDS reading and writing options.
	/// </summary>
	public class Generator
	{
		private byte m_mode; // 1 = write | 2 = read
		private byte[] m_stream;
		private bool m_noPalette = false;
		private TextureCompressionType m_prev;

		/// <summary>
		/// DDS data of the texture.
		/// </summary>
		public byte[] Buffer { get; private set; }

		/// <summary>
		/// <see cref="TextureCompressionType"/> type of the texture.
		/// </summary>
		public TextureCompressionType Compression { get; private set; }

		/// <summary>
		/// Area of the texture.
		/// </summary>
		public int Area { get; private set; }

		/// <summary>
		/// Height of the texture.
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Width of the texture.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Number of mipmaps in the texture.
		/// </summary>
		public int MipMaps { get; private set; }

		/// <summary>
		/// Size of the texture, in bytes.
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// Palette size of the texture, in bytes.
		/// </summary>
		public int PaletteSize { get; private set; }

		/// <summary>
		/// True if settings can be read and generator is based on DDS data passed; otherwise, false.
		/// </summary>
		public bool CanRead => this.m_mode == 1;

		/// <summary>
		/// True if DDS data can be written based on <see cref="Texture"/> passed; otherwise, false.
		/// </summary>
		public bool CanWrite => this.m_mode == 2;

		private bool IsCompressed()
		{
			switch (this.Compression)
			{
				case TextureCompressionType.TEXCOMP_DXTC1: return true;
				case TextureCompressionType.TEXCOMP_DXTC3: return true;
				case TextureCompressionType.TEXCOMP_DXTC5: return true;
				default: return false;
			}
		}

		private int PitchLinearSize()
		{
			int result;

			switch (this.Compression)
			{
				case TextureCompressionType.TEXCOMP_DXTC1:
					result = (1 > (this.Width + 3) / 4) ? 1 : (this.Width + 3) / 4;
					result *= (1 > (this.Height + 3) / 4) ? 1 : (this.Height + 3) / 4;
					result *= 8;
					break;

				case TextureCompressionType.TEXCOMP_DXTC3:
				case TextureCompressionType.TEXCOMP_DXTC5:
					result = (1 > (this.Width + 3) / 4) ? 1 : (this.Width + 3) / 4;
					result *= (1 > (this.Height + 3) / 4) ? 1 : (this.Height + 3) / 4;
					result *= 16;
					break;

				case TextureCompressionType.TEXCOMP_4BIT:
				case TextureCompressionType.TEXCOMP_4BIT_IA8:
				case TextureCompressionType.TEXCOMP_4BIT_RGB16_A8:
				case TextureCompressionType.TEXCOMP_4BIT_RGB24_A8:
					result = this.Width * 4 + 7;
					result /= 8;
					break;

				case TextureCompressionType.TEXCOMP_8BIT:
				case TextureCompressionType.TEXCOMP_8BIT_16:
				case TextureCompressionType.TEXCOMP_8BIT_64:
				case TextureCompressionType.TEXCOMP_8BIT_IA8:
				case TextureCompressionType.TEXCOMP_8BIT_RGB16_A8:
				case TextureCompressionType.TEXCOMP_8BIT_RGB24_A8:
					result = this.Width * 8 + 7;
					result /= 8;
					break;

				case TextureCompressionType.TEXCOMP_16BIT:
				case TextureCompressionType.TEXCOMP_16BIT_1555:
				case TextureCompressionType.TEXCOMP_16BIT_3555:
				case TextureCompressionType.TEXCOMP_16BIT_565:
					result = this.Width * 16 + 7;
					result /= 8;
					break;

				case TextureCompressionType.TEXCOMP_24BIT:
					result = this.Width * 24 + 7;
					result /= 8;
					break;

				case TextureCompressionType.TEXCOMP_32BIT:
					result = this.Width * 32 + 7;
					result /= 8;
					break;

				default:
					result = this.Width * 32 + 7;
					result /= 8;
					break;

			}

			return result;
		}

		private void WritePixelFormat(BinaryWriter bw)
		{
			var format = new DDS_PIXELFORMAT();

			switch (this.Compression)
			{
				case TextureCompressionType.TEXCOMP_DXTC1:
					DDS_CONST.DDSPF_DXT1(format);
					break;

				case TextureCompressionType.TEXCOMP_DXTC3:
					DDS_CONST.DDSPF_DXT3(format);
					break;

				case TextureCompressionType.TEXCOMP_DXTC5:
					DDS_CONST.DDSPF_DXT5(format);
					break;

				case TextureCompressionType.TEXCOMP_4BIT:
					DDS_CONST.DDSPF_PAL4(format);
					break;

				case TextureCompressionType.TEXCOMP_4BIT_IA8:
				case TextureCompressionType.TEXCOMP_4BIT_RGB16_A8:
				case TextureCompressionType.TEXCOMP_4BIT_RGB24_A8:
					DDS_CONST.DDSPF_PAL4A(format);
					break;

				case TextureCompressionType.TEXCOMP_8BIT:
				case TextureCompressionType.TEXCOMP_8BIT_16:
				case TextureCompressionType.TEXCOMP_8BIT_64:
					DDS_CONST.DDSPF_PAL8(format);
					break;

				case TextureCompressionType.TEXCOMP_8BIT_IA8:
				case TextureCompressionType.TEXCOMP_8BIT_RGB16_A8:
				case TextureCompressionType.TEXCOMP_8BIT_RGB24_A8:
					DDS_CONST.DDSPF_PAL8A(format);
					break;

				case TextureCompressionType.TEXCOMP_32BIT:
					DDS_CONST.DDSPF_A8R8G8B8(format);
					break;

				default:
					DDS_CONST.DDSPF_A8R8G8B8(format);
					break;

			}

			bw.Write(format.dwSize);
			bw.Write(format.dwFlags);
			bw.Write(format.dwFourCC);
			bw.Write(format.dwRGBBitCount);
			bw.Write(format.dwRBitMask);
			bw.Write(format.dwGBitMask);
			bw.Write(format.dwBBitMask);
			bw.Write(format.dwABitMask);
		}

		private void ReadCompression(uint flag, uint code)
		{
			switch (flag)
			{
				case (uint)DDS_TYPE.FOURCC:
					this.PaletteSize = 0;
					switch (code)
					{
						case 0x31545844: this.Compression = TextureCompressionType.TEXCOMP_DXTC1; return;
						case 0x33545844: this.Compression = TextureCompressionType.TEXCOMP_DXTC3; return;
						case 0x35545844: this.Compression = TextureCompressionType.TEXCOMP_DXTC5; return;
						default: throw new NotSupportedException("Not supported texture compression");
					}

				case (uint)DDS_TYPE.PAL4:
					this.Compression = TextureCompressionType.TEXCOMP_4BIT;
					this.PaletteSize = 0x40;
					return;

				case (uint)DDS_TYPE.PAL4A:
					this.Compression = TextureCompressionType.TEXCOMP_4BIT_IA8;
					this.PaletteSize = 0x40;
					return;

				case (uint)DDS_TYPE.PAL8:
					this.Compression = TextureCompressionType.TEXCOMP_8BIT;
					this.PaletteSize = 0x400;
					return;

				case (uint)DDS_TYPE.PAL8A:
					this.Compression = TextureCompressionType.TEXCOMP_8BIT_IA8;
					this.PaletteSize = 0x400;
					return;

				case (uint)DDS_TYPE.RGB:
					this.Compression = TextureCompressionType.TEXCOMP_24BIT;
					this.PaletteSize = 0;
					return;

				case (uint)DDS_TYPE.RGBA:
					this.Compression = TextureCompressionType.TEXCOMP_32BIT;
					this.PaletteSize = 0;
					return;

				default:
					throw new NotSupportedException("Not supported texture compression");

			}
		}

		private void ReadArea()
		{
			switch (this.Compression)
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
					this.Area = this.Width * this.Height;
					return;

				case TextureCompressionType.TEXCOMP_32BIT:
					this.Area = this.Width * this.Height * 4;
					return;

				default:
					this.Area = this.FlipToBase(this.Size);
					return;
			}
		}

		private int FlipToBase(int size)
		{
			uint x = (uint)size;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			x -= x >> 1;
			return (int)x;
		}

		/// <summary>
		/// Initializes new instance of <see cref="Generator"/> with ability to read settings of 
		/// DDS data buffer passed.
		/// </summary>
		/// <param name="buffer">DDS data buffer to read.</param>
		/// <param name="compToPalette">True if attempt to compress RGBA 32 bit DDS data 
		/// to 8 bit one; </param>
		public Generator(byte[] buffer, bool compToPalette)
		{
			this.m_mode = 1;
			this.m_noPalette = !compToPalette;
			this.m_stream = buffer;
		}

		/// <summary>
		/// Initializes new instance of <see cref="Generator"/> with ability to write DDS data 
		/// buffer based on <see cref="Texture"/> passed.
		/// </summary>
		/// <param name="texture"><see cref="Texture"/> to write data of.</param>
		/// <param name="makeNoPalette">True if palette should be decompressed into 
		/// 32 bpp DDS; false otherwise.</param>
		public Generator(TextureObject texture, bool makeNoPalette)
		{
			this.m_mode = 2;
			this.m_noPalette = makeNoPalette;
			this.Buffer = texture.Data;
			this.Height = texture.Height;
			this.Width = texture.Width;
			this.MipMaps = texture.Mipmaps;

			if (makeNoPalette)
			{
				this.m_noPalette = texture.HasPalette;
				this.m_prev = texture.CompressionType;
				this.Compression = texture.HasPalette
					? TextureCompressionType.TEXCOMP_32BIT
					: texture.CompressionType;
			}
			else
			{
				this.Compression = texture.CompressionType;
			}
		}

		/// <summary>
		/// Gets DDS data buffer for a texture passed.
		/// </summary>
		/// <returns>DDS data buffer.</returns>
		public byte[] GetDDSTexture()
		{
			if (!this.CanWrite)
			{
				throw new InvalidOperationException("Generator is not initialized for reading data");
			}

			return this.m_noPalette ? this.GetDDSTexWithLimits() : this.GetDDSTexWithoutLimits();
		}
	
		/// <summary>
		/// Gets settings from DDS data buffer passed.
		/// </summary>
		public void GetDDSSettings()
		{
			if (!this.CanRead)
			{
				throw new InvalidOperationException("Generator is not initialized for writing data");
			}

			using (var ms = new MemoryStream(this.m_stream))
			using (var br = new BinaryReader(ms))
			{

				br.BaseStream.Position += 0xC; // skip ID, size and flags
				this.Height = br.ReadInt32();
				this.Width = br.ReadInt32();
				br.BaseStream.Position += 0x8; // skip pitch/linear size and depth
				this.MipMaps = br.ReadInt32();
				if (this.MipMaps == 0) this.MipMaps = 1; // in case it is mipmap-less texture

				br.BaseStream.Position += 0x30; // skips padding

				var flag = br.ReadUInt32();
				var code = br.ReadUInt32();
				this.ReadCompression(flag, code);

			}

			if (this.m_noPalette) this.GetDDSSetWithoutLimits();
			else this.GetDDSSetWithLimits();

			this.ReadArea();
		}

		private byte[] GetDDSTexWithoutLimits()
		{
			var flags = DDS_HEADER_FLAGS.TEXTURE | DDS_HEADER_FLAGS.MIPMAP;
			flags |= this.IsCompressed() ? DDS_HEADER_FLAGS.LINEARSIZE : DDS_HEADER_FLAGS.PITCH;

			var array = new byte[this.Buffer.Length + 0x80];

			using (var ms = new MemoryStream(array))
			using (var bw = new BinaryWriter(ms))
			{

				bw.Write(DDS_MAIN.MAGIC);
				bw.Write(0x7C); // size = 0x7C
				bw.WriteEnum(flags);
				bw.Write(this.Height);
				bw.Write(this.Width);
				bw.Write(this.PitchLinearSize());
				bw.Write(1); // depth = 1
				bw.Write(this.MipMaps);

				bw.WriteBytes(0, 0x2C);
				this.WritePixelFormat(bw);

				bw.WriteEnum(DDS_SURFACE.SURFACE_FLAGS_ALL);
				bw.WriteBytes(0, 0x10);

			}

			Array.Copy(this.Buffer, 0, array, 0x80, this.Buffer.Length);
			return array;
		}

		private byte[] GetDDSTexWithLimits()
		{
			switch (this.m_prev)
			{
				case TextureCompressionType.TEXCOMP_4BIT:
				case TextureCompressionType.TEXCOMP_4BIT_IA8:
				case TextureCompressionType.TEXCOMP_4BIT_RGB16_A8:
				case TextureCompressionType.TEXCOMP_4BIT_RGB24_A8:
					this.Buffer = new Palette().P4toRGBA(this.Buffer);
					break;

				case TextureCompressionType.TEXCOMP_8BIT:
				case TextureCompressionType.TEXCOMP_8BIT_16:
				case TextureCompressionType.TEXCOMP_8BIT_64:
				case TextureCompressionType.TEXCOMP_8BIT_IA8:
				case TextureCompressionType.TEXCOMP_8BIT_RGB16_A8:
				case TextureCompressionType.TEXCOMP_8BIT_RGB24_A8:
					this.Buffer = new Palette().P8toRGBA(this.Buffer);
					break;

				default:
					throw new NotSupportedException("Unable to convert palette to RGBA format");

			}

			return this.GetDDSTexWithoutLimits();
		}

		private void GetDDSSetWithoutLimits()
		{
			var length = this.m_stream.Length - 0x80;
			this.Buffer = new byte[length];
			Array.Copy(this.m_stream, 0x80, this.Buffer, 0, length);
			this.Size = length - this.PaletteSize;
		}

		private void GetDDSSetWithLimits()
		{
			this.GetDDSSetWithoutLimits();

			if (this.Compression == TextureCompressionType.TEXCOMP_32BIT)
			{

				var array = new Palette().RGBAtoP8(this.Buffer);
				if (array == null) return;

				this.Buffer = array;
				this.Compression = TextureCompressionType.TEXCOMP_8BIT;
				this.PaletteSize = 0x400;
				this.Size = array.Length - 0x400;

			}
		}
	}
}
