using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace CoreExtensions.IO
{
	/// <summary>
	/// Provides all major extensions for <see cref="BinaryReader"/>
	/// </summary>
	public static class BinaryReaderX
	{
		/// <summary>
		/// Represents <see cref="Dictionary{TKey,TValue}"/> of <see cref="TypeCode"/> and
		/// <see cref="BinaryReader"/> reading methods.
		/// </summary>
		private static Dictionary<TypeCode, Delegate> _type_code_to_reader =
			new Dictionary<TypeCode, Delegate>()
		{
			{
				TypeCode.Boolean,
				new Func<BinaryReader, bool>(br => br.ReadBoolean())
			},

			{
				TypeCode.Byte,
				new Func<BinaryReader, byte>(br => br.ReadByte())
			},

			{
				TypeCode.SByte,
				new Func<BinaryReader, sbyte>(br => br.ReadSByte())
			},

			{
				TypeCode.Char,
				new Func<BinaryReader, char>(br => br.ReadChar())
			},

			{
				TypeCode.Int16,
				new Func<BinaryReader, short>(br => br.ReadInt16())
			},

			{
				TypeCode.UInt16,
				new Func<BinaryReader, ushort>(br => br.ReadUInt16())
			},

			{
				TypeCode.Int32,
				new Func<BinaryReader, int>(br => br.ReadInt32())
			},

			{
				TypeCode.UInt32,
				new Func<BinaryReader, uint>(br => br.ReadUInt32())
			},

			{
				TypeCode.Int64,
				new Func<BinaryReader, long>(br => br.ReadInt64())
			},

			{
				TypeCode.UInt64,
				new Func<BinaryReader, ulong>(br => br.ReadUInt64())
			},

			{
				TypeCode.Single,
				new Func<BinaryReader, float>(br => br.ReadSingle())
			},

			{
				TypeCode.Double,
				new Func<BinaryReader, double>(br => br.ReadDouble())
			},

			{
				TypeCode.Decimal,
				new Func<BinaryReader, decimal>(br => br.ReadDecimal())
			},

			{
				TypeCode.String,
				new Func<BinaryReader, string>(br => br.ReadNullTermUTF8())
			}
		};

		/// <summary>
		/// Position in the current stream as a 4-byte signed integer.
		/// </summary>
		/// <param name="br">This <see cref="BinaryReader"/>.</param>
		/// <returns>Position of the stream as a 4-byte signed integer.</returns>
		public static int IntPosition(this BinaryReader br) => (int)br.BaseStream.Position;

		/// <summary>
		/// Length in the current stream as a 4-byte signed integer.
		/// </summary>
		/// <param name="br">This <see cref="BinaryReader"/>.</param>
		/// <returns>Length of the stream as a 4-byte signed integer.</returns>
		public static int IntLength(this BinaryReader br) => (int)br.BaseStream.Length;

		/// <summary>
		/// Reads the specified number of bytes from the current stream into a byte array
		/// in reverse order and advances the current position by that number of bytes.
		/// </summary>
		/// <param name="br"></param>
		/// <param name="count">The number of bytes to read. This value must be 0 or a
		/// non-negative number or an exception will occur.</param>
		/// <returns>A byte array containing data read from the underlying stream. This might be
		/// less than the number of bytes requested if the end of the stream is reached.</returns>
		public static byte[] ReadReversedBytes(this BinaryReader br, int count)
		{
			var arr = new byte[count];
			for (int a1 = count - 1; a1 >= 0; --a1) arr[a1] = br.ReadByte();
			return arr;
		}

		/// <summary>
		/// Reads the <see cref="Enum"/> of type <typeparamref name="TypeID"/> and advances
		/// the current position by the size of the underlying type of the <see cref="Enum"/>.
		/// </summary>
		/// <typeparam name="TypeID">Type of the <see cref="Enum"/> to read.</typeparam>
		/// <returns>Value of the <see cref="Enum"/> passed. If value could not be parsed,
		/// or if the type passed is not Enum, exception might be thrown.</returns>
		public static TypeID ReadEnum<TypeID>(this BinaryReader br) where TypeID : Enum
		{
			var t = typeof(TypeID);
			switch (Type.GetTypeCode(Enum.GetUnderlyingType(t)))
			{
				case TypeCode.SByte:
					return (TypeID)Enum.ToObject(t, br.ReadSByte());

				case TypeCode.Byte:
					return (TypeID)Enum.ToObject(t, br.ReadByte());

				case TypeCode.Int16:
					return (TypeID)Enum.ToObject(t, br.ReadInt16());

				case TypeCode.UInt16:
					return (TypeID)Enum.ToObject(t, br.ReadUInt16());

				case TypeCode.Int32:
					return (TypeID)Enum.ToObject(t, br.ReadInt32());

				case TypeCode.UInt32:
					return (TypeID)Enum.ToObject(t, br.ReadUInt32());

				case TypeCode.Int64:
					return (TypeID)Enum.ToObject(t, br.ReadInt64());

				case TypeCode.UInt64:
					return (TypeID)Enum.ToObject(t, br.ReadUInt64());

				default:
					return default;
			}
		}

		/// <summary>
		/// Reads a C-Style null-terminated string that using UTF8 encoding.
		/// </summary>
		/// <returns>String with UTF8 style encoding.</returns>
		public static string ReadNullTermUTF8(this BinaryReader br)
		{
			string result = String.Empty;
			byte b;
			while ((b = br.ReadByte()) != 0) result += ((char)b).ToString();
			return result;
		}

		/// <summary>
		/// Reads a C-Style null-terminated string that using UTF16 encoding.
		/// </summary>
		/// <returns>String with UTF16 style encoding.</returns>
		public static string ReadNullTermUTF16(this BinaryReader br)
		{
			string result = String.Empty;
			char c;
			while ((c = br.ReadChar()) != '\0') result += c.ToString();
			return result;
		}

		/// <summary>
		/// Reads a C-Style null-terminated string that using UTF8 encoding.
		/// </summary>
		/// <param name="br"></param>
		/// <param name="length">Max length of the string to read.</param>
		/// <returns>String with UTF8 style encoding.</returns>
		public static string ReadNullTermUTF8(this BinaryReader br, int length)
		{
			string result = String.Empty;
			byte b;
			var pos = br.BaseStream.Position + length;
			for (int a1 = 0; a1 < length && (b = br.ReadByte()) != 0; ++a1) result += ((char)b).ToString();
			br.BaseStream.Position = pos;
			return result;
		}

		/// <summary>
		/// Reads a C-Style null-terminated string that using UTF16 encoding.
		/// </summary>
		/// <param name="br"></param>
		/// <param name="length">Max length of the string to read.</param>
		/// <returns>String with UTF16 style encoding.</returns>
		public static string ReadNullTermUTF16(this BinaryReader br, int length)
		{
			string result = String.Empty;
			char c;
			var pos = br.BaseStream.Position + length * sizeof(char);
			for (int a1 = 0; a1 < length && (c = br.ReadChar()) != 0; ++a1) result += c.ToString();
			br.BaseStream.Position = pos;
			return result;
		}

		/// <summary>
		/// Seeks position of the first occurence of the byte array provided.
		/// </summary>
		/// <param name="br"></param>
		/// <param name="array">Byte array to find.</param>
		/// <param name="fromstart">True if begin seeking from the start of the stream;
		/// false otherwise.</param>
		/// <returns>Position of the first occurence of the byte array. If array was not
		/// found, returns -1.</returns>
		public static long SeekArray(this BinaryReader br, byte[] array, bool fromstart)
		{
			if (!fromstart && array.Length > br.BaseStream.Length - br.BaseStream.Position) return -1;
			else if (fromstart && array.Length > br.BaseStream.Length) return -1;
			var pos = br.BaseStream.Position;
			long result = -1;
			int current = 0;
			int maxmatch = array.Length;
			if (fromstart) br.BaseStream.Position = 0;
			while (current < maxmatch && br.BaseStream.Position < br.BaseStream.Length)
			{
				byte b = br.ReadByte();
				if (b == array[current]) ++current;
				else if (b != array[current] && current > 0)
				{
					br.BaseStream.Position -= current;
					current = 0;
				}
				else current = 0;
			}
			if (current == maxmatch) result = br.BaseStream.Position - current;
			br.BaseStream.Position = pos;
			return result;
		}

		/// <summary>
		/// Seeks position of the first occurence of the convertible value provided (value cannot
		/// be string).
		/// </summary>
		/// <typeparam name="TypeID">Type of the value to find.</typeparam>
		/// <param name="br"></param>
		/// <param name="value">Value to find.</param>
		/// <param name="fromstart">True if begin seeking from the start of the stream;
		/// false otherwise.</param>
		/// <returns>Position of the first occurence of the value provided.. If array was not
		/// found, returns -1.</returns>
		public static long SeekValue<TypeID>(this BinaryReader br, TypeID value, bool fromstart)
			where TypeID : IConvertible
		{
			if (typeof(TypeID) == typeof(string)) return -1;
			if (!_type_code_to_reader.TryGetValue(Type.GetTypeCode(typeof(TypeID)), out var func)) return -1;

			var size = Marshal.SizeOf(typeof(TypeID));
			var pos = br.BaseStream.Position;
			long result = -1;
			if (fromstart) br.BaseStream.Position = 0;
			while (br.BaseStream.Position < br.BaseStream.Length)
			{
				var obj = (TypeID)func.DynamicInvoke(br);
				if (obj.Equals(value))
				{
					br.BaseStream.Position -= Marshal.SizeOf(typeof(TypeID));
					result = br.BaseStream.Position;
					break;
				}
			}
			br.BaseStream.Position = pos;
			return result;
		}

		/// <summary>
		/// Reads unmanaged value type.
		/// </summary>
		/// <typeparam name="T">Unmanaged type to read.</typeparam>
		/// <param name="br"></param>
		/// <returns>Instance of unmanaged type provided.</returns>
		public static unsafe T ReadUnmanaged<T>(this BinaryReader br) where T : unmanaged
		{
			var arr = br.ReadBytes(sizeof(T));
			fixed (byte* ptr = arr) { return *(T*)ptr; }
		}

		/// <summary>
		/// Reads a structure of the given type from a binary reader.
		/// </summary>
		/// <param name="br">A <see cref="BinaryReader"/> instance to read data from.</param>
		/// <typeparam name="T">The structure type. Must be a C# struct.</typeparam>
		/// <returns>A new instance of <typeparamref name="T"/> with data read from <paramref name="br"/>.</returns>
		public static T ReadStruct<T>(this BinaryReader br) where T : struct
		{
			var size = Marshal.SizeOf<T>();
			var buffer = br.ReadBytes(size);

			var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			var result = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
			handle.Free();

			return result;
		}
	}
}
