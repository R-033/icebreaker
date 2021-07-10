using System.Collections.Generic;
using System.IO;
using CoreExtensions.IO;
using Hypercooled.MostWanted.MapStream;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;

namespace Hypercooled.MostWanted.Parsers
{
	public class SceneryOverrideInfoParser : IParser
	{
		private BinBlock m_block;
		private List<SceneryOverrideInfo> m_soiMap;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.SceneryOverrideInfos;

		public List<SceneryOverrideInfo> SceneryOverrideInfoMap => this.m_soiMap;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			var count = this.m_block.Size / SceneryOverrideInfo.SizeOf;
			this.m_soiMap = new List<SceneryOverrideInfo>(count);
			br.BaseStream.Position = this.m_block.Start;

			for (int i = 0; i < count; ++i)
			{
				var data = br.ReadUnmanaged<SceneryOverrideInfo>();
				this.m_soiMap.Add(data);
			}
		}
	}
}
