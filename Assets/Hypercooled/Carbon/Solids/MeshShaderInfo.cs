using Hypercooled.Shared.Solids;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Carbon.Solids
{
	public enum DXShaderType : int
	{
		WorldShader = 0, // 0x24 size = 1 UV
		WorldReflectShader = 1, // 0x34 size = 3 UV
		WorldBoneShader = 2, // 0x48 size = 4 UV + 1 Tangent
		WorldNormalMap = 3, // 0x34 size = 3 UV
		CarShader = 4, // 0x30 size = 1 UV + 1 Tangent
		CARNORMALMAP = 5, // 0x30 size = 1 UV + 1 Tangent
		WorldMinShader = 6,
		FEShader = 7,
		FEMaskShader = 8,
		FilterShader = 9,
		ScreenFilterShader = 10,
		RainDropShader = 11,
		VisualTreatmentShader = 12,
		WorldPrelitShader = 13,
		ParticlesShader = 14,
		skyshader = 15, // 0x2C size = 2 UV
		shadow_map_mesh = 16,
		CarShadowMapShader = 17,
		WorldDepthShader = 18,
		shadow_map_mesh_depth = 19,
		NormalMapNoFog = 20,
		InstanceMesh = 21,
		ScreenEffectShader = 22,
		HDRShader = 23,
		UCAP = 24, // 0xAC size = ???
		GLASS_REFLECT = 25, // 0x24 size = 1 UV
		WATER = 26, // 0x24 size = 1 UV
		RVMPIP = 27,
		GHOSTCAR = 28,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MeshShaderInfo  // 0x00134B02
	{
		public Vector3 BoundsMin;
		public Vector3 BoundsMax;
		public byte TextureDiffuseIndex;  // DIFFUSEMAPTEXTURE
		public byte TextureNormalIndex;   // NORMALMAPTEXTURE
		public byte TextureHeightIndex;   // HEIGHTMAPTEXTURE
		public byte TextureSpecularIndex; // SPECULARMAPTEXTURE
		public byte TextureOpacityIndex;  // OPACITYMAPTEXTURE
		public byte ShaderIndex;
		public short Padding1;
		public long Padding2;
		public long Padding3;
		public DXShaderType DXShader;
		public int Padding4;
		public MeshEntryFlags Flags;
		public uint ReplacementTexture;
		public int NumVertices;
		public int SomeFlagPerhaps;
		public long Padding5;
		public long Padding6;
		public long Padding7;
		public int NumPolygons;
		public int PolygonIndex;
		public long Padding8;
		public long Padding9;
		public int Padding10;
		public int PolygonSize; // NumPolygons * 3
		public long Padding11;
		public long Padding12;

		public const int SizeOf = 0x90;
	}
}
