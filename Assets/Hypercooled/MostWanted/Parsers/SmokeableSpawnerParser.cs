using System.Collections.Generic;
using System.IO;
using CoreExtensions.IO;
using Hypercooled.MostWanted.MapStream;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;

namespace Hypercooled.MostWanted.Parsers
{
	public class SmokeableSpawnerParser : IParser
	{
		private BinBlock m_block;
		private List<SmokeableSpawner> m_smokeableSpawners;
		private ushort m_sectionNumber;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.SmokeableSpawners;

		public List<SmokeableSpawner> SmokeableSpawners => this.m_smokeableSpawners;
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
			this.m_smokeableSpawners = new List<SmokeableSpawner>();
			br.BaseStream.Position = this.m_block.Start;

			br.AlignReaderPow2(0x10);
			var header = br.ReadUnmanaged<SmokeableSpawnHeader>();
			this.m_sectionNumber = header.SectionNumber;

			for (int i = 0; i < header.NumSpawners; ++i)
			{
				var spawner = br.ReadUnmanaged<SmokeableSpawner>();
				this.m_smokeableSpawners.Add(spawner);
			}
		}
	}
}
