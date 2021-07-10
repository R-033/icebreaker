using CoreExtensions.IO;
using System.IO;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.World
{
	public class VVGridIsland : VVResolvable // cl
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Data
		{
			public ushort ID;
			public ushort OriginRow;
			public ushort OriginColumn;
			public ushort Width;
			public ushort Height;

			public const int SizeOf = 0x0A;
		}

		public override UppleUTag Tag => UppleUTag.WGridIsland;
		public override int SizeOfThis => Data.SizeOf;
		public override bool DynamicSize => false;

		public ushort ID { get; set; }
		public ushort OriginRow { get; set; }
		public ushort OriginColumn { get; set; }
		public ushort Width { get; set; }
		public ushort Height { get; set; }

		public override void Read(BinaryReader br)
		{
			var data = br.ReadUnmanaged<Data>();

			this.ID = data.ID;
			this.OriginRow = data.OriginRow;
			this.OriginColumn = data.OriginColumn;
			this.Width = data.Width;
			this.Height = data.Height;
		}

		public override void Write(BinaryWriter bw)
		{
			var data = new Data()
			{
				ID = this.ID,
				OriginRow = this.OriginRow,
				OriginColumn = this.OriginColumn,
				Width = this.Width,
				Height = this.Height,
			};

			bw.WriteUnmanaged(data);
		}
	}
}
