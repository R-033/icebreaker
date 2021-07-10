using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Underground2.MapStream;
using Hypercooled.Utils;
using System.Collections.Generic;
using System.IO;

namespace Hypercooled.Underground2.Parsers
{
	public class AcidEmitterParser : IParser
	{
		private BinBlock m_block;
		private List<AcidEmitter> m_acidEmitters;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.AcidEmitters;

		public List<AcidEmitter> AcidEmitters => this.m_acidEmitters;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			this.m_acidEmitters = new List<AcidEmitter>();
			br.BaseStream.Position = this.m_block.Start;

			br.AlignReaderPow2(0x10);
			var header = br.ReadUnmanaged<AcidEmitterPackHeader>();

			for (int i = 0; i < header.NumEmitters; ++i)
			{
				var emitter = br.ReadStruct<AcidEmitter>();

				emitter.Padding0 = 0;
				emitter.Name = Hashing.ResolveBinEqual(emitter.NameHash, emitter.Name);
				emitter.TextureName = Hashing.ResolveBinEqual(emitter.TextureNameHash, emitter.TextureName);
				emitter.GroupName = Hashing.ResolveBinEqual(emitter.GroupNameHash, emitter.GroupName);

				this.m_acidEmitters.Add(emitter);
			}
		}
	}
}
