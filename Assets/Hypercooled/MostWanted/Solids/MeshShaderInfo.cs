using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.MostWanted.Solids
{
	public enum DXShaderType : int
	{
		WorldShader = 0, // 0x24 size = 1 UV
		WorldReflectShader = 1, // 0x3C size = 4 UV
		WorldBoneShader = 2, // 0x3C size = 4 UV
		WorldNormalMap = 3, // 0x3C size = 4 UV
		CarShader = 4, // 0x30 size = 1 UV + 1 Tangent
		GlossyWindow = 5, // 0x24 size = 1 UV
		billboardshader = 6, // 0x24 size = 1 UV
		WorldMinShader = 7,
		WorldNoFogShader = 8,
		FEShader = 9,
		FEMaskShader = 10,
		FilterShader = 11,
		OverbrightShader = 12,
		ScreenFilterShader = 13,
		RainDropShader = 14,
		RunwayLightShader = 15,
		VisualTreatmentShader = 16,
		WorldPrelitShader = 17,
		ParticlesShader = 18,
		skyshader = 19, // 0x2C size = 2 UV
		shadow_map_mesh = 20,
		SkyboxCurrentGen = 21,
		ShadowPolyCurrentGen = 22,
		CarShadowMapShader = 23,
		WorldDepthShader = 24,
		WorldNormalMapDepth = 25,
		CarShaderDepth = 26,
		GlossyWindowDepth = 27,
		TreeDepthShader = 28,
		shadow_map_mesh_depth = 29,
		NormalMapNoFog = 30,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MeshShaderInfo  // 0x00134B02
	{
		public Vector3 BoundsMin;
		public Vector3 BoundsMax;
		public byte TextureDiffuseIndex;  // map_Kd
		public byte TextureNormalIndex;   // map_bump
		public byte TextureHeightIndex;   // 
		public byte TextureSpecularIndex; // map_Ks
		public byte TextureOpacityIndex;  // map_d
		public byte ShaderIndex;
		public short Padding1;
		public long Padding2;
		public long Padding3;
		public DXShaderType DXShader;
		public int Padding4;
		public int Flags;
		public int NumVertices;
		public int NumPolygons;
		public int PolygonIndex;
		public long Padding5;
		public long Padding6;
		public int Padding7;
		public int PolygonSize; // NumPolygons * 3
		public long Padding8;

		public const int SizeOf = 0x68;
	}
}
