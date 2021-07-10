namespace Hypercooled.PreProcessing
{
	public static class MWPreProcess
	{
		public static void AssertTypeSize()
		{
			// L2RX
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.DrivableScenerySectionInternal), MostWanted.MapStream.DrivableScenerySectionInternal.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.GenericRegion), MostWanted.MapStream.GenericRegion.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.TrackPathBarrier), MostWanted.MapStream.TrackPathBarrier.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.TrackPathZoneInternal), MostWanted.MapStream.TrackPathZoneInternal.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.TrackStreamingSection), MostWanted.MapStream.TrackStreamingSection.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.VisibleSectionBoundInternal), MostWanted.MapStream.VisibleSectionBoundInternal.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.VisibleSectionManageInfo), MostWanted.MapStream.VisibleSectionManageInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.WorldAnimCtrl), MostWanted.MapStream.WorldAnimCtrl.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.WorldAnimEntity), MostWanted.MapStream.WorldAnimEntity.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.WorldAnimInstance), MostWanted.MapStream.WorldAnimInstance.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.WorldAnimTreeMarker), MostWanted.MapStream.WorldAnimTreeMarker.SizeOf);

			// STREAML2RX
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.AcidEffect), MostWanted.MapStream.AcidEffect.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.AcidEffectPackHeader), MostWanted.MapStream.AcidEffectPackHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.SceneryInfo), MostWanted.MapStream.SceneryInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.SceneryInstance), MostWanted.MapStream.SceneryInstance.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.SceneryOverrideInfo), MostWanted.MapStream.SceneryOverrideInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.ScenerySectionHeader), MostWanted.MapStream.ScenerySectionHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.SceneryTreeNode), MostWanted.MapStream.SceneryTreeNode.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.SmokeableSpawner), MostWanted.MapStream.SmokeableSpawner.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.MapStream.SmokeableSpawnHeader), MostWanted.MapStream.SmokeableSpawnHeader.SizeOf);

			// Solids
			Assertions.AssertTypeSize(typeof(MostWanted.Solids.LightMaterial), MostWanted.Solids.LightMaterial.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.Solids.MeshInfoHeader), MostWanted.Solids.MeshInfoHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.Solids.MeshReducedVertex), MostWanted.Solids.MeshReducedVertex.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.Solids.MeshShaderInfo), MostWanted.Solids.MeshShaderInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.Solids.MeshVertex), MostWanted.Solids.MeshVertex.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.Solids.SolidInfo), MostWanted.Solids.SolidInfo.SizeOf);

			// Textures
			Assertions.AssertTypeSize(typeof(MostWanted.Textures.CompressionSlot), MostWanted.Textures.CompressionSlot.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.Textures.TextureAnimEntry), MostWanted.Textures.TextureAnimEntry.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.Textures.TextureAnimFrame), MostWanted.Textures.TextureAnimFrame.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.Textures.TextureAnimPackHeader), MostWanted.Textures.TextureAnimPackHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(MostWanted.Textures.TextureInfo), MostWanted.Textures.TextureInfo.SizeOf);
		}

		public static void AssertInterfaces()
		{
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.CollisionVolumesParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.EmitterSystemParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.GeometryPackParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.SceneryBarrierGroupParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.SceneryOverrideInfoParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.ScenerySectionParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.SmokeableSpawnerParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.TextureAnimPackParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.TexturePackParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.TrackPathBarrierParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.TrackStreamingSectionParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.VisibleManagerParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(MostWanted.Parsers.WCollisionAssetParser), typeof(Shared.Interfaces.IParser));
		}

		public static void AssertInheritance()
		{
			Assertions.AssertInheritance(typeof(MostWanted.Core.CollisionObject), typeof(Shared.Core.CollisionObject));
			Assertions.AssertInheritance(typeof(MostWanted.Core.MaterialObject), typeof(Shared.Core.MaterialObject));
			Assertions.AssertInheritance(typeof(MostWanted.Core.MeshContainer), typeof(Shared.Core.MeshContainer));
			Assertions.AssertInheritance(typeof(MostWanted.Core.MeshObject), typeof(Shared.Core.MeshObject));
			Assertions.AssertInheritance(typeof(MostWanted.Core.SceneryObject), typeof(Shared.Core.SceneryObject));
			Assertions.AssertInheritance(typeof(MostWanted.Core.TextureContainer), typeof(Shared.Core.TextureContainer));
			Assertions.AssertInheritance(typeof(MostWanted.Core.TextureObject), typeof(Shared.Core.TextureObject));
			Assertions.AssertInheritance(typeof(MostWanted.Core.TopologyManager), typeof(Shared.Core.TopologyManager));
			Assertions.AssertInheritance(typeof(MostWanted.Core.TopologyObject), typeof(Shared.Core.TopologyObject));
			Assertions.AssertInheritance(typeof(MostWanted.Core.TrackUnpacker), typeof(Shared.Core.TrackUnpacker));

			Assertions.AssertInheritance(typeof(MostWanted.MapStream.ScenerySection), typeof(Shared.MapStream.ScenerySection));

			Assertions.AssertInheritance(typeof(MostWanted.Runtime.TrackSection), typeof(Shared.Runtime.TrackSection));
			Assertions.AssertInheritance(typeof(MostWanted.Runtime.TrackStreamer), typeof(Shared.Runtime.TrackStreamer));
		}
	}
}
