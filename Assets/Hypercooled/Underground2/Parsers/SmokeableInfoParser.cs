using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Underground2.MapStream;
using Hypercooled.Utils;
using System.Collections.Generic;
using System.IO;

namespace Hypercooled.Underground2.Parsers
{
	public class SmokeableInfoParser : IParser
	{
		private BinBlock m_block;
		private List<SmokeableInfo> m_smokeableInfos;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.SmokeableInfos;

		public List<SmokeableInfo> SmokeableInfos => this.m_smokeableInfos;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			this.m_smokeableInfos = new List<SmokeableInfo>();
			br.BaseStream.Position = this.m_block.Start;

			br.AlignReaderPow2(0x10);
			int count = this.m_block.Size / 0x100;

			for (int i = 0; i < count; ++i)
			{
				var info = br.ReadStruct<SmokeableInfo>();
				info.Name = Hashing.ResolveBinEqual(info.Key, info.Name);
				this.m_smokeableInfos.Add(info);
			}
		}
	}
}
