using CoreExtensions.IO;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using System.Collections.Generic;
using System.IO;

namespace Hypercooled.Shared.Parsers
{
	public class ModelHierarchyTreeParser : IParser
	{
		private BinBlock m_block;
		private List<ModelHierarchy> m_hierarchies;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.ModelHierarchyTree;

		public List<ModelHierarchy> Hierarchies => this.m_hierarchies;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;

			this.m_hierarchies = new List<ModelHierarchy>();
			br.BaseStream.Position = this.m_block.Start;

			while (br.BaseStream.Position < this.m_block.End)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;

				if (id == BinBlockID.ModelHierarchy)
				{
					this.ReadSingleHierarchy(br);
				}

				br.BaseStream.Position = offset + size;
			}
		}

		private void ReadSingleHierarchy(BinaryReader br)
		{
			// todo : do we need to align?
			// br.AlignReaderPow2(0x10);

			var header = br.ReadUnmanaged<ModelHierarchyHeader>();

			var hierarchy = new ModelHierarchy()
			{
				Key = header.Key,
			};

			var offset = br.BaseStream.Position;
			hierarchy.Nodes.Add(this.RecursiveModelReader(br, offset));

			this.m_hierarchies.Add(hierarchy);
		}

		private ModelHierarchy.Node RecursiveModelReader(BinaryReader br, long offset)
		{
			var cnode = br.ReadUnmanaged<ModelHierarchyNode>();

			var mnode = new ModelHierarchy.Node()
			{
				ModelName = cnode.ModelName,
				SolidName = cnode.SolidKey,
			};

			var position = offset + cnode.ChildIndex * ModelHierarchyNode.SizeOf;

			for (int i = 0; i < cnode.NumChildren; ++i)
			{
				br.BaseStream.Position = position + i * ModelHierarchyNode.SizeOf;
				mnode.Children.Add(this.RecursiveModelReader(br, offset));
			}

			return mnode;
		}
	}
}
