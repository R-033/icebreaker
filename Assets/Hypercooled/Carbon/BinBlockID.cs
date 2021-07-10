namespace Hypercooled.Carbon
{
	public enum BinBlockID : uint
	{
		Empty = 0x00000000,

		SmokeableSpawners = 0x00034027,

		VisibleSectionUserInfo = 0x00034099,
		ScenerySectionHeader = 0x00034101,
		SceneryInfos = 0x00034102,
		SceneryInstances = 0x00034103,
		SceneryTreeNodes = 0x00034105,
		SceneryOverrideHooks = 0x00034106,
		PrecullerInfos = 0x00034107,
		SceneryOverrideInfos = 0x00034108,
		SceneryBarrierGroups = 0x00034109,
		ModelHierarchy = 0x0003410C,
		LightTextureCollections = 0x0003410D,
		TrackStreamingSections = 0x00034110,
		TrackStreamingInfos = 0x00034111,
		TrackStreamingBarriers = 0x00034112,
		TrackStreamingDiscBundle = 0x00034113,

		TrackPositionMarkers = 0x00034146,
		TrackPathZones = 0x0003414A,
		TrackPathBarriers = 0x0003414D,

		VisibleSectionManageInfo = 0x00034151,
		VisibleSectionBoundaries = 0x00034152,
		DrivableScenerySections = 0x00034153,
		LoadingSections = 0x00034155,
		ElevationPolygons = 0x00034156,
		VisibleSectionOverlays = 0x00034158,
		HeliSheetManager = 0x00034159,

		TrackObjectBounds = 0x00034191,

		TrackInfos = 0x00034201,
		SunInfos = 0x00034202,

		Weatherman = 0x00034250,

		EventTriggerPackHeader = 0x00036001,
		EventTriggerNodes = 0x00036002,
		EventTriggerEntries = 0x00036003,

		WorldAnimHeader = 0x00037220,
		WorldAnimMatrices = 0x00037240,
		WorldAnimRTNode = 0x00037250,
		WorldAnimNodeInfo = 0x00037260,
		WorldAnimPointer = 0x00037270,

		ParameterMapLayerHeader = 0x0003B602,
		ParameterMapLayerFields = 0x0003B603,
		ParameterMapIDK1 = 0x0003B604,
		ParameterMapIDK2 = 0x0003B605,
		ParameterMapQuad8s = 0x0003B607,
		ParameterMapQuad16s = 0x0003B608,

		UppleUWorld = 0x0003B800,
		WCollisionAssets = 0x0003B801,
		WorldGridMaker = 0x0003B802,

		CollisionBody = 0x0003B901,

		EmitterSystem = 0x0003BC00,

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
		MorphTargets = 0x0013401D,
		SelectionSet = 0x0013401F,
		MeshEdgeList = 0x00134020,
		MeshEdgeLine = 0x00134021,

		MeshInfoHeader = 0x00134900,
		MeshVertexBuffer = 0x00134B01,
		MeshShaderInfos = 0x00134B02,
		MeshPolygons = 0x00134B03,

		MeshVltMaterials = 0x00134C02,
		MeshUcapPcaWeights = 0x00134C04,

		LightSourcePackHeader = 0x00135001,
		LightAABBNodes = 0x00135002,
		LightSources = 0x00135003,

		LightFlarePackHeader = 0x00135101,
		LightFlare = 0x00135102,

		LightMaterials = 0x00135200,

		EAGLSkeletons = 0x00E34009,
		EAGLAnimations = 0x00E34010,

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

		ScenerySection = 0x80034100,
		ModelHierarchyTree = 0x8003410B,
		TrackPathManager = 0x80034147,
		VisibleSectionManager = 0x80034150,

		EventTriggerPack = 0x80036000,

		Quicksplines = 0x8003B000,

		ParameterMaps = 0x8003B600,
		ParameterMapLayer = 0x8003B601,

		EventSystem = 0x8003B810,

		CollisionVolumes = 0x8003B900,

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
