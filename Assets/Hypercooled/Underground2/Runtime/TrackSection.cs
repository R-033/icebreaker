using Hypercooled.Shared.Interfaces;
using Hypercooled.Utils;
using UnityEngine;

namespace Hypercooled.Underground2.Runtime
{
	public class TrackSection : Shared.Runtime.TrackSection
	{
		public TrackSection(ushort sectionNumber, Shared.Runtime.TrackStreamer streamer) : base(sectionNumber, streamer)
		{
		}

		private void ProcessVisibleSectionUserInfo(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.VisibleSectionUserInfoParser;
			this.UserInfo = parser.UserInfo;
		}
		private void ProcessTexturePack(IParser iparser)
		{
			var parser = iparser as Parsers.TexturePackParser;
			parser.Container.MergeTexturesIntoMap(this.TextureMap);
		}
		private void ProcessGeometryPack(IParser iparser)
		{
			var parser = iparser as Parsers.GeometryPackParser;
			parser.Container.MergeMeshesIntoMap(this.MeshMap);
		}
		private void ProcessTextureAnimPack(IParser iparser)
		{
			var parser = iparser as Parsers.TextureAnimPackParser;
			parser.Container.MergeAnimationsIntoMap(this.AnimationMap);
		}
		private void ProcessTopologyTree(IParser iparser)
		{
			var parser = iparser as Parsers.TopologyTreeParser;
			this.Topology = parser.Topology;
		}

		protected override BlockReader GetAssetBlockReader()
		{
			var br = new BlockReader();

			br.RegisterParser((uint)BinBlockID.VisibleSectionUserInfo, () => new Shared.Parsers.VisibleSectionUserInfoParser());
			br.RegisterParser((uint)BinBlockID.TexturePack, () => new Parsers.TexturePackParser());
			br.RegisterParser((uint)BinBlockID.GeometryPack, () => new Parsers.GeometryPackParser());
			br.RegisterParser((uint)BinBlockID.TextureAnimPack, () => new Parsers.TextureAnimPackParser());
			br.RegisterParser((uint)BinBlockID.TopologyTree, () => new Parsers.TopologyTreeParser());

			return br;
		}
		protected override void ProcessLoadedParsers(BlockReader br)
		{
			foreach (var iparser in br.Parsers)
			{
				switch ((BinBlockID)iparser.BlockID)
				{
					case BinBlockID.VisibleSectionUserInfo: this.ProcessVisibleSectionUserInfo(iparser); break;
					case BinBlockID.GeometryPack: this.ProcessGeometryPack(iparser); break;
					case BinBlockID.TexturePack: this.ProcessTexturePack(iparser); break;
					case BinBlockID.TextureAnimPack: this.ProcessTextureAnimPack(iparser); break;
					case BinBlockID.TopologyTree: this.ProcessTopologyTree(iparser); break;
					default: Debug.LogWarning($"Unknown asset chunk with ID {GenericHelper.GetEnumValueOrUIntHex<BinBlockID>(iparser.BlockID)}"); break;
				}
			}
		}
		protected override void InstantiateTopology()
		{
			// todo
		}
		protected override void UnloadOverrideResources()
		{
			// todo
		}
	}
}
