using System.Collections.Generic;
using System.IO;
using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;

namespace Hypercooled.Shared.Parsers
{
	public class TrackPositionMarkerParser : IParser
	{
		private BinBlock m_block;
		private List<TrackPositionMarker> m_tpmMap;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TrackPositionMarkers;

		public List<TrackPositionMarker> TrackPositionMarkers => this.m_tpmMap;

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

			var count = size / TrackPositionMarker.SizeOf;
			this.m_tpmMap = new List<TrackPositionMarker>(count);

			for (int i = 0; i < count; ++i)
			{
				var data = br.ReadStruct<TrackPositionMarker>();
				data.Padding = 0;
				this.m_tpmMap.Add(data);
			}
		}
	}
}
