namespace CoreExtensions.Native
{
	/// <summary>
	/// Provides all major extensions for changing internal byte values of integer types.
	/// </summary>
	public static class PointerX
	{
		/// <summary>
		/// Changes lower (first) byte of the 2-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) byte of the 2-byte
		/// signed integer.</param>
		/// <returns>New 2-byte signed integer with replaced lower (first) byte.</returns>
		public static unsafe short LOBYTE(this short num, byte value)
		{
			*(byte*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) byte of the 2-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) byte of the 2-byte
		/// unsigned integer.</param>
		/// <returns>New 2-byte unsigned integer with replaced lower (first) byte.</returns>
		public static unsafe ushort LOBYTE(this ushort num, byte value)
		{
			*(byte*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) byte of the 4-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) byte of the 4-byte
		/// signed integer.</param>
		/// <returns>New 4-byte signed integer with replaced lower (first) byte.</returns>
		public static unsafe int LOBYTE(this int num, byte value)
		{
			*(byte*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) byte of the 4-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) byte of the 4-byte
		/// unsigned integer.</param>
		/// <returns>New 4-byte unsigned integer with replaced lower (first) byte.</returns>
		public static unsafe uint LOBYTE(this uint num, byte value)
		{
			*(byte*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) byte of the 8-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) byte of the 8-byte
		/// signed integer.</param>
		/// <returns>New 8-byte signed integer with replaced lower (first) byte.</returns>
		public static unsafe long LOBYTE(this long num, byte value)
		{
			*(byte*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) byte of the 8-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) byte of the 8-byte
		/// unsigned integer.</param>
		/// <returns>New 8-byte unsigned integer with replaced lower (first) byte.</returns>
		public static unsafe ulong LOBYTE(this ulong num, byte value)
		{
			*(byte*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) byte of the 2-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) byte of the 2-byte
		/// signed integer.</param>
		/// <returns>New 2-byte signed integer with replaced higher (second) byte.</returns>
		public static unsafe short HIBYTE(this short num, byte value)
		{
			*((byte*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) byte of the 2-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) byte of the 2-byte
		/// unsigned integer.</param>
		/// <returns>New 2-byte unsigned integer with replaced higher (second) byte.</returns>
		public static unsafe ushort HIBYTE(this ushort num, byte value)
		{
			*((byte*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) byte of the 4-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) byte of the 4-byte
		/// signed integer.</param>
		/// <returns>New 4-byte signed integer with replaced higher (second) byte.</returns>
		public static unsafe int HIBYTE(this int num, byte value)
		{
			*((byte*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) byte of the 4-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) byte of the 4-byte
		/// unsigned integer.</param>
		/// <returns>New 4-byte unsigned integer with replaced higher (second) byte.</returns>
		public static unsafe uint HIBYTE(this uint num, byte value)
		{
			*((byte*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) byte of the 8-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) byte of the 8-byte
		/// signed integer.</param>
		/// <returns>New 8-byte signed integer with replaced higher (second) byte.</returns>
		public static unsafe long HIBYTE(this long num, byte value)
		{
			*((byte*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) byte of the 8-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) byte of the 8-byte
		/// unsigned integer.</param>
		/// <returns>New 8-byte unsigned integer with replaced higher (second) byte.</returns>
		public static unsafe ulong HIBYTE(this ulong num, byte value)
		{
			*((byte*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) 2 bytes of the 4-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) 2 bytes of the 4-byte
		/// signed integer.</param>
		/// <returns>New 4-byte signed integer with replaced lower (first) 2 bytes.</returns>
		public static unsafe int LOWORD(this int num, short value)
		{
			*(short*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) 2 bytes of the 4-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) 2 bytes of the 4-byte
		/// unsigned integer.</param>
		/// <returns>New 4-byte unsigned integer with replaced lower (first) 2 bytes.</returns>
		public static unsafe uint LOWORD(this uint num, short value)
		{
			*(short*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) 2 bytes of the 8-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) 2 bytes of the 8-byte
		/// signed integer.</param>
		/// <returns>New 8-byte signed integer with replaced lower (first) 2 bytes.</returns>
		public static unsafe long LOWORD(this long num, short value)
		{
			*(short*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) 2 bytes of the 8-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) 2 bytes of the 8-byte
		/// unsigned integer.</param>
		/// <returns>New 8-byte unsigned integer with replaced lower (first) 2 bytes.</returns>
		public static unsafe ulong LOWORD(this ulong num, short value)
		{
			*(short*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) 2 bytes of the 4-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) 2 bytes of the 4-byte
		/// signed integer.</param>
		/// <returns>New 4-byte signed integer with replaced higher (second) 2 bytes.</returns>
		public static unsafe int HIWORD(this int num, short value)
		{
			*((short*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) 2 bytes of the 4-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) 2 bytes of the 4-byte
		/// unsigned integer.</param>
		/// <returns>New 4-byte unsigned integer with replaced higher (second) 2 bytes.</returns>
		public static unsafe uint HIWORD(this uint num, short value)
		{
			*((short*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) 2 bytes of the 8-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) 2 bytes of the 8-byte
		/// signed integer.</param>
		/// <returns>New 8-byte signed integer with replaced higher (second) 2 bytes.</returns>
		public static unsafe long HIWORD(this long num, short value)
		{
			*((short*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) 2 bytes of the 8-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) 2 bytes of the 8-byte
		/// unsigned integer.</param>
		/// <returns>New 8-byte unsigned integer with replaced higher (second) 2 bytes.</returns>
		public static unsafe ulong HIWORD(this ulong num, short value)
		{
			*((short*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) 4 bytes of the 8-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) 4 bytes of the 8-byte
		/// signed integer.</param>
		/// <returns>New 8-byte signed integer with replaced lower (first) 4 bytes.</returns>
		public static unsafe long LODWORD(this long num, int value)
		{
			*(int*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes lower (first) 4 bytes of the 8-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces lower (first) 4 bytes of the 8-byte
		/// unsigned integer.</param>
		/// <returns>New 8-byte unsigned integer with replaced lower (first) 4 bytes.</returns>
		public static unsafe ulong LODWORD(this ulong num, int value)
		{
			*(int*)&num = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) 4 bytes of the 8-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) 4 bytes of the 8-byte
		/// signed integer.</param>
		/// <returns>New 8-byte signed integer with replaced higher (second) 4 bytes.</returns>
		public static unsafe long HIDWORD(this long num, int value)
		{
			*((int*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes higher (second) 4 bytes of the 8-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces higher (second) 4 bytes of the 8-byte
		/// nnsigned integer.</param>
		/// <returns>New 8-byte unsigned integer with replaced higher (second) 4 bytes.</returns>
		public static unsafe ulong HIDWORD(this ulong num, int value)
		{
			*((int*)&num + 1) = value;
			return num;
		}

		/// <summary>
		/// Changes n-th byte of the 4-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces n-th byte of the 4-byte signed integer.</param>
		/// <param name="pos">Position of the byte to replace.</param>
		/// <returns>New 4-byte signed integer with replaced n-th byte.</returns>
		public static unsafe int BYTEn(this int num, byte value, int pos)
		{
			*((byte*)&num + pos) = value;
			return num;
		}

		/// <summary>
		/// Changes n-th byte of the 4-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces n-th byte of the 4-byte unsigned integer.</param>
		/// <param name="pos">Position of the byte to replace.</param>
		/// <returns>New 4-byte unsigned integer with replaced n-th byte.</returns>
		public static unsafe uint BYTEn(this uint num, byte value, int pos)
		{
			*((byte*)&num + pos) = value;
			return num;
		}

		/// <summary>
		/// Changes n-th byte of the 8-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces n-th byte of the 8-byte signed integer.</param>
		/// <param name="pos">Position of the byte to replace.</param>
		/// <returns>New 8-byte signed integer with replaced n-th byte.</returns>
		public static unsafe long BYTEn(this long num, byte value, int pos)
		{
			*((byte*)&num + pos) = value;
			return num;
		}

		/// <summary>
		/// Changes n-th byte of the 8-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces n-th byte of the 8-byte unsigned integer.</param>
		/// <param name="pos">Position of the byte to replace.</param>
		/// <returns>New 8-byte unsigned integer with replaced n-th byte.</returns>
		public static unsafe ulong BYTEn(this ulong num, byte value, int pos)
		{
			*((byte*)&num + pos) = value;
			return num;
		}

		/// <summary>
		/// Changes n-th 2 bytes of the 8-byte signed integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces n-th 2 bytes of the 8-byte
		/// signed integer.</param>
		/// <param name="pos">Position of the 2 bytes to replace.</param>
		/// <returns>New 8-byte signed integer with replaced n-th 2 bytes.</returns>
		public static unsafe long WORDn(this long num, short value, int pos)
		{
			*((short*)&num + pos) = value;
			return num;
		}

		/// <summary>
		/// Changes n-th 2 bytes of the 8-byte unsigned integer.
		/// </summary>
		/// <param name="num"></param>
		/// <param name="value">Value that replaces n-th 2 bytes of the 8-byte
		/// unsigned integer.</param>
		/// <param name="pos">Position of the 2 bytes to replace.</param>
		/// <returns>New 8-byte unsigned integer with replaced n-th 2 bytes.</returns>
		public static unsafe ulong WORDn(this ulong num, short value, int pos)
		{
			*((short*)&num + pos) = value;
			return num;
		}
	}
}
