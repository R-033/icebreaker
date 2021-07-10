using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CoreExtensions.Text
{
	/// <summary>
	/// Provides all major extensions for <see cref="Regex"/> and <see cref="String"/>.
	/// </summary>
	public static class RegX
	{
		/// <summary>
		/// Determines whether given string can be a hexadecimal digit of type 0x[...].
		/// </summary>
		/// <returns>True if string can be a hexadecimal digit; false otherwise.</returns>
		public static bool IsHexString(this string value)
		{
			if (value is null || value.Length < 3) return false;
			if (value[0] != '0') return false;
			if (value[1] != 'x' && value[1] != 'X') return false;

			for (int i = 2; i < value.Length; ++i)
			{
				char c = value[i];

				if (('0' <= c && c <= '9') ||
					('A' <= c && c <= 'F') ||
					('a' <= c && c <= 'f'))
				{
					continue;
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// Attempts to convert hexadecimal string to an unsigned integer value. Hexadecimal
		/// string should start with '0x' or '0X' value.
		/// </summary>
		/// <param name="value">String value to attempt to parse.</param>
		/// <param name="result">Unsigned integer value converted from the string value passed.</param>
		/// <returns>True if conversion was successful; false if string was not a hexadecimal
		/// string and/or contained characters that could not be converted.</returns>
		public static bool TryHexStringToUInt32(this string value, out uint result)
		{
			result = 0;

			if (value is null || value.Length < 3) return false;
			if (value[0] != '0') return false;
			if (value[1] != 'x' && value[1] != 'X') return false;

			for (int i = 2, k = value.Length - 3; i < value.Length; ++i, --k)
			{
				char c = value[i];
				uint mult = (uint)(1 << (k << 2));

				if ('0' <= c && c <= '9')
				{
					result += (uint)(c - '0') * mult;
					continue;
				}
				if ('A' <= c && c <= 'F')
				{
					result += (uint)(c - 'A' + 10) * mult;
					continue;
				}
				if ('a' <= c && c <= 'f')
				{
					result += (uint)(c - 'a' + 10) * mult;
					continue;
				}

				result = 0;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Converts unsigned integer passed to its hexadecimal string representation that
		/// starts with '0x'. Faster than <see cref="UInt32.ToString()"/>.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="toLower">True if all 'a'-'f' characters should be lowercase in the
		/// string representation; false if all 'A'-'F' should be uppercase instead.</param>
		/// <returns>Hexadecimal string representation of an unsigned integer value.</returns>
		public static string FastToHexString(this uint value, bool toLower)
		{
			var array = new char[10] { '0', 'x', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0' };

			if (toLower)
			{
				for (int i = 0, k = 9; i < 8; ++i, --k)
				{
					var bit = (value >> (i << 2)) & 0x0F;

					if (bit < 10) array[k] = (char)(0x30 + bit);
					else array[k] = (char)(0x57 + bit);
				}
			}
			else
			{
				for (int i = 0, k = 9; i < 8; ++i, --k)
				{
					var bit = (value >> (i << 2)) & 0x0F;

					if (bit < 10) array[k] = (char)(0x30 + bit);
					else array[k] = (char)(0x37 + bit);
				}
			}

			return new string(array);
		}

		/// <summary>
		/// Gets first quoted string from the given string.
		/// </summary>
		/// <returns>First quoted string.</returns>
		public static string GetQuotedString(this string value)
		{
			var match = new Regex("[\"]{1}[^\n]*[\"]{1}").Match(value ?? String.Empty);
			return match.Success ? match.Value.Trim('\"') : String.Empty;
		}

		/// <summary>
		/// Splits string by whitespace and quotation marks.
		/// </summary>
		/// <returns><see cref="IEnumerable{T}"/> of strings.</returns>
		public static IEnumerable<string> SmartSplitString(this string value)
		{
			if (String.IsNullOrWhiteSpace(value)) yield break;
			var result = Regex.Split(value, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

			foreach (var str in result)
			{
				if (String.IsNullOrEmpty(str)) continue;
				yield return str.StartsWith("\"") && str.EndsWith("\"") ? str.Substring(1, str.Length - 2) : str;
			}
		}

		/// <summary>
		/// Gets array of bytes of from the current string provided.
		/// </summary>
		/// <param name="value">String to convert to array of bytes.</param>
		/// <returns>Array of bytes of the string.</returns>
		public static byte[] GetBytes(this string value)
		{
			var result = new byte[value.Length];
			for (int i = 0; i < value.Length; ++i) result[i] = (byte)value[i];
			return result;
		}

		/// <summary>
		/// Gets string from array of bytes using UTF8 encoding.
		/// </summary>
		/// <param name="array">Array of bytes to convert to string.</param>
		/// <returns>String from array of bytes.</returns>
		public static string GetString(this byte[] array)
		{
			if (array is null) return null;
			if (array.Length == 0) return String.Empty;
			var sb = new StringBuilder(array.Length);
			for (int i = 0; i < array.Length; ++i) sb.Append((char)array[i]);
			return sb.ToString();
		}

		/// <summary>
		/// Gets HashCode of the string; if string is null, returns String.Empty HashCode.
		/// </summary>
		/// <param name="value"></param>
		/// <returns>HashCode of the string.</returns>
		public static int GetSafeHashCode(this string value) =>
			String.IsNullOrEmpty(value) ? String.Empty.GetHashCode() : value.GetHashCode();

		/// <summary>
		/// Splits string into substrings with length specified.
		/// </summary>
		/// <param name="str">This string to split.</param>
		/// <param name="size">Size of each splitted substring.</param>
		/// <returns><see cref="IEnumerable{T}"/> of substrings.</returns>
		public static IEnumerable<string> SplitByLength(this string str, int size) =>
			Enumerable.Range(0, str.Length / size)
				.Select(i => str.Substring(i * size, size));
	}
}
