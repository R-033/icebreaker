using CoreExtensions.IO;
using Hypercooled.Shared.Core;
using System.IO;

namespace Hypercooled.Utils
{
	public static class BinaryHelper
	{
		public static void AlignReader(this BinaryReader br, int alignment, long absOffset = 0)
		{
			var offset = absOffset + br.BaseStream.Position;
			var diff = alignment - (offset % alignment);
			if (diff != alignment) br.BaseStream.Position += diff;
		}

		public static void AlignReaderPow2(this BinaryReader br, int alignment, long absOffset = 0)
		{
			var offset = absOffset + br.BaseStream.Position;
			var diff = alignment - (offset & (alignment - 1));
			if (diff != alignment) br.BaseStream.Position += diff;
		}

		public static void AlignWriter(this BinaryWriter bw, int alignment, long absOffset = 0)
		{
			var offset = absOffset + bw.BaseStream.Position;
			var diff = alignment - (offset % alignment);
			if (diff == alignment) diff = 0;
			for (int i = 0; i < diff; ++i) bw.Write((byte)0x11);
		}

		public static void AlignWriterPow2(this BinaryWriter bw, int alignment, long absOffset = 0)
		{
			var offset = absOffset + bw.BaseStream.Position;
			var diff = alignment - (offset & (alignment - 1));
			if (diff == alignment) diff = 0;
			for (int i = 0; i < diff; ++i) bw.Write((byte)0x11);
		}

		public static long SeekBlockWithID(this BinaryReader br, uint id, long start, long length)
		{
			var current = br.BaseStream.Position;
			br.BaseStream.Position = start;

			while (br.BaseStream.Position < start + length)
			{
				var offset = br.BaseStream.Position;
				var bid = br.ReadUInt32();
				var size = br.ReadInt32();

				if (bid == id)
				{
					br.BaseStream.Position = current;
					return offset;
				}

				br.BaseStream.Position += size;
			}

			br.BaseStream.Position = current;
			return -1;
		}

		public static void GeneratePadding(this BinaryWriter bw, int alignment)
		{
			var start = bw.BaseStream.Position;
			var difference = alignment - ((start + 0x50) % alignment);

			if (difference == alignment) difference = 0;

			var size = difference + 0x50;
			var end = start + size;

			bw.Write(0);
			bw.Write((int)(size - 8));
			bw.Write(Namings.Hypercooled);
			bw.Write(0);

			bw.WriteNullTermUTF8(Namings.Padding, 0x20);
			bw.WriteNullTermUTF8(Namings.Watermark, 0x20);

			while (bw.BaseStream.Position < end) bw.Write((byte)0);
		}

		public static void GeneratePaddingPow2(this BinaryWriter bw, int alignment)
		{
			var start = bw.BaseStream.Position;
			var difference = alignment - ((start + 0x50) & (alignment - 1));

			if (difference == alignment) difference = 0;

			var size = difference + 0x50;
			var end = start + size;

			bw.Write(0);
			bw.Write((int)(size - 8));
			bw.Write(Namings.Hypercooled);
			bw.Write(0);

			bw.WriteNullTermUTF8(Namings.Padding, 0x20);
			bw.WriteNullTermUTF8(Namings.Watermark, 0x20);

			while (bw.BaseStream.Position < end) bw.Write((byte)0);
		}
	}
}
