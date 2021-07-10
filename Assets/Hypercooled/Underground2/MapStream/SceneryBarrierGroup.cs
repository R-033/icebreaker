using CoreExtensions.IO;
using Hypercooled.Utils;
using System.IO;

namespace Hypercooled.Underground2.MapStream
{
	public class SceneryBarrierGroup // 0x00034108
	{
		private ushort[] m_overrides;

		public string Name { get; set; }
		public uint Key { get; set; }

		public ushort[] Overrides
		{
			get => this.m_overrides;
			set => this.m_overrides = value is null ? new ushort[0] : value;
		}

		public SceneryBarrierGroup()
		{
			this.m_overrides = new ushort[0];
		}

		public void Read(BinaryReader br)
		{
			br.BaseStream.Position += 8;
			this.Name = br.ReadNullTermUTF8(0x20);
			this.Key = br.ReadUInt32();
			br.BaseStream.Position += 4;
			var count = br.ReadInt32();
			this.m_overrides = new ushort[count];
			this.Name = Hashing.ResolveBinEqual(this.Key, this.Name);

			for (int i = 0; i < count; ++i) m_overrides[i] = br.ReadUInt16();
		}
	}
}
