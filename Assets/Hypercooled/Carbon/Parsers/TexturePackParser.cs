using Hypercooled.Carbon.Core;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using System.IO;

namespace Hypercooled.Carbon.Parsers
{
	public class TexturePackParser : IParser
	{
		private BinBlock m_block;
		private TextureContainer m_container;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.TexturePack;

		public TextureContainer Container => this.m_container;

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

			this.m_container = new TextureContainer();
			this.m_container.Load(br);
		}
	}
}
