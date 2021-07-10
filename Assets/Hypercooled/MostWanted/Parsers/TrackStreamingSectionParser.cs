using System.Collections.Generic;
using System.IO;
using CoreExtensions.IO;
using Hypercooled.MostWanted.MapStream;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;

namespace Hypercooled.MostWanted.Parsers
{
	public class TrackStreamingSectionParser : IParser
	{
		private BinBlock m_block;
		private List<TrackStreamingSection> m_tssMap;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TrackStreamingSections;

		public List<TrackStreamingSection> TrackStreamingSectionMap => this.m_tssMap;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			var count = this.m_block.Size / TrackStreamingSection.SizeOf;
			this.m_tssMap = new List<TrackStreamingSection>(count);
			br.BaseStream.Position = this.m_block.Start;

			for (int i = 0; i < count; ++i)
			{
				var data = br.ReadStruct<TrackStreamingSection>();
				this.m_tssMap.Add(data);
			}
		}
	}
}
