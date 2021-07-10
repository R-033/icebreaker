using CoreExtensions.Management;
using System;
using UnityEngine;

namespace Hypercooled.Utils
{
	public class TextureConverter
	{
		private readonly byte[] m_dxt3compressedAlphas;
		private readonly byte[] m_dxt3alphas;
		private readonly Color[] m_dxt3colorTable;
		private readonly Color32[] m_dxt1colors;
		private readonly uint[] m_dxt1colorIndices;
		private readonly byte[] m_dxt1colorIndexBytes;

		public TextureConverter()
		{
			m_dxt3compressedAlphas = new byte[0x10];
			m_dxt3alphas = new byte[0x10];
			m_dxt3colorTable = new Color[0x04];
			m_dxt1colors = new Color32[0x10];
			m_dxt1colorIndices = new uint[0x10];
			m_dxt1colorIndexBytes = new byte[0x04];
		}

		public Texture2D Convert(Shared.Core.TextureObject texture)
		{
			try
			{
				var format = TextureFormat.RGBA32;
				var data = texture.Data;
				if (data is null) return null;

				switch (texture.CompressionType)
				{
					case Shared.Textures.TextureCompressionType.TEXCOMP_DXTC1:
						format = TextureFormat.DXT1;
						break;

					case Shared.Textures.TextureCompressionType.TEXCOMP_DXTC3:
						data = DecodeDXT3ToARGB(data, (uint)texture.Width, (uint)texture.Height);
						format = TextureFormat.ARGB32;
						break;

					case Shared.Textures.TextureCompressionType.TEXCOMP_DXTC5:
						format = TextureFormat.DXT5;
						break;

					case Shared.Textures.TextureCompressionType.TEXCOMP_8BIT:
						data = new DDS.Palette().P8toRGBA(data);
						break;

					case Shared.Textures.TextureCompressionType.TEXCOMP_32BIT:
						break;

					default:
						Debug.LogWarning($"Unsupported texture format {texture.CompressionType} in texture {texture.CollectionName}");
						return null;
				}

				var result = new Texture2D(texture.Width, texture.Height, format, false);
				result.LoadRawTextureData(data);
				result.Apply(true);
				return result;
			}
			catch (Exception ex)
			{
#if DEBUG
				Debug.LogError(ex.GetLowestMessage());
#endif
				return null;
			}
		}

		private ulong GetBits(uint bitOffset, uint bitCount, byte[] bytes)
		{
			Debug.Assert((bitCount <= 64) && ((bitOffset + bitCount) <= (8 * bytes.Length)));

			ulong bits = 0;
			uint remainingBitCount = bitCount;
			uint byteIndex = bitOffset / 8;
			uint bitIndex = bitOffset - (8 * byteIndex);

			uint numBitsLeftInByte = 0;
			uint numBitsReadNow = 0;
			uint unmaskedBits = 0;
			uint bitMask = 0;
			uint bitsReadNow = 0;

			while (remainingBitCount > 0)
			{
				// Read bits from the byte array.
				numBitsLeftInByte = 8 - bitIndex;
				numBitsReadNow = Math.Min(remainingBitCount, numBitsLeftInByte);
				unmaskedBits = (uint)bytes[byteIndex] >> (int)(8 - (bitIndex + numBitsReadNow));
				bitMask = 0xFFu >> (int)(8 - numBitsReadNow);
				bitsReadNow = unmaskedBits & bitMask;

				// Store the bits we read.
				bits <<= (int)numBitsReadNow;
				bits |= bitsReadNow;

				// Prepare for the next iteration.
				bitIndex += numBitsReadNow;

				if (bitIndex == 8)
				{
					byteIndex++;
					bitIndex = 0;
				}

				remainingBitCount -= numBitsReadNow;
			}

			return bits;
		}

		private Color32[] DecodeDXT1TexelBlock(byte[] reader, ref int ind, Color[] colorTable)
		{
			Debug.Assert(colorTable.Length == 4);

			for (int i = 0; i < m_dxt1colorIndexBytes.Length; i++)
			{
				m_dxt1colorIndexBytes[i] = reader[ind++];
			}

			const uint bitsPerColorIndex = 2;

			uint rowIndex = 0;
			uint columnIndex = 0;
			uint rowBaseColorIndexIndex = 0;
			uint rowBaseBitOffset = 0;
			uint bitOffset = 0;

			for (rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				rowBaseColorIndexIndex = 4 * rowIndex;
				rowBaseBitOffset = 8 * rowIndex;

				for (columnIndex = 0; columnIndex < 4; columnIndex++)
				{
					// Color indices are arranged from right to left.
					bitOffset = rowBaseBitOffset + (bitsPerColorIndex * (3 - columnIndex));

					m_dxt1colorIndices[rowBaseColorIndexIndex + columnIndex] = (uint)GetBits(bitOffset, bitsPerColorIndex, m_dxt1colorIndexBytes);
				}
			}

			// Calculate pixel colors.
			//var colors = new Color32[16];

			for (rowIndex = 0; rowIndex < 16; rowIndex++)
			{
				m_dxt1colors[rowIndex] = colorTable[m_dxt1colorIndices[rowIndex]];
			}

			return m_dxt1colors;
		}

		private byte[] DecodeDXT3ToARGB(byte[] compressedData, uint width, uint height)
		{
			var argb = new byte[width * height * 4];

			int ind = 0;

			for (int rowIndex = 0; rowIndex < height; rowIndex += 4)
			{
				for (int columnIndex = 0; columnIndex < width; columnIndex += 4)
				{
					Color32[] colors = DecodeDXT3TexelBlock(compressedData, ref ind);
					CopyDecodedTexelBlock(colors, argb, rowIndex, columnIndex, (int)width, (int)height);
				}
			}

			return argb;
		}

		private Color R5G6B5ToColor(ushort R5G6B5)
		{
			var R5 = ((R5G6B5 >> 11) & 31);
			var G6 = ((R5G6B5 >> 5) & 63);
			var B5 = (R5G6B5 & 31);

			return new Color((float)R5 / 31, (float)G6 / 63, (float)B5 / 31, 1);
		}

		private Color32[] DecodeDXT3TexelBlock(byte[] reader, ref int ind)
		{
			// Read compressed pixel alphas.
			//var compressedAlphas = new byte[16];
			int rowIndex = 0;
			int columnIndex = 0;
			ushort compressedAlphaRow = 0;

			for (rowIndex = 0; rowIndex < 4; rowIndex++)
			{
				compressedAlphaRow = BitConverter.ToUInt16(reader, ind);
				ind += 2;

				for (columnIndex = 0; columnIndex < 4; columnIndex++)
				{
					// Each compressed alpha is 4 bits.
					m_dxt3compressedAlphas[(4 * rowIndex) + columnIndex] = (byte)((compressedAlphaRow >> (columnIndex * 4)) & 0xF);
				}
			}

			// Calculate pixel alphas.
			//var Dxt3_Alphas = new byte[16];
			int i = 0;
			for (i = 0; i < 16; i++)
			{
				m_dxt3alphas[i] = (byte)Mathf.RoundToInt(
					((float)m_dxt3compressedAlphas[i] / 15)
					* 255);
			}

			// Create the color table.
			//var colorTable = new Color[4];
			m_dxt3colorTable[0] = R5G6B5ToColor(BitConverter.ToUInt16(reader, ind));
			ind += 2;
			m_dxt3colorTable[1] = R5G6B5ToColor(BitConverter.ToUInt16(reader, ind));
			ind += 2;
			m_dxt3colorTable[2] = Color.Lerp(m_dxt3colorTable[0], m_dxt3colorTable[1], 1.0f / 3);
			m_dxt3colorTable[3] = Color.Lerp(m_dxt3colorTable[0], m_dxt3colorTable[1], 2.0f / 3);

			// Calculate pixel colors.
			Color32[] colors = DecodeDXT1TexelBlock(reader, ref ind, m_dxt3colorTable);

			for (i = 0; i < 16; i++)
			{
				colors[i].a = m_dxt3alphas[i];
			}

			return colors;
		}

		private void CopyDecodedTexelBlock(Color32[] decodedTexels, byte[] argb, int baseRowIndex, int baseColumnIndex, int textureWidth, int textureHeight)
		{
			for (int i = 0; i < 4; i++) // row
			{
				for (int j = 0; j < 4; j++) // column
				{
					var rowIndex = baseRowIndex + i;
					var columnIndex = baseColumnIndex + j;

					// Don't copy padding on mipmaps.
					if ((rowIndex < textureHeight) && (columnIndex < textureWidth))
					{
						var decodedTexelIndex = (4 * i) + j;
						var color = decodedTexels[decodedTexelIndex];

						var ARGBPixelOffset = (textureWidth * rowIndex) + columnIndex;
						var basePixelARGBIndex = 4 * ARGBPixelOffset;

						argb[basePixelARGBIndex] = color.a;
						argb[basePixelARGBIndex + 1] = color.r;
						argb[basePixelARGBIndex + 2] = color.g;
						argb[basePixelARGBIndex + 3] = color.b;
					}
				}
			}
		}
	}
}
