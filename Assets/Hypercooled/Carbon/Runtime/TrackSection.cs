using Hypercooled.Shared.Interfaces;
using Hypercooled.Utils;
using UnityEngine;

namespace Hypercooled.Carbon.Runtime
{
	public class TrackSection : Shared.Runtime.TrackSection
	{
		public Shared.Core.HeliSectionManager HeliManager { get; private set; }

		public TrackSection(ushort sectionNumber, Shared.Runtime.TrackStreamer streamer) : base(sectionNumber, streamer)
		{
		}

		private void ProcessVisibleSectionUserInfo(IParser iparser)
		{
			this.UserInfo = (iparser as Shared.Parsers.VisibleSectionUserInfoParser).UserInfo;
		}
		private void ProcessTexturePack(IParser iparser)
		{
			var parser = iparser as Parsers.TexturePackParser;
			parser.Container.MergeTexturesIntoMap(this.TextureMap);
			parser.Container.MergeAnimationsIntoMap(this.AnimationMap);
		}
		private void ProcessGeometryPack(IParser iparser)
		{
			(iparser as Parsers.GeometryPackParser).Container.MergeMeshesIntoMap(this.MeshMap);
		}
		private void ProcessHeliSectionManager(IParser iparser)
		{
			this.HeliManager = (iparser as Shared.Parsers.HeliSectionManagerParser).HeliManager;
		}

		protected override BlockReader GetAssetBlockReader()
		{
			var br = new BlockReader();

			br.RegisterParser((uint)BinBlockID.VisibleSectionUserInfo, () => new Shared.Parsers.VisibleSectionUserInfoParser());
			br.RegisterParser((uint)BinBlockID.TexturePack, () => new Parsers.TexturePackParser());
			br.RegisterParser((uint)BinBlockID.GeometryPack, () => new Parsers.GeometryPackParser());
			br.RegisterParser((uint)BinBlockID.HeliSheetManager, () => new Shared.Parsers.HeliSectionManagerParser());
			// todo : wcollisionassets, wgridmaker, worldanim

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
					case BinBlockID.HeliSheetManager: this.ProcessHeliSectionManager(iparser); break;
					// todo : wcollisionassets, wgridmake, worldanim
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
