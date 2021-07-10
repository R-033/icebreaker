using System;

namespace Hypercooled.Shared.Solids
{
	[Flags()]
	public enum MeshEntryFlags : int
	{
		IsVisible = 0x01,
		Flag0x02 = 0x02, // always with 0x10
		//Flag0x04 = 0x04,
		//Flag0x08 = 0x08,
		Flag0x10 = 0x10, // always with 0x02
		//Flag0x20 = 0x20,
		Flag0x40 = 0x40, // idk mayb road reflection?
		HasFullVertices = 0x80, // for ug2 - normals, for mw & c - tangents
		UseLighting = 0x0100,
		//Flag0x0200 = 0x0200,
		//Flag0x0400 = 0x0400,
		Flag0x0800 = 0x0800, // 2 UV channels?
		Flag0x1000 = 0x1000, // WaterShader
		//Flag0x2000 = 0x2000,
		IsSolid = 0x4000,
		Flag0x8000 = 0x8000, // FEMaskShader
		HasNormalSmoother = 0x10000,
		HasNormalMap = 0x20000,
		HasHeightMap = 0x40000,
		HasSpecularMap = 0x80000,
		Flag0x100000 = 0x100000, // used in restaurants?
		EnableAlphaBlend = 0x200000, // always enabled when normal map
		Flag0x400000 = 0x400000, // WaterShader
		Flag0x800000 = 0x800000, // always enabled when specular map?
		//Flag0x01000000 = 0x01000000,
		Flag0x02000000 = 0x02000000, // fence?
		Flag0x04000000 = 0x04000000, // 2 UV channels?
	}
}
