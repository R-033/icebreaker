namespace Hypercooled.PreProcessing
{
	public static class U2PreProcess
	{
		public static void AssertTypeSize()
		{
			// L4RX
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.AcidEmitter), Underground2.MapStream.AcidEmitter.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.AcidEmitterPackHeader), Underground2.MapStream.AcidEmitterPackHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.DrivableScenerySection), Underground2.MapStream.DrivableScenerySection.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.GenericRegion), Underground2.MapStream.GenericRegion.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.SmokeableInfo), Underground2.MapStream.SmokeableInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TopologyBarrierGroup), Underground2.MapStream.TopologyBarrierGroup.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TrackSpecificSection), Underground2.MapStream.TrackSpecificSection.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TrackStreamingSection), Underground2.MapStream.TrackStreamingSection.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.VisibleSectionBoundInternal), Underground2.MapStream.VisibleSectionBoundInternal.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.VisibleSectionManageInfo), Underground2.MapStream.VisibleSectionManageInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.WorldAnimCtrl), Underground2.MapStream.WorldAnimCtrl.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.WorldAnimEntity), Underground2.MapStream.WorldAnimEntity.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.WorldAnimInstance), Underground2.MapStream.WorldAnimInstance.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.WorldAnimTreeMarker), Underground2.MapStream.WorldAnimTreeMarker.SizeOf);

			// STREAML4RX
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.AcidEffect), Underground2.MapStream.AcidEffect.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.AcidEffectPackHeader), Underground2.MapStream.AcidEffectPackHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.SceneryInfo), Underground2.MapStream.SceneryInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.SceneryInstance), Underground2.MapStream.SceneryInstance.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.SceneryOverrideInfo), Underground2.MapStream.SceneryOverrideInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.ScenerySectionHeader), Underground2.MapStream.ScenerySectionHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.SceneryTreeNode), Underground2.MapStream.SceneryTreeNode.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.SmokeableSpawner), Underground2.MapStream.SmokeableSpawner.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.SmokeableSpawnHeader), Underground2.MapStream.SmokeableSpawnHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TopologyCoordinate), Underground2.MapStream.TopologyCoordinate.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TopologyIndexation), Underground2.MapStream.TopologyIndexation.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TopologyOrdering), Underground2.MapStream.TopologyOrdering.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TopologyTreeHeader), Underground2.MapStream.TopologyTreeHeader.SizeOf);

			// PathsXXXX
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TrackPathArea), Underground2.MapStream.TrackPathArea.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TrackPathBarrier), Underground2.MapStream.TrackPathBarrier.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TrackPathLane), Underground2.MapStream.TrackPathLane.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TrackPathLanePointer), Underground2.MapStream.TrackPathLanePointer.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TrackPathPoint), Underground2.MapStream.TrackPathPoint.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.MapStream.TrackPathZoneInternal), Underground2.MapStream.TrackPathZoneInternal.SizeOf);

			// Solids
			Assertions.AssertTypeSize(typeof(Underground2.Solids.CollisionHeader), Underground2.Solids.CollisionHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Solids.CollisionVertex), Underground2.Solids.CollisionVertex.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Solids.LightMaterial), Underground2.Solids.LightMaterial.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Solids.MeshInfoHeader), Underground2.Solids.MeshInfoHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Solids.MeshReducedVertex), Underground2.Solids.MeshReducedVertex.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Solids.MeshShaderInfo), Underground2.Solids.MeshShaderInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Solids.MeshVertex), Underground2.Solids.MeshVertex.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Solids.SolidInfo), Underground2.Solids.SolidInfo.SizeOf);

			// Textures
			Assertions.AssertTypeSize(typeof(Underground2.Textures.CompressionSlot), Underground2.Textures.CompressionSlot.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Textures.TextureAnimEntry), Underground2.Textures.TextureAnimEntry.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Textures.TextureAnimFrame), Underground2.Textures.TextureAnimFrame.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Textures.TextureAnimPackHeader), Underground2.Textures.TextureAnimPackHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Underground2.Textures.TextureInfo), Underground2.Textures.TextureInfo.SizeOf);
		}

		public static void AssertInterfaces()
		{
			Assertions.AssertInheritance(typeof(Underground2.Core.CollisionObject), typeof(Shared.Core.CollisionObject));
			Assertions.AssertInheritance(typeof(Underground2.Core.MaterialObject), typeof(Shared.Core.MaterialObject));
			Assertions.AssertInheritance(typeof(Underground2.Core.MeshContainer), typeof(Shared.Core.MeshContainer));
			Assertions.AssertInheritance(typeof(Underground2.Core.MeshObject), typeof(Shared.Core.MeshObject));
			Assertions.AssertInheritance(typeof(Underground2.Core.SceneryObject), typeof(Shared.Core.SceneryObject));
			Assertions.AssertInheritance(typeof(Underground2.Core.TextureContainer), typeof(Shared.Core.TextureContainer));
			Assertions.AssertInheritance(typeof(Underground2.Core.TextureObject), typeof(Shared.Core.TextureObject));
			Assertions.AssertInheritance(typeof(Underground2.Core.TopologyManager), typeof(Shared.Core.TopologyManager));
			Assertions.AssertInheritance(typeof(Underground2.Core.TopologyObject), typeof(Shared.Core.TopologyObject));
			Assertions.AssertInheritance(typeof(Underground2.Core.TrackUnpacker), typeof(Shared.Core.TrackUnpacker));

			Assertions.AssertInheritance(typeof(Underground2.MapStream.ScenerySection), typeof(Shared.MapStream.ScenerySection));

			Assertions.AssertInheritance(typeof(Underground2.Runtime.TrackSection), typeof(Shared.Runtime.TrackSection));
			Assertions.AssertInheritance(typeof(Underground2.Runtime.TrackStreamer), typeof(Shared.Runtime.TrackStreamer));
		}

		public static void AssertInheritances()
		{
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.AcidEffectParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.AcidEmitterParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.CollisionVolumesParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.GeometryPackParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.SceneryBarrierGroupParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.SceneryOverrideInfoParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.ScenerySectionParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.SmokeableInfoParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.SmokeableSpawnerParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.TextureAnimPackParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.TexturePackParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.TopologyBarrierGroupParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.TopologyTreeParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.TrackStreamingSectionParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Underground2.Parsers.VisibleManagerParser), typeof(Shared.Interfaces.IParser));
		}
	}
}
