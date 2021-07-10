namespace Hypercooled.Underground2
{
	public enum BinBlockID : uint
	{
		Empty = 0x00000000,

		SmokeableInfos = 0x00034026,
		SmokeableSpawners = 0x00034027,

		VisibleSectionUserInfo = 0x00034099,
		ScenerySectionHeader = 0x00034101,
		SceneryInfos = 0x00034102,
		SceneryInstances = 0x00034103,
		SceneryTreeNodes = 0x00034104,
		SceneryOverrideHooks = 0x00034105,
		PrecullerInfos = 0x00034106,
		SceneryOverrideInfos = 0x00034107,
		SceneryBarrierGroups = 0x00034108,
		TrackStreamingSections = 0x00034110,
		TrackStreamingInfos = 0x00034111,
		TrackStreamingBarriers = 0x00034112,
		TrackStreamingDiscBundle = 0x00034113,

		TrackRouteManager = 0x00034121,
		SignPosts = 0x00034122,
		TrafficIntersections = 0x00034123,
		CrossTrafficEmitters = 0x00034124,

		TopologyBarrierGroups = 0x0003412F,
		TopologyTreeHeader = 0x00034131,
		TopologyCoordinatesUG2 = 0x00034134,
		TopologyIndexationUG2 = 0x00034136,
		TopologyOrdrering = 0x00034137,
		TopologyObjectNames = 0x00034138,

		TrackPositionMarkers = 0x00034146,
		TrackPathPoints = 0x00034148,
		TrackPathLanes = 0x00034149,
		TrackPathZones = 0x0003414A,
		_some_track_path_area_ = 0x0003414C,
		TrackPathBarriers = 0x0003414D,

		VisibleSectionManageInfo = 0x00034151,
		VisibleSectionBoundaries = 0x00034152,
		DrivableScenerySections = 0x00034153,
		TrackSpecificSections = 0x00034154,
		LoadingSections = 0x00034155,

		TrackInfos = 0x00034201,
		SunInfos = 0x00034202,

		Weatherman = 0x00034250,

		AcidEffects = 0x00035020,
		AcidEmitters = 0x00035021,

		EventTriggerPackHeader = 0x00036001,
		EventTriggerNodes = 0x00036002,
		EventTriggerEntries = 0x00036003,

		WorldAnimEntities = 0x00037080,
		WorldAnimInstances = 0x00037090,
		CarPartsAnimHeader = 0x00037100,
		WorldAnimTreeMarkers = 0x00037110,
		WorldAnimEventDirs = 0x00037140,
		WorldAnimCtrl = 0x00037150,

		CollisionHeader = 0x00039200,
		CollisionVertices = 0x00039201,
		CollisionIndices = 0x00039202,

		DragCamera = 0x0003B210,

		ParameterMapLayerHeader = 0x0003B602,
		ParameterMapLayerFields = 0x0003B603,
		ParameterMapIDK1 = 0x0003B604,
		ParameterMapIDK2 = 0x0003B605,
		ParameterMapQuad8s = 0x0003B607,
		ParameterMapQuad16s = 0x0003B608,

		MeshContainerHeader = 0x00134002,
		MeshContainerKeys = 0x00134003,
		MeshContainerOffsets = 0x00134004,
		MeshContainerEmpty = 0x00134008,
		SolidInfo = 0x00134011,
		SolidTextures = 0x00134012,
		SolidShaders = 0x00134013,

		MeshNormalSmoother = 0x00134017,
		MeshSmoothVertices = 0x00134018,
		MeshSmoothVertexPlats = 0x00134019,
		SolidMarkers = 0x0013401A,

		MeshInfoHeader = 0x00134900,
		MeshVertexBuffer = 0x00134B01,
		MeshShaderInfos = 0x00134B02,
		MeshPolygons = 0x00134B03,

		LightSourcePackHeader = 0x00135001,
		LightAABBNodes = 0x00135002,
		LightSources = 0x00135003,

		LightFlarePackHeader = 0x00135101,
		LightFlare = 0x00135102,

		LightMaterials = 0x00135200,

		EAGLSkeletons = 0x00E34009,
		EAGLAnimations = 0x00E34010,

		TextureAnimPackHeader = 0x30300101,
		TextureAnimEntry = 0x30300102,
		TextureAnimFrames = 0x30300103,

		TexturePackInfoHeader = 0x33310001,
		TexturePackInfoKeys = 0x33310002,
		TexturePackInfoEntries = 0x33310003,
		TexturePackInfoTextures = 0x33310004,
		TexturePackInfoComps = 0x33310005,
		TexturePackAnimNames = 0x33312001,
		TexturePackAnimFrames = 0x33312002,
		TexturePackDataHeader = 0x33320001,
		TexturePackDataArray = 0x33320002,
		TexturePackDataUnknown = 0x33320003,

		CollisionVolumes = 0x80034020,
		ScenerySection = 0x80034100,
		TopologyTree = 0x80034130,
		TrackPathManager = 0x80034147,
		VisibleSectionManager = 0x80034150,

		EventTriggerPack = 0x80036000,

		Quicksplines = 0x8003B000,

		DragCameras = 0x8003B200,

		ParameterMaps = 0x8003B600,
		ParameterMapLayer = 0x8003B601,

		GeometryPack = 0x80134000,
		MeshContainerInfo = 0x80134001,
		SolidPack = 0x80134010,
		MeshInfoContainer = 0x80134100,

		LightSourcesPack = 0x80135000,
		LightFlaresPack = 0x80135100,

		TextureAnimPack = 0xB0300100,

		TexturePack = 0xB3300000,
		TexturePackInfo = 0xB3310000,
		TexturePackBinary = 0xB3312000,
		TexturePackAnim = 0xB3312004,
		TexturePackData = 0xB3320000,
	}
}
