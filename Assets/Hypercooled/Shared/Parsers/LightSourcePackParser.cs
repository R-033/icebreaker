using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;
using System.Collections.Generic;
using System.IO;

namespace Hypercooled.Shared.Parsers
{
	public class LightSourcePackParser : IParser
	{
		private BinBlock m_block;
		private List<LightSource> m_lightSources;
		private ushort m_sectionNumber;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.LightSourcesPack;

		public List<LightSource> LightSources => m_lightSources;
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
			this.m_lightSources = new List<LightSource>();
			br.BaseStream.Position = this.m_block.Start;

			while (br.BaseStream.Position < this.m_block.End)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;

				if (id == BinBlockID.LightSourcePackHeader)
				{
					var header = br.ReadUnmanaged<LightSourcePackHeader>();
					this.m_sectionNumber = (ushort)header.ScenerySectionNumber;
				}
				else if (id == BinBlockID.LightSources)
				{
					br.AlignReaderPow2(0x10);
					var count = (size - (int)(br.BaseStream.Position - offset)) / LightSource.SizeOf;
					for (int i = 0; i < count; ++i) this.m_lightSources.Add(br.ReadStruct<LightSource>());
				}

				br.BaseStream.Position = offset + size;
			}
		}
	}
}
