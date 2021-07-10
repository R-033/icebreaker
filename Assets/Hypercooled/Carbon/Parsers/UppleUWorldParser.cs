using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using System.IO;

namespace Hypercooled.Carbon.Parsers
{
	public class UppleUWorldParser : IParser
	{
		private BinBlock m_block;
		

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.UppleUWorld;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			// todo
		}
	}
}
