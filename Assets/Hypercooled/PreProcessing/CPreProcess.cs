namespace Hypercooled.PreProcessing
{
	public static class CPreProcess
	{
		public static void AssertTypeSize()
		{
			// L5RX
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.DrivableScenerySectionInternal), Carbon.MapStream.DrivableScenerySectionInternal.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.ElevationPolygon), Carbon.MapStream.ElevationPolygon.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.GenericRegion), Carbon.MapStream.GenericRegion.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.TrackPathBarrier), Carbon.MapStream.TrackPathBarrier.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.TrackPathZoneInternal), Carbon.MapStream.TrackPathZoneInternal.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.TrackStreamingSection), Carbon.MapStream.TrackStreamingSection.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.VisibleSectionBoundInternal), Carbon.MapStream.VisibleSectionBoundInternal.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.VisibleSectionManageInfo), Carbon.MapStream.VisibleSectionManageInfo.SizeOf);

			// STREAML5RX
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.AcidEffect), Carbon.MapStream.AcidEffect.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.AcidEffectPackHeader), Carbon.MapStream.AcidEffectPackHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.LightTextureCollection), Carbon.MapStream.LightTextureCollection.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.LightTextureEntry), Carbon.MapStream.LightTextureEntry.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.SceneryInfo), Carbon.MapStream.SceneryInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.SceneryInstance), Carbon.MapStream.SceneryInstance.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.SceneryOverrideInfo), Carbon.MapStream.SceneryOverrideInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.ScenerySectionHeader), Carbon.MapStream.ScenerySectionHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.SceneryTreeNode), Carbon.MapStream.SceneryTreeNode.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.SmokeableSpawner), Carbon.MapStream.SmokeableSpawner.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.SmokeableSpawnHeader), Carbon.MapStream.SmokeableSpawnHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.WorldAnimHeader), Carbon.MapStream.WorldAnimHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.WorldAnimMatrix), Carbon.MapStream.WorldAnimMatrix.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.WorldAnimNodeInfo), Carbon.MapStream.WorldAnimNodeInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.WorldAnimPointer), Carbon.MapStream.WorldAnimPointer.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.MapStream.WorldAnimRTNode), Carbon.MapStream.WorldAnimRTNode.SizeOf);

			// Solids
			Assertions.AssertTypeSize(typeof(Carbon.Solids.LightMaterial), Carbon.Solids.LightMaterial.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.Solids.MeshInfoHeader), Carbon.Solids.MeshInfoHeader.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.Solids.MeshReducedVertex), Carbon.Solids.MeshReducedVertex.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.Solids.MeshShaderInfo), Carbon.Solids.MeshShaderInfo.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.Solids.MeshVertex), Carbon.Solids.MeshVertex.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.Solids.SolidInfo), Carbon.Solids.SolidInfo.SizeOf);

			// Textures
			Assertions.AssertTypeSize(typeof(Carbon.Textures.CompressionSlot), Carbon.Textures.CompressionSlot.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.Textures.TextureAnimEntry), Carbon.Textures.TextureAnimEntry.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.Textures.TextureAnimFrame), Carbon.Textures.TextureAnimFrame.SizeOf);
			Assertions.AssertTypeSize(typeof(Carbon.Textures.TextureInfo), Carbon.Textures.TextureInfo.SizeOf);
		}

		public static void AssertInterfaces()
		{
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.CollisionVolumesParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.EmitterSystemParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.GeometryPackParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.SceneryBarrierGroupParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.SceneryOverrideInfoParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.ScenerySectionParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.SmokeableSpawnerParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.TexturePackParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.TrackPathBarrierParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.TrackStreamingSectionParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.UppleUWorldParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.VisibleManagerParser), typeof(Shared.Interfaces.IParser));
			Assertions.AssertInterfaces(typeof(Carbon.Parsers.WCollisionAssetParser), typeof(Shared.Interfaces.IParser));
		}

		public static void AssertInheritance()
		{
			Assertions.AssertInheritance(typeof(Carbon.Core.CollisionObject), typeof(Shared.Core.CollisionObject));
			Assertions.AssertInheritance(typeof(Carbon.Core.MaterialObject), typeof(Shared.Core.MaterialObject));
			Assertions.AssertInheritance(typeof(Carbon.Core.MeshContainer), typeof(Shared.Core.MeshContainer));
			Assertions.AssertInheritance(typeof(Carbon.Core.MeshObject), typeof(Shared.Core.MeshObject));
			Assertions.AssertInheritance(typeof(Carbon.Core.SceneryObject), typeof(Shared.Core.SceneryObject));
			Assertions.AssertInheritance(typeof(Carbon.Core.TextureContainer), typeof(Shared.Core.TextureContainer));
			Assertions.AssertInheritance(typeof(Carbon.Core.TextureObject), typeof(Shared.Core.TextureObject));
			Assertions.AssertInheritance(typeof(Carbon.Core.TopologyManager), typeof(Shared.Core.TopologyManager));
			Assertions.AssertInheritance(typeof(Carbon.Core.TopologyObject), typeof(Shared.Core.TopologyObject));
			Assertions.AssertInheritance(typeof(Carbon.Core.TrackUnpacker), typeof(Shared.Core.TrackUnpacker));

			Assertions.AssertInheritance(typeof(Carbon.MapStream.ScenerySection), typeof(Shared.MapStream.ScenerySection));

			Assertions.AssertInheritance(typeof(Carbon.Runtime.TrackSection), typeof(Shared.Runtime.TrackSection));
			Assertions.AssertInheritance(typeof(Carbon.Runtime.TrackStreamer), typeof(Shared.Runtime.TrackStreamer));
		}
	}
}
