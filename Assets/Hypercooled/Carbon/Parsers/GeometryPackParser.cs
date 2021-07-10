using Hypercooled.Carbon.Core;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using System.IO;

namespace Hypercooled.Carbon.Parsers
{
	public class GeometryPackParser : IParser
	{
		private BinBlock m_block;
		private MeshContainer m_container;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.GeometryPack;

		public MeshContainer Container => this.m_container;

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

			this.m_container = new MeshContainer();
			this.m_container.Load(br);
		}
	}
}
