using System.Collections.Generic;
using System.IO;
using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Structures;

namespace Hypercooled.Shared.Parsers
{
	public class TrackStreamingBarrierParser : IParser
	{
		private BinBlock m_block;
		private List<TrackStreamingBarrier> m_tsbMap;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TrackStreamingBarriers;

		public List<TrackStreamingBarrier> TrackStreamingBarrierMap => this.m_tsbMap;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			var count = this.m_block.Size / TrackStreamingBarrier.SizeOf;
			this.m_tsbMap = new List<TrackStreamingBarrier>(count);
			br.BaseStream.Position = this.m_block.Start;

			for (int i = 0; i < count; ++i)
			{
				var data = br.ReadUnmanaged<TrackStreamingBarrier>();
				this.m_tsbMap.Add(data);
			}
		}
	}
}
