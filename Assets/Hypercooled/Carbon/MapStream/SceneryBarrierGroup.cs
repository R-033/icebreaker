using Hypercooled.Utils;
using System.IO;

namespace Hypercooled.Carbon.MapStream
{
	public class SceneryBarrierGroup // 0x00034109
	{
		private ushort[] m_overrides;

		public string Name { get; set; }
		public uint Key { get; set; }
		public ushort Index { get; set; }
		public bool Barrier { get; set; }

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
			this.Key = br.ReadUInt32();
			this.Name = this.Key.BinString();
			this.Index = br.ReadUInt16();
			var count = br.ReadUInt16();
			this.Barrier = br.ReadBoolean();
			br.BaseStream.Position += 3;
			this.m_overrides = new ushort[count];

			for (int i = 0; i < count; ++i) m_overrides[i] = br.ReadUInt16();
		}
	}
}
