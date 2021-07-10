using System;

namespace Hypercooled.Shared.MapStream
{
	[Flags()]
	public enum SceneryInstanceFlags : uint
	{
		ExcludeFlagSplitScreen = 1u << 0, // 0x01
		ExcludeFlagMainView = 1u << 1, // 0x02
		ExcludeFlagRacing = 1u << 2, // 0x04
		ExcludeFlagDisableRendering = 1u << 3, // 0x08
		ExcludeFlagGroupDisable = 1u << 4, // 0x10
		Driveable = 1u << 5, // 0x20
		EnableRearView = 1u << 6, // 0x40
		EnableReflection = 1u << 7, // 0x80
		EnvmapShadow = 1u << 8, // 0x100
		ChoppedRoadway = 1u << 9, // 0x200
		IdentityMatrix = 1u << 10, // 0x400
		ArtworkFlipped = 1u << 11, // 0x800
		Reflection = 1u << 12, // 0x1000
		EnvironmentMap = 1u << 13, // 0x2000
		Swayable = 1u << 14, // 0x4000
		EnableWind = 1u << 15, // 0x8000
		AlwaysFacing = 1u << 16, // 0x00010000
		DontReceiveShadows = 1u << 17, // 0x00020000
		InstanceRender = 1u << 18, // 0x00040000
		HighPlatformOnly = 1u << 19, // 0x00080000
		CastShadowVolume = 1u << 20, // 0x00100000
		CastShadowMap = 1u << 21, // 0x00200000
		InvertedMatrix = 1u << 22, // 0x00400000
		FlipOnBackwardsTrack = 1u << 23, // 0x00800000
		ReflectInOcean = 1u << 24, // 0x01000000
		VisibleFurther = 1u << 25, // 0x02000000
		CastShadowMapMesh = 1u << 26, // 0x04000000
		EnableReflectionNG = 1u << 27, // 0x08000000
		AUXLighting = 1u << 28, // 0x10000000
		PCPlatform = 1u << 29, // 0x20000000
		Smokeable = 1u << 30, // 0x40000000
		Lightmapped = 1u << 31, // 0x80000000
	}
}
