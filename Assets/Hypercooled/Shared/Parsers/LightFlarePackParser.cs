using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;
using System.Collections.Generic;
using System.IO;

namespace Hypercooled.Shared.Parsers
{
	public class LightFlarePackParser : IParser
	{
		private BinBlock m_block;
		private List<LightFlare> m_lightFlares;
		private ushort m_sectionNumber;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.LightFlaresPack;

		public List<LightFlare> LightFlares => m_lightFlares;
		public ushort SectionNumber => this.m_sectionNumber;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			this.m_lightFlares = new List<LightFlare>();
			br.BaseStream.Position = this.m_block.Start;

			while (br.BaseStream.Position < this.m_block.End)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;

				if (id == BinBlockID.LightFlarePackHeader)
				{
					var header = br.ReadUnmanaged<LightFlarePackHeader>();
					this.m_sectionNumber = (ushort)header.ScenerySectionNumber;
				}
				else if (id == BinBlockID.LightFlare)
				{
					br.AlignReaderPow2(0x10);
					var count = (size - (int)(br.BaseStream.Position - offset)) / LightSource.SizeOf;
					for (int i = 0; i < count; ++i) this.m_lightFlares.Add(br.ReadUnmanaged<LightFlare>());
				}

				br.BaseStream.Position = offset + size;
			}
		}
	}
}
