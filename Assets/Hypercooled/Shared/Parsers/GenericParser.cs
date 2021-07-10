using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using System.IO;

namespace Hypercooled.Shared.Parsers
{
	public class GenericParser : IParser
	{
		private static byte[] ms_emptyData = new byte[0];

		private BinBlock m_block;
		private byte[] m_data;
		private uint m_id;

		/// <inheritdoc/>
		public uint BlockID => this.m_id;

		public byte[] Data => this.m_data;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
			this.m_id = block.ID;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			br.BaseStream.Position = this.m_block.Offset;

			this.m_data = this.m_block.Size == 0
				? GenericParser.ms_emptyData
				: br.ReadBytes(this.m_block.Size + 8);
		}
	}
}
