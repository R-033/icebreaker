using Hypercooled.Carbon.Solids;
using UnityEngine;

namespace Hypercooled.Carbon.Core
{
	public static class ShaderFactory
	{
		public static Shader DefaultShader { get; }
		public static Shader WorldShader { get; set; }
		public static Shader WorldReflectShader { get; set; }
		public static Shader WorldBoneShader { get; set; }
		public static Shader WorldNormalMap { get; set; }
		public static Shader CarShader { get; set; }
		public static Shader CARNORMALMAP { get; set; }
		public static Shader WorldMinShader { get; set; }
		public static Shader FEShader { get; set; }
		public static Shader FEMaskShader { get; set; }
		public static Shader FilterShader { get; set; }
		public static Shader ScreenFilterShader { get; set; }
		public static Shader RainDropShader { get; set; }
		public static Shader VisualTreatmentShader { get; set; }
		public static Shader WorldPrelitShader { get; set; }
		public static Shader ParticlesShader { get; set; }
		public static Shader skyshader { get; set; }
		public static Shader shadow_map_mesh { get; set; }
		public static Shader CarShadowMapShader { get; set; }
		public static Shader WorldDepthShader { get; set; }
		public static Shader shadow_map_mesh_depth { get; set; }
		public static Shader NormalMapNoFog { get; set; }
		public static Shader InstanceMesh { get; set; }
		public static Shader ScreenEffectShader { get; set; }
		public static Shader HDRShader { get; set; }
		public static Shader UCAP { get; set; }
		public static Shader GLASS_REFLECT { get; set; }
		public static Shader WATER { get; set; }
		public static Shader RVMPIP { get; set; }
		public static Shader GHOSTCAR { get; set; }
		// avail : more shaders here

		static ShaderFactory()
		{
			DefaultShader = Shader.Find("Okano/Double-sided Standard Lit");
		}

		public static Shader GetDXShader(DXShaderType type)
		{
			return ShaderFactory.DefaultShader;

			// todo : actual shaders
			//switch (type)
			//{
			//	case DXShaderType.WorldShader: return WorldShader;
			//	case DXShaderType.WorldReflectShader: return WorldReflectShader;
			//	case DXShaderType.WorldBoneShader: return WorldBoneShader;
			//	case DXShaderType.WorldNormalMap: return WorldNormalMap;
			//	case DXShaderType.CarShader: return CarShader;
			//	case DXShaderType.CARNORMALMAP: return CARNORMALMAP;
			//	case DXShaderType.WorldMinShader: return WorldMinShader;
			//	case DXShaderType.FEShader: return FEShader;
			//	case DXShaderType.FEMaskShader: return FEMaskShader;
			//	case DXShaderType.FilterShader: return FilterShader;
			//	case DXShaderType.ScreenFilterShader: return ScreenFilterShader;
			//	case DXShaderType.RainDropShader: return RainDropShader;
			//	case DXShaderType.VisualTreatmentShader: return VisualTreatmentShader;
			//	case DXShaderType.WorldPrelitShader: return WorldPrelitShader;
			//	case DXShaderType.ParticlesShader: return ParticlesShader;
			//	case DXShaderType.skyshader: return skyshader;
			//	case DXShaderType.shadow_map_mesh: return shadow_map_mesh;
			//	case DXShaderType.CarShadowMapShader: return CarShadowMapShader;
			//	case DXShaderType.WorldDepthShader: return WorldDepthShader;
			//	case DXShaderType.shadow_map_mesh_depth: return shadow_map_mesh_depth;
			//	case DXShaderType.NormalMapNoFog: return NormalMapNoFog;
			//	case DXShaderType.InstanceMesh: return InstanceMesh;
			//	case DXShaderType.ScreenEffectShader: return ScreenEffectShader;
			//	case DXShaderType.HDRShader: return HDRShader;
			//	case DXShaderType.UCAP: return UCAP;
			//	case DXShaderType.GLASS_REFLECT: return GLASS_REFLECT;
			//	case DXShaderType.WATER: return WATER;
			//	case DXShaderType.RVMPIP: return RVMPIP;
			//	case DXShaderType.GHOSTCAR: return GHOSTCAR;
			//	default: return DefaultShader;
			//}
		}
	}
}
