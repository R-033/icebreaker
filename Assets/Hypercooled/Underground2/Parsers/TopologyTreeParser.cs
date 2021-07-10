using System.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Underground2.Core;

namespace Hypercooled.Underground2.Parsers
{
	public class TopologyTreeParser : IParser
	{
		private BinBlock m_block;
		private TopologyManager m_topologyTree;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TopologyTree;

		public TopologyManager Topology => this.m_topologyTree;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			this.m_topologyTree = new TopologyManager();
			br.BaseStream.Position = this.m_block.Offset;

			this.m_topologyTree.Load(br);
		}
	}
}
