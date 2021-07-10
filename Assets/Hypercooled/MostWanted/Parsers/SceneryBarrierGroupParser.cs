using Hypercooled.MostWanted.MapStream;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;
using System.Collections.Generic;
using System.IO;

namespace Hypercooled.MostWanted.Parsers
{
	public class SceneryBarrierGroupParser : IParser
	{
		private BinBlock m_block;
		private List<SceneryBarrierGroup> m_sbg;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.SceneryBarrierGroups;

		public List<SceneryBarrierGroup> SceneryBarrierGroups => m_sbg;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			this.m_sbg = new List<SceneryBarrierGroup>();
			br.BaseStream.Position = this.m_block.Start;

			// todo : we should not do this lol
			"SMOKEABLE".BinHash();

			for (int i = 0; i < 10000; ++i)
			{
				$"BARRIER_SPLINE_{i}".BinHash();
			}

			while (br.BaseStream.Position < this.m_block.End)
			{
				var group = new SceneryBarrierGroup();
				group.Read(br);
				br.AlignReaderPow2(0x04);
				this.m_sbg.Add(group);
			}
		}
	}
}
