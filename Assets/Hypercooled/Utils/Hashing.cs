using CoreExtensions.Text;
using System;
using System.Collections.Generic;

namespace Hypercooled.Utils
{
	/// <summary>
	/// Provides all major extensions of hashing strings.
	/// </summary>
	public static class Hashing
	{
		private static readonly Dictionary<uint, string> ms_binToString;
		private static readonly Dictionary<uint, string> ms_vltToString;

		static Hashing()
		{
			ms_binToString = new Dictionary<uint, string>
			{
				[0] = String.Empty,
				[UInt32.MaxValue] = String.Empty,
			};
			ms_vltToString = new Dictionary<uint, string>
			{
				[0] = String.Empty,
				[UInt32.MaxValue] = String.Empty,
			};
		}

		/// <summary>
		/// Hashes string passed and returns its binary hash.
		/// </summary>
		/// <param name="value">String to be hashed.</param>
		/// <returns>Bin Memory Hash of the string as an unsigned integer.</returns>
		public static uint BinHash(this string value)
		{
			if (String.IsNullOrWhiteSpace(value)) return 0;
			if (value.TryHexStringToUInt32(out var hex)) return hex;

			var len = 0;
			var result = 0xFFFFFFFF;

			while (len < value.Length)
			{
				result *= 0x21;
				result += (byte)value[len++];
			}

			ms_binToString[result] = value;
			return result;
		}

		/// <summary>
		/// Hashes string passed and returns its binary hash.
		/// </summary>
		/// <param name="value">String to be hashed.</param>
		/// <param name="prefix">Prefix binary hash that string passed should be hashed based on.</param>
		/// <returns>Bin Memory Hash of the string as an unsigned integer.</returns>
		public static uint BinHash(this string value, uint prefix)
		{
			if (String.IsNullOrWhiteSpace(value)) return prefix;
			if (value.TryHexStringToUInt32(out var hex)) return hex;

			for (int len = 0; len < value.Length; ++len)
			{
				prefix *= 0x21;
				prefix += (byte)value[len];
			}

			return prefix;
		}

		/// <summary>
		/// Hashes string passed and returns its vault hash.
		/// </summary>
		/// <param name="value">String to be hashed.</param>
		/// <returns>Vlt Memory Hash of the string as an unsigned integer.</returns>
		public static uint VltHash(this string value)
		{
			if (String.IsNullOrWhiteSpace(value)) return 0;
			if (value.TryHexStringToUInt32(out var hex)) return hex;

			var arr = value.GetBytes();
			var a = 0x9E3779B9;
			var b = 0x9E3779B9;
			var c = 0xABCDEF00;
			var v1 = 0;
			var v2 = arr.Length;

			while (v2 >= 12)
			{

				a += BitConverter.ToUInt32(arr, v1);
				b += BitConverter.ToUInt32(arr, v1 + 4);
				c += BitConverter.ToUInt32(arr, v1 + 8);
				Mix32_1(ref a, ref b, ref c);
				v1 += 12;
				v2 -= 12;

			}

			c += (uint)arr.Length;

			switch (v2)
			{
				case 11:
					c += (uint)arr[10 + v1] << 24;
					goto case 10;

				case 10:
					c += (uint)arr[9 + v1] << 16;
					goto case 9;

				case 9:
					c += (uint)arr[8 + v1] << 8;
					goto case 8;

				case 8:
					b += (uint)arr[7 + v1] << 24;
					goto case 7;

				case 7:
					b += (uint)arr[6 + v1] << 16;
					goto case 6;

				case 6:
					b += (uint)arr[5 + v1] << 8;
					goto case 5;

				case 5:
					b += arr[4 + v1];
					goto case 4;

				case 4:
					a += (uint)arr[3 + v1] << 24;
					goto case 3;

				case 3:
					a += (uint)arr[2 + v1] << 16;
					goto case 2;

				case 2:
					a += (uint)arr[1 + v1] << 8;
					goto case 1;

				case 1:
					a += arr[v1];
					break;

				default:
					break;
			}

			var result = Mix32_2(a, b, c);

			// Put into raider keys
			ms_vltToString[result] = value;
			return result;
		}

		/// <summary>
		/// Tries to resolve Bin key provided.
		/// </summary>
		/// <param name="key">Bin key to resolve.</param>
		/// <param name="type"><see cref="LookupReturn"/> type.</param>
		/// <returns>Resolved Bin Key.</returns>
		public static string BinString(this uint key)
		{
			if (ms_binToString.TryGetValue(key, out var result)) return result;
			else return key.FastToHexString(false);
		}

		/// <summary>
		/// Tries to resolve Vlt key provided.
		/// </summary>
		/// <param name="key">Vlt key to resolve.</param>
		/// <returns>Resolved Vlt Key.</returns>
		public static string VltString(this uint key)
		{
			if (ms_vltToString.TryGetValue(key, out var result)) return result;
			else return key.FastToHexString(false);
		}

		/// <summary>
		/// Checks whether Bin key of the string passed equals the Bin key passed, and, if it is, returns
		/// the string itself, else attempts to return resolved value of the Bin key passed.
		/// </summary>
		/// <param name="key">Bin key passed to check.</param>
		/// <param name="name">String passed to check.</param>
		/// <returns>Resolved string based on comparisons.</returns>
		public static string ResolveBinEqual(uint key, string name)
		{
			if (key == name.BinHash()) return name;
			else return key.BinString();
		}

		/// <summary>
		/// Checks whether Vlt key of the string passed equals the Vlt key passed, and, if it is, returns
		/// the string itself, else attempts to return resolved value of the Vlt key passed.
		/// </summary>
		/// <param name="key">Vlt key passed to check.</param>
		/// <param name="name">String passed to check.</param>
		/// <returns>Resolved string based on comparisons.</returns>
		public static string ResolveVltEqual(uint key, string name)
		{
			if (key == name.VltHash()) return name;
			else return key.VltString();
		}

		private static void Mix32_1(ref uint a, ref uint b, ref uint c)
		{
			a = c >> 13 ^ (a - b - c);
			b = a << 8 ^ (b - c - a);
			c = b >> 13 ^ (c - a - b);
			a = c >> 12 ^ (a - b - c);
			b = a << 16 ^ (b - c - a);
			c = b >> 5 ^ (c - a - b);
			a = c >> 3 ^ (a - b - c);
			b = a << 10 ^ (b - c - a);
			c = b >> 15 ^ (c - a - b);
		}

		private static uint Mix32_2(uint a, uint b, uint c)
		{
			a = c >> 13 ^ (a - b - c);
			b = a << 8 ^ (b - c - a);
			c = b >> 13 ^ (c - a - b);
			a = c >> 12 ^ (a - b - c);
			b = a << 16 ^ (b - c - a);
			c = b >> 5 ^ (c - a - b);
			a = c >> 3 ^ (a - b - c);
			b = a << 10 ^ (b - c - a);
			return b >> 15 ^ (c - a - b);
		}
	}
}
