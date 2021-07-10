using System.IO;
using Hypercooled.Shared.Core;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;

namespace Hypercooled.Shared.Parsers
{
	public class VisibleSectionUserInfoParser : IParser
	{
		private BinBlock m_block;
		private VisibleSectionUserInfo m_userInfo;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.VisibleSectionUserInfo;

		public VisibleSectionUserInfo UserInfo => this.m_userInfo;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;

			br.BaseStream.Position = this.m_block.Start;
			this.m_userInfo = VisibleSectionUserInfo.Deserialize(br);
		}
	}
}
