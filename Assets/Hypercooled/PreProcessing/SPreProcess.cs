using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Parsers;
using Hypercooled.Shared.Structures;
using Hypercooled.Shared.Solids;
using Hypercooled.Shared.Textures;
using Hypercooled.Shared.World;
using SerializationHeader = Hypercooled.Shared.Core.VisibleSectionUserInfo.SerializationHeader;

namespace Hypercooled.PreProcessing
{
	public static class SPreProcess
	{
		public static void AssertTypeSize()
		{
			// Structures
			Assertions.AssertTypeSize(typeof(BinBlock.Info), BinBlock.Info.SizeOf);
			Assertions.AssertTypeSize(typeof(Color32F), Color32F.SizeOf);
			Assertions.AssertTypeSize(typeof(Color8UI), Color8UI.SizeOf);
			Assertions.AssertTypeSize(typeof(Matrix3x3), Matrix3x3.SizeOf);
			Assertions.AssertTypeSize(typeof(Matrix3x3Packed), Matrix3x3Packed.SizeOf);
			Assertions.AssertTypeSize(typeof(OffsetEntry), OffsetEntry.SizeOf);
			Assertions.AssertTypeSize(typeof(SerializationHeader), SerializationHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Vector3Packed), Vector3Packed.SizeOf);
			Assertions.AssertTypeSize(typeof(Vector4Packed), Vector4Packed.SizeOf);

			// LXRY
			Assertions.AssertTypeSize(typeof(LoadingSection), LoadingSection.SizeOf);
			Assertions.AssertTypeSize(typeof(ModelHierarchyHeader), ModelHierarchyHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(ModelHierarchyNode), ModelHierarchyNode.SizeOf);
			Assertions.AssertTypeSize(typeof(TrackObjectBounds), TrackObjectBounds.SizeOf);
			Assertions.AssertTypeSize(typeof(TrackPositionMarker), TrackPositionMarker.SizeOf);
			Assertions.AssertTypeSize(typeof(TrackStreamingBarrier), TrackStreamingBarrier.SizeOf);
			Assertions.AssertTypeSize(typeof(TrackStreamingInfo), TrackStreamingInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(WeathermanHeader), WeathermanHeader.SizeOf);

			// STREAMLXRY
			Assertions.AssertTypeSize(typeof(EventTriggerEntry), EventTriggerEntry.SizeOf);
			Assertions.AssertTypeSize(typeof(EventTriggerNode), EventTriggerNode.SizeOf);
			Assertions.AssertTypeSize(typeof(EventTriggerPackHeader), EventTriggerPackHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(EventTriggerTree), EventTriggerTree.SizeOf);
			Assertions.AssertTypeSize(typeof(HeliPolygon), HeliPolygon.SizeOf);
			Assertions.AssertTypeSize(typeof(HeliSection), HeliSection.SizeOf);
			Assertions.AssertTypeSize(typeof(LightAABBNode), LightAABBNode.SizeOf);
			Assertions.AssertTypeSize(typeof(LightAABBTree), LightAABBTree.SizeOf);
			Assertions.AssertTypeSize(typeof(LightFlare), LightFlare.SizeOf);
			Assertions.AssertTypeSize(typeof(LightFlarePackHeader), LightFlarePackHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(LightSource), LightSource.SizeOf);
			Assertions.AssertTypeSize(typeof(LightSourcePackHeader), LightSourcePackHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(PrecullerInfo), PrecullerInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(SceneryOverrideHook), SceneryOverrideHook.SizeOf);

			// Textures
			Assertions.AssertTypeSize(typeof(TexturePackBuffer), TexturePackBuffer.SizeOf);
			Assertions.AssertTypeSize(typeof(TexturePackHeader), TexturePackHeader.SizeOf);

			// Solids
			Assertions.AssertTypeSize(typeof(CVBound), CVBound.SizeOf);
			Assertions.AssertTypeSize(typeof(CVHeader), CVHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(MeshContainerHeader), MeshContainerHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(NormalSmoother), NormalSmoother.SizeOf);
			Assertions.AssertTypeSize(typeof(SmoothVertex), SmoothVertex.SizeOf);
			Assertions.AssertTypeSize(typeof(SmoothVertexPlat), SmoothVertexPlat.SizeOf);
			Assertions.AssertTypeSize(typeof(SolidMarker), SolidMarker.SizeOf);
			Assertions.AssertTypeSize(typeof(SolidMaterial), SolidMaterial.SizeOf);
			Assertions.AssertTypeSize(typeof(SolidTexture), SolidTexture.SizeOf);

			// World
			Assertions.AssertTypeSize(typeof(UppleUHeader), UppleUHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(UppleUGroup.UGroup), UppleUGroup.UGroup.SizeOf);
			Assertions.AssertTypeSize(typeof(UppleUData.UData), UppleUData.UData.SizeOf);
			Assertions.AssertTypeSize(typeof(VVCollisionArticle.VVCollisionBarrier), VVCollisionArticle.VVCollisionBarrier.SizeOf);
			Assertions.AssertTypeSize(typeof(VVCollisionArticle.VVCollisionHeader), VVCollisionArticle.VVCollisionHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(VVCollisionArticle.VVCollisionStripSphere), VVCollisionArticle.VVCollisionStripSphere.SizeOf);
			Assertions.AssertTypeSize(typeof(VVCollisionArticle.VVCollisionSurface), VVCollisionArticle.VVCollisionSurface.SizeOf);
			Assertions.AssertTypeSize(typeof(VVCollisionArticle.VVCollisionVertex), VVCollisionArticle.VVCollisionVertex.SizeOf);
			Assertions.AssertTypeSize(typeof(VVCollisionInstance.Data), VVCollisionInstance.Data.SizeOf);
			Assertions.AssertTypeSize(typeof(VVCollisionObject.Data), VVCollisionObject.Data.SizeOf);
			Assertions.AssertTypeSize(typeof(VVGrid.Data), VVGrid.Data.SizeOf);
			Assertions.AssertTypeSize(typeof(VVGridIsland.Data), VVGridIsland.Data.SizeOf);
			Assertions.AssertTypeSize(typeof(VVGridNode.VVGridNodeHeader), VVGridNode.VVGridNodeHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(VVGridNode.VVGridNodeInstance), VVGridNode.VVGridNodeInstance.SizeOf);
			Assertions.AssertTypeSize(typeof(VVGridNode.VVGridNodeRoadSegment), VVGridNode.VVGridNodeRoadSegment.SizeOf);
			Assertions.AssertTypeSize(typeof(VVRoad.Data), VVRoad.Data.SizeOf);
			Assertions.AssertTypeSize(typeof(VVRoadNetworkInfo.Data), VVRoadNetworkInfo.Data.SizeOf);
			Assertions.AssertTypeSize(typeof(VVRoadNode.Data), VVRoadNode.Data.SizeOf);
			Assertions.AssertTypeSize(typeof(VVRoadProfile.Data), VVRoadProfile.Data.SizeOf);
			Assertions.AssertTypeSize(typeof(VVRoadProfile.VVRoadBits), VVRoadProfile.VVRoadBits.SizeOf);
			Assertions.AssertTypeSize(typeof(VVRoadSegment.Data), VVRoadSegment.Data.SizeOf);
		}

		public static void AssertInterfaces()
		{
			Assertions.AssertInterfaces(typeof(EventTriggerPackParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(GenericParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(HeliSectionManagerParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(LightFlarePackParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(LightSourcePackParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(ModelHierarchyTreeParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(TrackObjectBoundsParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(TrackPositionMarkerParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(TrackStreamingBarrierParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(TrackStreamingInfoParser), typeof(IParser));
			Assertions.AssertInterfaces(typeof(VisibleSectionUserInfoParser), typeof(IParser));
		}

		public static void AssertInheritance()
		{
			Assertions.AssertInheritance(typeof(VVCollisionArticle), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVCollisionInstance), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVCollisionObject), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVGrid), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVGridIsland), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVGridNode), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVNameString), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVRoad), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVRoadNetworkInfo), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVRoadNode), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVRoadProfile), typeof(VVResolvable));
			Assertions.AssertInheritance(typeof(VVRoadSegment), typeof(VVResolvable));
		}
	}
}
