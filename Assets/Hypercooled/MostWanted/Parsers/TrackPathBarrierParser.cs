using System.Collections.Generic;
using System.IO;
using CoreExtensions.IO;
using Hypercooled.MostWanted.MapStream;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;

namespace Hypercooled.MostWanted.Parsers
{
	public class TrackPathBarrierParser : IParser
	{
		private BinBlock m_block;
		private List<TrackPathBarrier> m_tpbMap;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TrackPathBarriers;

		public List<TrackPathBarrier> TrackPathBarriers => this.m_tpbMap;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;

			var count = this.m_block.Size / TrackPathBarrier.SizeOf;
			this.m_tpbMap = new List<TrackPathBarrier>(count);
			br.BaseStream.Position = this.m_block.Start;

			for (int i = 0; i < count; ++i)
			{
				var data = br.ReadUnmanaged<TrackPathBarrier>();
				this.m_tpbMap.Add(data);
			}
		}
	}
}
