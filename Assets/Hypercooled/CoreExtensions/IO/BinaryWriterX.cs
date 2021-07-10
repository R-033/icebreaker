using CoreExtensions.Conversions;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CoreExtensions.IO
{
	/// <summary>
	/// Provides all major extensions for <see cref="BinaryWriter"/>
	/// </summary>
	public static class BinaryWriterX
	{
		/// <summary>
		/// Position in the current stream as a 4-byte signed integer.
		/// </summary>
		/// <param name="bw">This <see cref="BinaryWriter"/>.</param>
		/// <returns>Position of the stream as a 4-byte signed integer.</returns>
		public static int IntPosition(this BinaryWriter bw) => (int)bw.BaseStream.Position;

		/// <summary>
		/// Length in the current stream as a 4-byte signed integer.
		/// </summary>
		/// <param name="bw">This <see cref="BinaryWriter"/>.</param>
		/// <returns>Length of the stream as a 4-byte signed integer.</returns>
		public static int IntLength(this BinaryWriter bw) => (int)bw.BaseStream.Length;

		/// <summary>
		/// Writes a byte array to the underlying stream in reverse order.
		/// </summary>
		/// <param name="bw"></param>
		/// <param name="array">A byte array containing the data to write.</param>
		public static void WriteReversedBytes(this BinaryWriter bw, byte[] array)
		{
			if (array == null || array.Length == 0) return;
			for (int a1 = array.Length - 1; a1 >= 0; --a1) bw.Write(array[1]);
		}

		/// <summary>
		/// Writes the <see cref="Enum"/> of type <typeparamref name="TypeID"/> and advances
		/// the current position by the size of the underlying type of the <see cref="Enum"/>.
		/// </summary>
		/// <typeparam name="TypeID">Type of the <see cref="Enum"/> to read.</typeparam>
		/// <param name="bw"></param>
		/// <param name="value"><see cref="Enum"/> value to write.</param>
		public static void WriteEnum<TypeID>(this BinaryWriter bw, TypeID value) where TypeID : IConvertible
		{
			var t = typeof(TypeID);
			switch (Type.GetTypeCode(Enum.GetUnderlyingType(t)))
			{
				case TypeCode.SByte:
					bw.Write(CastX.StaticCast<sbyte>(value));
					break;

				case TypeCode.Byte:
					bw.Write(CastX.StaticCast<byte>(value));
					break;

				case TypeCode.Int16:
					bw.Write(CastX.StaticCast<short>(value));
					break;

				case TypeCode.UInt16:
					bw.Write(CastX.StaticCast<ushort>(value));
					break;

				case TypeCode.Int32:
					bw.Write(CastX.StaticCast<int>(value));
					break;

				case TypeCode.UInt32:
					bw.Write(CastX.StaticCast<uint>(value));
					break;

				case TypeCode.Int64:
					bw.Write(CastX.StaticCast<long>(value));
					break;

				case TypeCode.UInt64:
					bw.Write(CastX.StaticCast<ulong>(value));
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Writes a C-Style null-terminated string that using UTF8 encoding.
		/// </summary>
		/// <param name="bw"></param>
		/// <param name="value">String value to write.</param>
		public static void WriteNullTermUTF8(this BinaryWriter bw, string value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				for (int a1 = 0; a1 < value.Length; ++a1) bw.Write((byte)value[a1]);
			}

			bw.Write((byte)0);
		}

		/// <summary>
		/// Writes a C-Style null-terminated string that using UTF16 encoding.
		/// </summary>
		/// <param name="bw"></param>
		/// <param name="value">String value to write.</param>
		public static void WriteNullTermUTF16(this BinaryWriter bw, string value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				for (int a1 = 0; a1 < value.Length; ++a1) bw.Write(value[a1]);
			}

			bw.Write((char)0);
		}

		/// <summary>
		/// Writes a C-Style null-terminated string that using UTF8 encoding.
		/// </summary>
		/// <param name="bw"></param>
		/// <param name="value">String value to write.</param>
		/// <param name="length">Max length of the string to write; if length of the string
		/// is less then length specified, padding will be added after it to fill buffer.</param>
		public static void WriteNullTermUTF8(this BinaryWriter bw, string value, int length)
		{
			if (length <= 0)
			{
				return;
			}
			else if (String.IsNullOrEmpty(value))
			{
				for (int a1 = 0; a1 < length; ++a1) bw.Write((byte)0);
			}
			else
			{
				int dif = (value.Length > length - 1) ? length - 1 : value.Length;
				for (int a1 = 0; a1 < dif; ++a1) bw.Write((byte)value[a1]);
				for (int a1 = dif; a1 < length; ++a1) bw.Write((byte)0);
			}
		}

		/// <summary>
		/// Writes a C-Style null-terminated string that using UTF16 encoding.
		/// </summary>
		/// <param name="bw"></param>
		/// <param name="value">String value to write.</param>
		/// <param name="length">Max length of the string to write; if length of the string
		/// is less then length specified, padding will be added after it to fill buffer.</param>
		public static void WriteNullTermUTF16(this BinaryWriter bw, string value, int length)
		{
			if (String.IsNullOrEmpty(value))
			{
				for (int a1 = 0; a1 < length; ++a1) bw.Write((char)0);
			}
			else
			{
				int dif = (value.Length > length - 1) ? length - 1 : value.Length;
				for (int a1 = 0; a1 < dif; ++a1) bw.Write(value[a1]);
				for (int a1 = dif; a1 < length; ++a1) bw.Write((char)0);
			}
		}

		/// <summary>
		/// Attempts to write struct of type <typeparamref name="T"/>. In order for struct
		/// to be read correctly, it should have a <see cref="StructLayoutAttribute"/>.
		/// </summary>
		/// <typeparam name="T">Type of struct to write.</typeparam>
		/// <param name="bw"></param>
		/// <param name="value">Struct of type <typeparamref name="T"/> to write.</param>
		public static void WriteStruct<T>(this BinaryWriter bw, T value) where T : struct
		{
			var size = Marshal.SizeOf<T>();
			var array = new byte[size];

			var handle = GCHandle.Alloc(array, GCHandleType.Pinned);
			Marshal.StructureToPtr(value, handle.AddrOfPinnedObject(), false);

			bw.Write(array);
			handle.Free();
		}

		/// <summary>
		/// Fills stream buffer till the certain padding is reached.
		/// </summary>
		/// <param name="bw"></param>
		/// <param name="align">Align to fill the stream buffer to.</param>
		public static void FillBuffer(this BinaryWriter bw, int align)
		{
			int padding = align - ((int)(bw.BaseStream.Position % align));
			if (padding == align) padding = 0;
			for (int i = 0; i < padding; ++i) bw.Write((byte)0);
		}

		/// <summary>
		/// Fills stream buffer till the certain padding is reached; padding should be a power of two.
		/// </summary>
		/// <param name="bw"></param>
		/// <param name="align">Align to fill the stream buffer to. This value should be a power of two.</param>
		public static void FillBufferPow2(this BinaryWriter bw, int align)
		{
			int padding = align - ((int)(bw.BaseStream.Position & (align - 1)));
			if (padding == align) padding = 0;
			for (int i = 0; i < padding; ++i) bw.Write((byte)0);
		}

		/// <summary>
		/// Writes amount of bytes specified.
		/// </summary>
		/// <param name="bw"></param>
		/// <param name="value">Byte value to write.</param>
		/// <param name="count">Amount of bytes to write.</param>
		public static void WriteBytes(this BinaryWriter bw, byte value, int count)
		{
			for (int i = 0; i < count; ++i) bw.Write(value);
		}

		/// <summary>
		/// Writes unmanaged value type.
		/// </summary>
		/// <typeparam name="T">Unmanaged type to write.</typeparam>
		/// <param name="bw"></param>
		/// <param name="value">Instance of unmanaged type to write.</param>
		public static unsafe void WriteUnmanaged<T>(this BinaryWriter bw, T value) where T : unmanaged
		{
			var array = new byte[sizeof(T)];
			fixed (byte* ptr = array) { *(T*)ptr = value; }
			bw.Write(array);
		}
	}
}
