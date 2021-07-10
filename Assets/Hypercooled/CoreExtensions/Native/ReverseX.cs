namespace CoreExtensions.Native
{
	/// <summary>
	/// Provides all major extensions for reversing (endian-swapping) primitive values.
	/// </summary>
	public static class ReverseX
	{
		/// <summary>
		/// Reverses (endian-swaps) a 2-byte signed integer value.
		/// </summary>
		/// <param name="value">A 2-byte signed integer value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 2-byte signed integer value.</returns>
		public static short Reverse(this short value)
		{
			return (short)((value >> 8) | ((value & 0xFF) << 8));
		}

		/// <summary>
		/// Reverses (endian-swaps) a 2-byte unsigned integer value.
		/// </summary>
		/// <param name="value">A 2-byte unsigned integer value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 2-byte unsigned integer value.</returns>
		public static ushort Reverse(this ushort value)
		{
			return (ushort)((value >> 8) | ((value & 0xFF) << 8));
		}

		/// <summary>
		/// Reverses (endian-swaps) a 2-byte char value.
		/// </summary>
		/// <param name="value">A 2-byte char value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 2-byte char value.</returns>
		public static char Reverse(this char value)
		{
			return (char)(((short)value >> 8) | (((short)value & 0xFF) << 8));
		}

		/// <summary>
		/// Reverses (endian-swaps) a 4-byte signed integer value.
		/// </summary>
		/// <param name="value">A 4-byte signed integer value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 4-byte signed integer value.</returns>
		public static int Reverse(this int value)
		{
			return (value << 24) |
				   (((value >> 16) << 24) >> 16) |
				   (((value << 16) >> 24) << 16) |
				   (value >> 24);
		}

		/// <summary>
		/// Reverses (endian-swaps) a 4-byte unsigned integer value.
		/// </summary>
		/// <param name="value">A 4-byte unsigned integer value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 4-byte unsigned integer value.</returns>
		public static uint Reverse(this uint value)
		{
			return (value << 24) |
				   (((value >> 16) << 24) >> 16) |
				   (((value << 16) >> 24) << 16) |
				   (value >> 24);
		}

		/// <summary>
		/// Reverses (endian-swaps) a 4-byte floating point value.
		/// </summary>
		/// <param name="value">A 4-byte floating point value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 4-byte floating point value.</returns>
		public static float Reverse(this float value)
		{
			unsafe
			{
				var ptr = (byte*)&value;
				var arr = new byte[4] { ptr[3], ptr[2], ptr[1], ptr[0] };
				fixed (byte* _ = &arr[0]) { return *(float*)_; }
			}
		}

		/// <summary>
		/// Reverses (endian-swaps) an 8-byte signed integer value.
		/// </summary>
		/// <param name="value">An 8-byte signed integer value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 8-byte signed integer value.</returns>
		public static long Reverse(this long value)
		{
			unsafe
			{
				var ptr = (byte*)&value;
				var arr = new byte[8] { ptr[7], ptr[6], ptr[5], ptr[4],
										ptr[3], ptr[2], ptr[1], ptr[0]};
				fixed (byte* _ = &arr[0]) { return *(long*)_; }
			}
		}

		/// <summary>
		/// Reverses (endian-swaps) an 8-byte unsigned integer value.
		/// </summary>
		/// <param name="value">An 8-byte unsigned integer value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 8-byte unsigned integer value.</returns>
		public static ulong Reverse(this ulong value)
		{
			unsafe
			{
				var ptr = (byte*)&value;
				var arr = new byte[8] { ptr[7], ptr[6], ptr[5], ptr[4],
										ptr[3], ptr[2], ptr[1], ptr[0]};
				fixed (byte* _ = &arr[0]) { return *(ulong*)_; }
			}
		}

		/// <summary>
		/// Reverses (endian-swaps) an 8-byte floating point value.
		/// </summary>
		/// <param name="value">An 8-byte floating point value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 8-byte floating point value.</returns>
		public static double Reverse(this double value)
		{
			unsafe
			{
				var ptr = (byte*)&value;
				var arr = new byte[8] { ptr[7], ptr[6], ptr[5], ptr[4],
										ptr[3], ptr[2], ptr[1], ptr[0]};
				fixed (byte* _ = &arr[0]) { return *(double*)_; }
			}
		}

		/// <summary>
		/// Reverses (endian-swaps) a 16-byte floating point value.
		/// </summary>
		/// <param name="value">A 16-byte floating point value to reverse (endian-swap).</param>
		/// <returns>Reversed (endian-swapped) 16-byte floating point value.</returns>
		public static decimal Reverse(this decimal value)
		{
			unsafe
			{
				var ptr = (byte*)&value;
				var arr = new byte[16] { ptr[15], ptr[14], ptr[13], ptr[12],
										 ptr[11], ptr[10],  ptr[9],  ptr[8],
										 ptr[7],   ptr[6],  ptr[5],  ptr[4],
										 ptr[3],   ptr[2],  ptr[1],  ptr[0]};
				fixed (byte* _ = &arr[0]) { return *(decimal*)_; }
			}
		}
	}
}
