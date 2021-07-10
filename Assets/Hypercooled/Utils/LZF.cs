/*
 *
 * Natively improved version of C# LibLZF Port using pointers and native allocations:
 * Copyright (c) 2020 MaxHwoy <max.hwoy@gmail.com>
 *
 * Improved version to C# LibLZF Port:
 * Copyright (c) 2010 Roman Atachiants <kelindar@gmail.com>
 *
 * Original CLZF Port:
 * Copyright (c) 2005 Oren J. Maurice <oymaurice@hazorea.org.il>
 *
 * Original LibLZF Library & Algorithm:
 * Copyright (c) 2000-2008 Marc Alexander Lehmann <schmorp@schmorp.de>
 *
 * Redistribution and use in source and binary forms, with or without modifica-
 * tion, are permitted provided that the following conditions are met:
 *
 *   1.  Redistributions of source code must retain the above copyright notice,
 *       this list of conditions and the following disclaimer.
 *
 *   2.  Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *
 *   3.  The name of the author may not be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MER-
 * CHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO
 * EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPE-
 * CIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
 * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTH-
 * ERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 * Alternatively, the contents of this file may be used under the terms of
 * the GNU General Public License version 2 (the "GPL"), in which case the
 * provisions of the GPL are applicable instead of the above. If you wish to
 * allow the use of your version of this file only under the terms of the
 * GPL and not to allow others to use your version of this file under the
 * BSD license, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the GPL. If
 * you do not delete the provisions above, a recipient may use your version
 * of this file under either the BSD or the GPL.
 */

using CoreExtensions.Native;
using System;
using System.Runtime.InteropServices;

namespace Hypercooled.Utils
{
	/// <summary>
	/// Improved C# LZF Compressor, a very small data compression library. The compression algorithm is extremely fast.
	/// </summary>
	public class LZF
	{
		private readonly long[] m_hashTable = new long[HSIZE];

		private const uint HLOG = 14;
		private const uint HSIZE = (1 << 14);
		private const uint MAX_LIT = (1 << 5);
		private const uint MAX_OFF = (1 << 13);
		private const uint MAX_REF = ((1 << 8) + (1 << 3));

		/// <summary>
		/// Compresses buffer passed using LZF compression algorithm.
		/// </summary>
		/// <param name="input">Input buffer to compress.</param>
		/// <returns>Compressed buffer.</returns>
		public unsafe byte[] Compress(byte[] input)
		{
			if (input == null || input.Length == 0) return new byte[0];

			fixed (byte* inputPtr = input)
			{
				int inputCount = input.Length;
				int outputCount = input.Length << 1;
				var output = new NativeArray<byte>(outputCount);
				int finalCount = 0;
				byte[] result = null;

				try
				{
					finalCount = Compress(inputPtr, inputCount, output.GetPointer(), outputCount);

					while (finalCount == 0)
					{
						output.Free();
						outputCount <<= 1;
						output = new NativeArray<byte>(outputCount);
						finalCount = Compress(inputPtr, inputCount, output.GetPointer(), outputCount);
					}
				}
				finally
				{
					result = new byte[finalCount];
					if (finalCount > 0) Marshal.Copy((IntPtr)output.GetPointer(), result, 0, finalCount);
					output.Free();
				}

				return result;
			}
		}

		/// <summary>
		/// Decompresses buffer passed using LZF decompression algorithm.
		/// </summary>
		/// <param name="input">Input buffer to decompress.</param>
		/// <param name="decompressedSize">Actual original decompressed size, if known. In case
		/// size is not final, resizes output automatically.</param>
		/// <returns>Decompressed buffer.</returns>
		public unsafe byte[] Decompress(byte[] input, int decompressedSize = 0)
		{
			if (input == null || input.Length == 0) return new byte[0];

			fixed (byte* inputPtr = input)
			{
				int inputCount = input.Length;
				int outputCount = decompressedSize <= 0 ? input.Length << 1 : decompressedSize;
				var output = new NativeArray<byte>(outputCount);
				int finalCount = 0;
				byte[] result = null;

				try
				{
					finalCount = Decompress(inputPtr, inputCount, output.GetPointer(), outputCount);

					while (finalCount == 0)
					{
						output.Free();
						outputCount <<= 1;
						output = new NativeArray<byte>(outputCount);
						finalCount = Decompress(inputPtr, inputCount, output.GetPointer(), outputCount);
					}
				}
				finally
				{
					result = new byte[finalCount];
					if (finalCount > 0) Marshal.Copy((IntPtr)output.GetPointer(), result, 0, finalCount);
					output.Free();
				}

				return result;
			}
		}

		/// <summary>
		/// Compresses buffer passed using LZF compression algorithm.
		/// </summary>
		/// <param name="input">Input buffer to compress.</param>
		/// <returns>Compressed buffer.</returns>
		public unsafe byte[] Compress(NativeArray<byte> input)
		{
			int inputCount = input.Length;
			int outputCount = input.Length << 1;
			var output = new NativeArray<byte>(outputCount);
			int finalCount = 0;
			byte[] result = null;

			try
			{
				finalCount = Compress(input.GetPointer(), inputCount, output.GetPointer(), outputCount);

				while (finalCount == 0)
				{
					output.Free();
					outputCount <<= 1;
					output = new NativeArray<byte>(outputCount);
					finalCount = Compress(input.GetPointer(), inputCount, output.GetPointer(), outputCount);
				}
			}
			finally
			{
				result = new byte[finalCount];
				if (finalCount > 0) Marshal.Copy((IntPtr)output.GetPointer(), result, 0, finalCount);
				output.Free();
			}

			return result;
		}

		/// <summary>
		/// Decompresses buffer passed using LZF decompression algorithm.
		/// </summary>
		/// <param name="input">Input buffer to decompress.</param>
		/// <param name="decompressedSize">Actual original decompressed size, if known. In case
		/// size is not final, resizes output automatically.</param>
		/// <returns>Decompressed buffer.</returns>
		public unsafe byte[] Decompress(NativeArray<byte> input, int decompressedSize = 0)
		{
			int inputCount = input.Length;
			int outputCount = decompressedSize <= 0 ? input.Length << 1 : decompressedSize;
			var output = new NativeArray<byte>(outputCount);
			int finalCount = 0;
			byte[] result = null;

			try
			{
				finalCount = Decompress(input.GetPointer(), inputCount, output.GetPointer(), outputCount);

				while (finalCount == 0)
				{
					output.Free();
					outputCount <<= 1;
					output = new NativeArray<byte>(outputCount);
					finalCount = Decompress(input.GetPointer(), inputCount, output.GetPointer(), outputCount);
				}
			}
			finally
			{
				result = new byte[finalCount];
				if (finalCount > 0) Marshal.Copy((IntPtr)output.GetPointer(), result, 0, finalCount);
				output.Free();
			}

			return result;
		}

		private unsafe int Compress(byte* input, int inputLength, byte* output, int outputLength)
		{
			Array.Clear(m_hashTable, 0, (int)HSIZE);

			long hslot;
			uint iidx = 0;
			uint oidx = 0;
			long reference;

			uint hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]); // FRST(in_data, iidx);
			long off;
			int lit = 0;

			while (true)
			{
				if (iidx < inputLength - 2)
				{
					hval = (hval << 8) | input[iidx + 2];
					hslot = ((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1));
					reference = m_hashTable[hslot];
					m_hashTable[hslot] = (long)iidx;

					if ((off = iidx - reference - 1) < MAX_OFF
						&& iidx + 4 < inputLength
						&& reference > 0
						&& input[reference + 0] == input[iidx + 0]
						&& input[reference + 1] == input[iidx + 1]
						&& input[reference + 2] == input[iidx + 2]
						)
					{
						/* match found at *reference++ */
						uint len = 2;
						uint maxlen = (uint)inputLength - iidx - len;
						maxlen = maxlen > MAX_REF ? MAX_REF : maxlen;

						if (oidx + lit + 1 + 3 >= outputLength) return 0;

						do { len++; }
						while (len < maxlen && input[reference + len] == input[iidx + len]);

						if (lit != 0)
						{
							output[oidx++] = (byte)(lit - 1);
							lit = -lit;
							do { output[oidx++] = input[iidx + lit]; }
							while ((++lit) != 0);
						}

						len -= 2;
						iidx++;

						if (len < 7)
						{
							output[oidx++] = (byte)((off >> 8) + (len << 5));
						}
						else
						{
							output[oidx++] = (byte)((off >> 8) + (7 << 5));
							output[oidx++] = (byte)(len - 7);
						}

						output[oidx++] = (byte)off;

						iidx += len - 1;
						hval = (uint)(((input[iidx]) << 8) | input[iidx + 1]);

						hval = (hval << 8) | input[iidx + 2];
						m_hashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
						iidx++;

						hval = (hval << 8) | input[iidx + 2];
						m_hashTable[((hval ^ (hval << 5)) >> (int)(((3 * 8 - HLOG)) - hval * 5) & (HSIZE - 1))] = iidx;
						iidx++;
						continue;
					}
				}
				else if (iidx == inputLength) break;

				/* one more literal byte we must copy */
				lit++;
				iidx++;

				if (lit == MAX_LIT)
				{
					if (oidx + 1 + MAX_LIT >= outputLength) return 0;

					output[oidx++] = (byte)(MAX_LIT - 1);
					lit = -lit;
					do { output[oidx++] = input[iidx + lit]; }
					while ((++lit) != 0);
				}
			}

			if (lit != 0)
			{
				if (oidx + lit + 1 >= outputLength) return 0;

				output[oidx++] = (byte)(lit - 1);
				lit = -lit;
				do { output[oidx++] = input[iidx + lit]; }
				while ((++lit) != 0);
			}

			return (int)oidx;
		}

		private unsafe int Decompress(byte* input, int inputLength, byte* output, int outputLength)
		{
			uint iidx = 0;
			uint oidx = 0;

			do
			{
				uint ctrl = input[iidx++];

				if (ctrl < (1 << 5)) /* literal run */
				{
					ctrl++;

					if (oidx + ctrl > outputLength)
					{
						//SET_ERRNO (E2BIG);
						return 0;
					}

					do { output[oidx++] = input[iidx++]; }
					while ((--ctrl) != 0);
				}
				else /* back reference */
				{
					uint len = ctrl >> 5;

					int reference = (int)(oidx - ((ctrl & 0x1f) << 8) - 1);

					if (len == 7) len += input[iidx++];

					reference -= input[iidx++];

					if (oidx + len + 2 > outputLength)
					{
						//SET_ERRNO (E2BIG);
						return 0;
					}

					if (reference < 0)
					{
						//SET_ERRNO (EINVAL);
						return 0;
					}

					output[oidx++] = output[reference++];
					output[oidx++] = output[reference++];

					do { output[oidx++] = output[reference++]; }
					while ((--len) != 0);
				}
			}
			while (iidx < inputLength);

			return (int)oidx;
		}
	}
}
