using CoreExtensions.Native;
using System.IO;
using System.Text;

namespace CoreExtensions.IO
{
	/// <summary>
	/// Writes primitive types in binary to a stream and supports writing strings in
	/// a specific encoding using big-endian methods.
	/// </summary>
	public class BigEndianBinaryWriter : BinaryWriter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BigEndianBinaryWriter"/> class based on
		/// the specified stream and using UTF-8 encoding.
		/// </summary>
		/// <param name="output">The output stream.</param>
		public BigEndianBinaryWriter(Stream output) : base(output, Encoding.BigEndianUnicode) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BigEndianBinaryWriter"/> class based on
		/// the specified stream and character encoding.
		/// </summary>
		/// <param name="output">The output stream.</param>
		/// <param name="encoding">The character encoding to use.</param>
		public BigEndianBinaryWriter(Stream output, Encoding encoding) : base(output, encoding) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="BigEndianBinaryWriter"/> class based on
		/// the specified stream and character encoding, and optionally leaves the stream open.
		/// </summary>
		/// <param name="output">The output stream.</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <param name="leaveOpen">true to leave the stream open after the
		/// <see cref="BigEndianBinaryWriter"/> object is disposed; otherwise, false.</param>
		public BigEndianBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) :
			base(output, encoding, leaveOpen)
		{ }

		/// <summary>
		/// Writes a two-byte signed integer to the current stream using big-endian encoding
		/// and advances the stream position by two bytes.
		/// </summary>
		/// <param name="value">The two-byte signed integer to write.</param>
		public override void Write(short value) => base.Write(value.Reverse());

		/// <summary>
		/// Writes a two-byte unsigned integer to the current stream using big-endian encoding
		/// and advances the stream position by two bytes.
		/// </summary>
		/// <param name="value">The two-byte unsigned integer to write.</param>
		public override void Write(ushort value) => base.Write(value.Reverse());

		/// <summary>
		/// Writes a four-byte signed integer to the current stream using big-endian encoding
		/// and advances the stream position by four bytes.
		/// </summary>
		/// <param name="value">The four-byte signed integer to write.</param>
		public override void Write(int value) => base.Write(value.Reverse());

		/// <summary>
		/// Writes a four-byte unsigned integer to the current stream using big-endian encoding
		/// and advances the stream position by four bytes.
		/// </summary>
		/// <param name="value">The four-byte unsigned integer to write.</param>
		public override void Write(uint value) => base.Write(value.Reverse());

		/// <summary>
		/// Writes an eight-byte signed integer to the current stream using big-endian encoding
		/// and advances the stream position by eight bytes.
		/// </summary>
		/// <param name="value">The eight-byte signed integer to write.</param>
		public override void Write(long value) => base.Write(value.Reverse());

		/// <summary>
		/// Writes an eight-byte unsigned integer to the current stream using big-endian encoding
		/// and advances the stream position by eight bytes.
		/// </summary>
		/// <param name="value">The eight-byte unsigned integer to write.</param>
		public override void Write(ulong value) => base.Write(value.Reverse());

		/// <summary>
		/// Writes a four-byte floating-point value to the current stream using
		/// big-endian encodingand advances the stream position by four bytes.
		/// </summary>
		/// <param name="value">The four-byte floating-point value to write.</param>
		public override void Write(float value) => base.Write(value.Reverse());

		/// <summary>
		/// Writes an eight-byte floating-point value to the current stream using
		/// big-endian encoding and advances the stream position by eight bytes.
		/// </summary>
		/// <param name="value">The eight-byte floating-point value to write.</param>
		public override void Write(double value) => base.Write(value.Reverse());

		/// <summary>
		/// Writes a decimal value to the current stream using big-endian encoding
		/// and advances the stream position by sixteen bytes.
		/// </summary>
		/// <param name="value">The decimal value to write.</param>
		public override void Write(decimal value) => base.Write(value.Reverse());
	}
}
