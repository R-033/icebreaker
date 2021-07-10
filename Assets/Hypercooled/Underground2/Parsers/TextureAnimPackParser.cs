using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Underground2.Core;
using System.IO;

namespace Hypercooled.Underground2.Parsers
{
	public class TextureAnimPackParser : IParser
	{
		private BinBlock m_block;
		private AnimContainer m_container;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TextureAnimPack;

		public AnimContainer Container => this.m_container;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;

			br.BaseStream.Position = this.m_block.Offset;
			this.m_container = new Core.AnimContainer();
			this.m_container.Load(br);
		}
	}
}
