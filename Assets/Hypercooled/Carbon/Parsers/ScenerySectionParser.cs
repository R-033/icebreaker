using System.IO;
using CoreExtensions.IO;
using Hypercooled.Carbon.MapStream;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;

namespace Hypercooled.Carbon.Parsers
{
	public class ScenerySectionParser : IParser
	{
		private BinBlock m_block;
		private ScenerySection m_scenerySection;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.ScenerySection;

		public ScenerySection Scenery => this.m_scenerySection;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			this.m_scenerySection = new ScenerySection();
			br.BaseStream.Position = this.m_block.Start;

			while (br.BaseStream.Position < this.m_block.End)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;

				switch (id)
				{
					case BinBlockID.ScenerySectionHeader: this.ReadScenerySectionHeader(br); break;
					case BinBlockID.SceneryInfos: this.ReadSceneryInfos(br, size); break;
					case BinBlockID.SceneryInstances: this.ReadSceneryInstances(br, size); break;
					case BinBlockID.SceneryTreeNodes: this.ReadSceneryTreeNodes(br, size); break;
					case BinBlockID.SceneryOverrideHooks: this.ReadSceneryOverrideHooks(br, size); break;
					case BinBlockID.PrecullerInfos: this.ReadPrecullerInfos(br, size); break;
					case BinBlockID.LightTextureCollections: this.ReadLightTextureCollections(br, size); break;
					default: break;
				}

				br.BaseStream.Position = offset + size;
			}
		}

		private void ReadScenerySectionHeader(BinaryReader br)
		{
			br.AlignReaderPow2(0x10);
			this.m_scenerySection.SetScenerySectionHeader(br.ReadUnmanaged<ScenerySectionHeader>());
		}

		private void ReadSceneryInfos(BinaryReader br, int size)
		{
			var count = size / SceneryInfo.SizeOf;
			this.m_scenerySection.SceneryInfos.Capacity = count;

			for (int i = 0; i < count; ++i)
			{
				this.m_scenerySection.SceneryInfos.Add(br.ReadStruct<SceneryInfo>());
			}
		}

		private void ReadSceneryInstances(BinaryReader br, int size)
		{
			var current = br.BaseStream.Position;
			br.AlignReaderPow2(0x10);
			size -= (int)(br.BaseStream.Position - current);

			var count = size / SceneryInstance.SizeOf;
			this.m_scenerySection.SceneryInstances.Capacity = count;

			for (int i = 0; i < count; ++i)
			{
				this.m_scenerySection.SceneryInstances.Add(br.ReadUnmanaged<SceneryInstance>());
			}
		}

		private void ReadSceneryTreeNodes(BinaryReader br, int size)
		{
			var count = size / SceneryTreeNode.SizeOf;
			this.m_scenerySection.SceneryTreeNodes.Capacity = count;

			for (int i = 0; i < count; ++i)
			{
				this.m_scenerySection.SceneryTreeNodes.Add(br.ReadStruct<SceneryTreeNode>());
			}
		}

		private void ReadSceneryOverrideHooks(BinaryReader br, int size)
		{
			var count = size / Shared.MapStream.SceneryOverrideHook.SizeOf;
			this.m_scenerySection.SceneryOverrideHooks.Capacity = count;

			for (int i = 0; i < count; ++i)
			{
				var sceneryOverrideHook = br.ReadUnmanaged<Shared.MapStream.SceneryOverrideHook>();
				this.m_scenerySection.SceneryOverrideHooks.Add(sceneryOverrideHook);
			}
		}

		private void ReadPrecullerInfos(BinaryReader br, int size)
		{
			var count = size / Shared.MapStream.PrecullerInfo.SizeOf;
			this.m_scenerySection.PrecullerInfos.Capacity = count;

			for (int i = 0; i < count; ++i)
			{
				var precullerInfo = br.ReadStruct<Shared.MapStream.PrecullerInfo>();
				this.m_scenerySection.PrecullerInfos.Add(precullerInfo);
			}
		}

		private void ReadLightTextureCollections(BinaryReader br, int size)
		{
			var count = size / LightTextureCollection.SizeOf;
			this.m_scenerySection.LightTextureCollections.Capacity = count;

			for (int i = 0; i < count; ++i)
			{
				var lightTextureCollection = br.ReadUnmanaged<LightTextureCollection>();
				this.m_scenerySection.LightTextureCollections.Add(lightTextureCollection);
			}
		}
	}
}
