using System.Collections.Generic;
using System.IO;
using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Underground2.MapStream;
using Hypercooled.Utils;

namespace Hypercooled.Underground2.Parsers
{
	public class AcidEffectParser : IParser
	{
		private BinBlock m_block;
		private List<AcidEffect> m_acidEffects;
		private ushort m_sectionNumber;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.AcidEffects;

		public List<AcidEffect> AcidEffects => this.m_acidEffects;
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
			this.m_acidEffects = new List<AcidEffect>();
			br.BaseStream.Position = this.m_block.Start;

			br.AlignReaderPow2(0x10);
			var header = br.ReadUnmanaged<AcidEffectPackHeader>();
			this.m_sectionNumber = (ushort)header.SectionNumber;

			for (int i = 0; i < header.NumEffects; ++i)
			{
				var effect = br.ReadStruct<AcidEffect>();
				effect.Padding = 0;
				this.m_acidEffects.Add(effect);
			}
		}
	}
}
