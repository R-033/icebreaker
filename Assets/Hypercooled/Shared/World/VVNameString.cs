using CoreExtensions.IO;
using System;
using System.IO;

namespace Hypercooled.Shared.World
{
	public class VVNameString : VVResolvable
	{
		public override UppleUTag Tag => UppleUTag.Name;
		public override int SizeOfThis => this.InternalGetSizeOfThis();
		public override bool DynamicSize => true;

		public string Name { get; set; }

		public VVNameString()
		{
			this.Name = String.Empty;
		}

		public override void Read(BinaryReader br)
		{
			this.Name = br.ReadNullTermUTF8();
		}

		public override void Write(BinaryWriter bw)
		{
			bw.WriteNullTermUTF8(this.Name);
		}

		private int InternalGetSizeOfThis()
		{
			if (String.IsNullOrEmpty(this.Name))
			{
				return 1;
			}
			else
			{
				return this.Name.Length + 1;
			}
		}
	}
}
