using CoreExtensions.IO;
using System.IO;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.World
{
	public class VVRoad : VVResolvable // RNrd
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Data
		{
			public ushort Scale;
			public ushort Length;
			public sbyte Shortcut;
			public byte MinWidth;
			public ushort SpeechID;

			public const int SizeOf = 0x08;
		}

		public override UppleUTag Tag => UppleUTag.WRoad;
		public override int SizeOfThis => Data.SizeOf;
		public override bool DynamicSize => false;

		public ushort Scale { get; set; }
		public ushort Length { get; set; }
		public sbyte Shortcut { get; set; }
		public byte MinWidth { get; set; }
		public ushort SpeechID { get; set; }

		public override void Read(BinaryReader br)
		{
			var data = br.ReadUnmanaged<Data>();

			this.Scale = data.Scale;
			this.Length = data.Length;
			this.Shortcut = data.Shortcut;
			this.MinWidth = data.MinWidth;
			this.SpeechID = data.SpeechID;
		}

		public override void Write(BinaryWriter bw)
		{
			var data = new Data()
			{
				Scale = this.Scale,
				Length = this.Length,
				Shortcut = this.Shortcut,
				MinWidth = this.MinWidth,
				SpeechID = this.SpeechID,
			};

			bw.WriteUnmanaged(data);
		}
	}
}
