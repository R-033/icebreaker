using CoreExtensions.Native;
using System.IO;
using System.Text;

namespace CoreExtensions.IO
{
	/// <summary>
	/// Reads primitive data types as binary values in a specific encoding using big-endian methods.
	/// </summary>
	public class BigEndianBinaryReader : BinaryReader
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BigEndianBinaryReader"/> class based on
		/// the specified stream and using UTF-8 encoding.
		/// </summary>
		/// <param name="input">The input stream.</param>
		public BigEndianBinaryReader(Stream input) : base(input, Encoding.BigEndianUnicode) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BigEndianBinaryReader"/> class based on
		/// the specified stream and using UTF-8 encoding.
		/// </summary>
		/// <param name="input">The input stream.</param>
		/// <param name="encoding">The character encoding to use.</param>
		public BigEndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BigEndianBinaryReader"/> class based on
		/// the specified stream and character encoding, and optionally leaves the stream open.
		/// </summary>
		/// <param name="input">The input stream.</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <param name="leaveOpen">true to leave the stream open after the
		/// <see cref="BigEndianBinaryReader"/> object is disposed; otherwise, false.</param>
		public BigEndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen) :
			base(input, encoding, leaveOpen)
		{ }

		/// <summary>
		/// Reads a 2-byte signed integer from the current stream using big-endian encoding
		/// and advances the position of the stream by two bytes.
		/// </summary>
		/// <returns>A 2-byte signed integer read from the current stream.</returns>
		public override short ReadInt16() => base.ReadInt16().Reverse();

		/// <summary>
		/// Reads a 2-byte unsigned integer from the current stream using big-endian encoding
		/// and advances the position of the stream by two bytes.
		/// </summary>
		/// <returns>A 2-byte unsigned integer read from this stream.</returns>
		public override ushort ReadUInt16() => base.ReadUInt16().Reverse();

		/// <summary>
		/// Reads a 4-byte signed integer from the current stream using big-endian encoding
		/// and advances the position of the stream by four bytes.
		/// </summary>
		/// <returns>A 4-byte signed integer read from the current stream.</returns>
		public override int ReadInt32() => base.ReadInt32().Reverse();

		/// <summary>
		/// Reads a 4-byte unsigned integer from the current stream using big-endian encoding
		/// and advances the position of the stream by four bytes.
		/// </summary>
		/// <returns></returns>
		public override uint ReadUInt32() => base.ReadUInt32().Reverse();

		/// <summary>
		/// Reads an 8-byte signed integer from the current stream using big-endian encoding
		/// and advances the position of the stream by eight bytes.
		/// </summary>
		/// <returns>An 8-byte signed integer read from this stream.</returns>
		public override long ReadInt64() => base.ReadInt64().Reverse();

		/// <summary>
		/// Reads an 8-byte unsigned integer from the current stream using big-endian encoding
		/// and advances the position of the stream by eight bytes.
		/// </summary>
		/// <returns>An 8-byte unsigned integer read from this stream.</returns>
		public override ulong ReadUInt64() => base.ReadUInt64().Reverse();

		/// <summary>
		/// Reads a 4-byte floating point value from the current stream using big-endian encoding
		/// and advances the position of the stream by four bytes.
		/// </summary>
		/// <returns>A 4-byte floating point value read from the current stream.</returns>
		public override float ReadSingle() => base.ReadSingle().Reverse();

		/// <summary>
		/// Reads an 8-byte floating point value from the current stream using big-endian encoding
		/// and advances the position of the stream by eight bytes.
		/// </summary>
		/// <returns>An 8-byte floating point value read from the current stream.</returns>
		public override double ReadDouble() => base.ReadDouble().Reverse();

		/// <summary>
		/// Reads an decimal value from the current stream using big-endian encoding
		/// and advances the position of the stream by sixteen bytes.
		/// </summary>
		/// <returns>A decimal value read from the current stream.</returns>
		public override decimal ReadDecimal() => base.ReadDecimal().Reverse();
	}
}
