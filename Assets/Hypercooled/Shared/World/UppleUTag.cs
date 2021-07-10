namespace Hypercooled.Shared.World
{
	public enum UppleUTag : uint
	{
		// Indexed
		WCollisionArticle = 0x6361,  // 'ca'
		WCollisionInstance = 0x6369, // 'ci'
		WGridIsland = 0x636C,        // 'cl'
		WCollisionObject = 0x636F,   // 'co'

		// UGroups
		Main = 0x43415250,     // 'CARP'
		CData = 0x43446174,    // 'CDat'
		Map = 0x4D617020,      // 'Map '
		Article = 0x41727469,  // 'Arti'
		WNetwork = 0x524E6770, // 'RNgp'

		// UDatas
		Name = 0x4E616D65,             // 'Name'
		WGrid = 0x43477264,            // 'CGrd'
		WRoadNetworkInfo = 0x524E6864, // 'RNhd'
		WRoadNode = 0x524E6E64,        // 'RNnd'
		WRoadProfile = 0x524E7066,     // 'RNpf'
		WRoad = 0x524E7264,            // 'RNrd'
		WRoadSegment = 0x524E7367,     // 'RNsg'
		WGridNode = 0x4347636E,        // 'CGcn'
	}
}
