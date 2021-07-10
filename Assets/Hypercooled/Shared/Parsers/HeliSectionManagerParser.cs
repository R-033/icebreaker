using System.IO;
using Hypercooled.Shared.Core;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;

namespace Hypercooled.Shared.Parsers
{
	public class HeliSectionManagerParser : IParser
	{
		private BinBlock m_block;
		private HeliSectionManager m_heliManager;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.HeliSheetManager;

		public HeliSectionManager HeliManager => this.m_heliManager;

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
			this.m_heliManager = new HeliSectionManager();
			this.m_heliManager.Load(br);
		}
	}
}
