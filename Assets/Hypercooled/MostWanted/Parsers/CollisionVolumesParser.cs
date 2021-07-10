using System.IO;
using System.Collections.Generic;
using Hypercooled.MostWanted.Core;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;

namespace Hypercooled.MostWanted.Parsers
{
	public class CollisionVolumesParser : IParser
	{
		private BinBlock m_block;
		private List<CollisionObject> m_collisionObjects;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.CollisionVolumes;

		public List<CollisionObject> CollisionObjects => this.m_collisionObjects;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			this.m_collisionObjects = new List<CollisionObject>();
			br.BaseStream.Position = this.m_block.Start;

			while (br.BaseStream.Position < this.m_block.End)
			{
				var collision = new CollisionObject();
				collision.Deserialize(br);
				this.m_collisionObjects.Add(collision);
			}
		}
	}
}