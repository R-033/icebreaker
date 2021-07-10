using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;
using Hypercooled.Underground2.MapStream;
using System.Collections.Generic;
using System.IO;

namespace Hypercooled.Underground2.Parsers
{
	public class TopologyBarrierGroupParser : IParser
	{
		private BinBlock m_block;
		private List<TopologyBarrierGroup> m_tbgList;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TopologyBarrierGroups;

		public List<TopologyBarrierGroup> BarrierGroups => this.m_tbgList;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			var count = this.m_block.Size / TopologyBarrierGroup.SizeOf;
			this.m_tbgList = new List<TopologyBarrierGroup>(count);
			br.BaseStream.Position = this.m_block.Start;

			for (int i = 0; i < count; ++i)
			{
				var data = br.ReadStruct<TopologyBarrierGroup>();
				var name = Hashing.ResolveBinEqual(data.Key, data.Name);
				if (data.Key == 0) name = data.Name; // exception

				data.Name = name;
				this.m_tbgList.Add(data);
			}
		}
	}
}
