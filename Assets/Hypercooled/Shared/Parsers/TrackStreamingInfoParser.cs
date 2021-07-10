using System.Collections.Generic;
using System.IO;
using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Structures;

namespace Hypercooled.Shared.Parsers
{
	public class TrackStreamingInfoParser : IParser
	{
		private BinBlock m_block;
		private List<TrackStreamingInfo> m_tsiMap;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TrackStreamingInfos;

		public List<TrackStreamingInfo> TrackStreamingInfos => this.m_tsiMap;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			var count = this.m_block.Size / TrackStreamingInfo.SizeOf;
			this.m_tsiMap = new List<TrackStreamingInfo>(count);
			br.BaseStream.Position = this.m_block.Start;

			for (int i = 0; i < count; ++i)
			{
				var data = br.ReadUnmanaged<TrackStreamingInfo>();
				this.m_tsiMap.Add(data);
			}
		}
	}
}
