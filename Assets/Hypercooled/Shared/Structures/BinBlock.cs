using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Structures
{
	public struct BinBlock
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Info
		{
			public uint ID;
			public int Size;

			public const int SizeOf = 0x08;
		}

		public uint ID { get; }
		public int Size { get; }
		public long Offset { get; }
		public long Start => this.Offset + 8;
		public long End => this.Start + this.Size;

		public static BinBlock Null { get; }

		static BinBlock()
		{
			Null = new BinBlock(default, -1);
		}
		public BinBlock(Info info, long offset)
		{
			this.ID = info.ID;
			this.Size = info.Size;
			this.Offset = offset;
		}

		public override string ToString() => $"Offset: [0x{this.Offset:X8}] | ID: [0x{this.ID:X8}] | Size: [0x{this.Size:X8}]";

		public static BinBlock Empty => new BinBlock(new Info(), 0L);
	}
}
