using System.Collections.Generic;

namespace Hypercooled.Utils.DDS
{
	/// <summary>
	/// Collection of DDS palette compression and decompression functions.
	/// </summary>
	public class Palette
	{
		/// <summary>
		/// Convert PAL8 .dds format into 32-bit RGBA.
		/// </summary>
		/// <param name="data">Binary data block to be converted.</param>
		/// <returns>32-bit RGBA byte array converted based on PAL8 palette.</returns>
		public unsafe byte[] P8toRGBA(byte[] data)
		{
			var palette = new uint[0x100]; // palette size is 0x400 bytes = 0x100 ints
			var result = new byte[(data.Length - 0x400) * 4]; // 8 bpp * 4 bytes per color

			fixed (byte* byteptr_t = &data[0])
			{
				// Load palette into memory for access
				for (int loop = 0; loop < 0x100; ++loop)
				{
					var bgra = *(uint*)(byteptr_t + loop * 4);

					var color = (bgra & 0xFF00FF00)
						| ((bgra & 0x00FF0000) >> 16)  // b
						| ((bgra & 0x000000FF) << 16); // r

					palette[loop] = color;
				}
			}

			fixed (byte* byteptr_t = &result[0])
			{
				// Write result data based on palette
				for (int loop = 0, index = 0; loop < data.Length - 0x400; ++loop, index += 4)
				{
					var color = palette[data[loop + 0x400]];
					*(uint*)(byteptr_t + index) = color;
				}
			}

			return result;
		}

		/// <summary>
		/// Convert RGBA .dds format to 8-bit PAL8.
		/// </summary>
		/// <param name="data">Binary data of the .dds file including header.</param>
		/// <returns>PAL8 formatted byte array with palette and compressed data.</returns>
		public unsafe byte[] RGBAtoP8(byte[] data)
		{
			int size = 0x400 + data.Length / 4;
			var result = new byte[size];
			var map = new Dictionary<uint, byte>(0x100);
			int used = 0; // num of palette colors used; max = 0x100

			fixed (byte* byteptr_t = &data[0])
			{
				// Marshal through RGBA data
				for (int offset = 0, mapoff = 0, dataoff = 0x400; offset < data.Length; offset += 4)
				{
					uint color = *(uint*)(byteptr_t + offset);

					if (map.TryGetValue(color, out byte index))
					{
						result[dataoff++] = index;
					}
					else
					{
						if (used == 0x100) return null; // return null if palette is over
						map[color] = (byte)used; // add palette to the dictionary
						result[mapoff++] = *(byteptr_t + offset + 2); // b
						result[mapoff++] = *(byteptr_t + offset + 1); // g
						result[mapoff++] = *(byteptr_t + offset + 0); // r
						result[mapoff++] = *(byteptr_t + offset + 3); // a
						result[dataoff++] = (byte)used++;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Convert PAL4 .dds format into 32-bit RGBA.
		/// </summary>
		/// <param name="data">Binary data block to be converted.</param>
		/// <returns>32-bit RGBA byte array converted based on PAL4 palette.</returns>
		public unsafe byte[] P4toRGBA(byte[] data)
		{
			var palette = new uint[0x10]; // palette size is 0x400 bytes = 0x100 ints
			var result = new byte[(data.Length - 0x40) * 8]; // 4 bpp * 4 bytes per color

			fixed (byte* byteptr_t = &data[0])
			{
				// Load palette into memory for access
				for (int loop = 0; loop < 0x10; ++loop)
				{
					var bgra = *(uint*)(byteptr_t + loop * 4);

					var color = (bgra & 0xFF00FF00)
						| ((bgra & 0x00FF0000) >> 16)  // b
						| ((bgra & 0x000000FF) << 16); // r

					palette[loop] = color;
				}
			}

			fixed (byte* byteptr_t = &result[0])
			{
				// Write result data based on palette
				for (int loop = 0, index = 0; loop < data.Length - 0x40; ++loop, index += 8)
				{
					var bit1 = data[loop + 0x40] >> 4; // first 4 bits in the byte being read
					var bit2 = data[loop + 0x40] & 0x0F; // second 4 bits in the byte being read

					*(uint*)(byteptr_t + index) = palette[bit1];
					*(uint*)(byteptr_t + index + 4) = palette[bit2];
				}
			}

			return result;
		}

		/// <summary>
		/// Convert RGBA .dds format to 4-bit PAL4.
		/// </summary>
		/// <param name="data">Binary data of the .dds file including header.</param>
		/// <returns>PAL4 formatted byte array with palette and compressed data.</returns>
		public unsafe byte[] RGBAtoP4(byte[] data)
		{
			int size = 0x40 + data.Length / 8;
			var result = new byte[size];
			var map = new Dictionary<uint, byte>(0x10);
			byte used = 0; // num of palette colors used; max = 0x10

			fixed (byte* byteptr_t = &data[0])
			{
				// Marshal through RGBA data
				for (int offset = 0, mapoff = 0, dataoff = 0x40; offset < data.Length; offset += 8)
				{
					uint color1 = *(uint*)(byteptr_t + offset);
					uint color2 = *(uint*)(byteptr_t + offset + 4);

					if (!map.TryGetValue(color1, out byte bit1))
					{
						if (used == 0x10) return null; // return null if palette is over
						bit1 = used; // assign current color index
						map[color1] = used++; // add palette to the dictionary

						// Write color to the palette
						result[mapoff++] = *(byteptr_t + offset + 2); // b
						result[mapoff++] = *(byteptr_t + offset + 1); // g
						result[mapoff++] = *(byteptr_t + offset + 0); // r
						result[mapoff++] = *(byteptr_t + offset + 3); // a
					}

					if (!map.TryGetValue(color2, out byte bit2))
					{
						if (used == 0x10) return null; // return null if palette is over
						bit2 = used; // assign current color index
						map[color2] = used++; // add palette to the dictionary

						// Write color to the palette
						result[mapoff++] = *(byteptr_t + offset + 6); // b
						result[mapoff++] = *(byteptr_t + offset + 5); // g
						result[mapoff++] = *(byteptr_t + offset + 4); // r
						result[mapoff++] = *(byteptr_t + offset + 7); // a
					}

					result[dataoff++] = (byte)((bit1 << 4) | bit2);
				}
			}

			return result;
		}
	}
}
