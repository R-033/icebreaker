using System.Collections.Generic;
using System.IO;
using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;

namespace Hypercooled.Shared.Parsers
{
	public class TrackObjectBoundsParser : IParser
	{
		private BinBlock m_block;
		private List<TrackObjectBounds> m_trackOOBMap;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TrackObjectBounds;

		public List<TrackObjectBounds> TrackOOBs => this.m_trackOOBMap;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;

			br.BaseStream.Position = this.m_block.Start;
			br.AlignReaderPow2(0x10);
			var size = this.m_block.Size - ((int)(br.BaseStream.Position - this.m_block.Start));

			var count = size / TrackObjectBounds.SizeOf;
			this.m_trackOOBMap = new List<TrackObjectBounds>(count);

			for (int i = 0; i < count; ++i)
			{
				var data = br.ReadStruct<TrackObjectBounds>();
				this.m_trackOOBMap.Add(data);
			}
		}
	}
}
