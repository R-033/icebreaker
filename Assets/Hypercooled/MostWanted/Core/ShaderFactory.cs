using Hypercooled.MostWanted.Solids;
using UnityEngine;

namespace Hypercooled.MostWanted.Core
{
	public static class ShaderFactory
	{
		public static Shader DefaultShader { get; }
		public static Shader WorldShader { get; set; }
		public static Shader WorldReflectShader { get; set; }
		public static Shader WorldBoneShader { get; set; }
		public static Shader WorldNormalMap { get; set; }
		public static Shader CarShader { get; set; }
		public static Shader GlossyWindow { get; set; }
		public static Shader billboardshader { get; set; }
		public static Shader WorldMinShader { get; set; }
		public static Shader WorldNoFogShader { get; set; }
		public static Shader FEShader { get; set; }
		public static Shader FEMaskShader { get; set; }
		public static Shader FilterShader { get; set; }
		public static Shader OverbrightShader { get; set; }
		public static Shader ScreenFilterShader { get; set; }
		public static Shader RainDropShader { get; set; }
		public static Shader RunwayLightShader { get; set; }
		public static Shader VisualTreatmentShader { get; set; }
		public static Shader WorldPrelitShader { get; set; }
		public static Shader ParticlesShader { get; set; }
		public static Shader skyshader { get; set; }
		public static Shader shadow_map_mesh { get; set; }
		public static Shader SkyboxCurrentGen { get; set; }
		public static Shader ShadowPolyCurrentGen { get; set; }
		public static Shader CarShadowMapShader { get; set; }
		public static Shader WorldDepthShader { get; set; }
		public static Shader WorldNormalMapDepth { get; set; }
		public static Shader CarShaderDepth { get; set; }
		public static Shader GlossyWindowDepth { get; set; }
		public static Shader TreeDepthShader { get; set; }
		public static Shader shadow_map_mesh_depth { get; set; }
		public static Shader NormalMapNoFog { get; set; }

		static ShaderFactory()
		{
			DefaultShader = Shader.Find("MW/world");
			WorldShader = Shader.Find("MW/world");
			WorldReflectShader = Shader.Find("MW/worldreflect");
			WorldBoneShader = Shader.Find("MW/world");
			WorldNormalMap = Shader.Find("MW/worldnormalmap");
			CarShader = Shader.Find("MW/car");
			GlossyWindow = Shader.Find("MW/glassreflectshader");
			billboardshader = Shader.Find("MW/world");
			WorldMinShader = Shader.Find("MW/world");
			WorldNoFogShader = Shader.Find("MW/world");
			FEShader = Shader.Find("MW/world");
			FEMaskShader = Shader.Find("MW/world");
			FilterShader = Shader.Find("MW/world");
			OverbrightShader = Shader.Find("MW/world");
			ScreenFilterShader = Shader.Find("MW/world");
			RainDropShader = Shader.Find("MW/world");
			RunwayLightShader = Shader.Find("MW/world");
			VisualTreatmentShader = Shader.Find("MW/world");
			WorldPrelitShader = Shader.Find("MW/world");
			ParticlesShader = Shader.Find("MW/world");
			skyshader = Shader.Find("MW/sky");
			shadow_map_mesh = Shader.Find("MW/world");
			SkyboxCurrentGen = Shader.Find("MW/sky");
			ShadowPolyCurrentGen = Shader.Find("MW/world");
			CarShadowMapShader = Shader.Find("MW/car");
			WorldDepthShader = Shader.Find("MW/world");
			WorldNormalMapDepth = Shader.Find("MW/worldnormalmap");
			CarShaderDepth = Shader.Find("MW/car");
			GlossyWindowDepth = Shader.Find("MW/glassreflectshader");
			TreeDepthShader = Shader.Find("MW/trees");
			shadow_map_mesh_depth = Shader.Find("MW/world");
			NormalMapNoFog = Shader.Find("MW/worldnormalmap");
		}

		public static Shader GetDXShader(DXShaderType type)
		{
			switch (type)
			{
				case DXShaderType.WorldShader: return WorldShader;
				case DXShaderType.WorldReflectShader: return WorldReflectShader;
				case DXShaderType.WorldBoneShader: return WorldBoneShader;
				case DXShaderType.WorldNormalMap: return WorldNormalMap;
				case DXShaderType.CarShader: return CarShader;
				case DXShaderType.GlossyWindow: return GlossyWindow;
				case DXShaderType.billboardshader: return billboardshader;
				case DXShaderType.WorldMinShader: return WorldMinShader;
				case DXShaderType.WorldNoFogShader: return WorldNoFogShader;
				case DXShaderType.FEShader: return FEShader;
				case DXShaderType.FEMaskShader: return FEMaskShader;
				case DXShaderType.FilterShader: return FilterShader;
				case DXShaderType.OverbrightShader: return OverbrightShader;
				case DXShaderType.ScreenFilterShader: return ScreenFilterShader;
				case DXShaderType.RainDropShader: return RainDropShader;
				case DXShaderType.RunwayLightShader: return RunwayLightShader;
				case DXShaderType.VisualTreatmentShader: return VisualTreatmentShader;
				case DXShaderType.WorldPrelitShader: return WorldPrelitShader;
				case DXShaderType.ParticlesShader: return ParticlesShader;
				case DXShaderType.skyshader: return skyshader;
				case DXShaderType.shadow_map_mesh: return shadow_map_mesh;
				case DXShaderType.SkyboxCurrentGen: return SkyboxCurrentGen;
				case DXShaderType.ShadowPolyCurrentGen: return ShadowPolyCurrentGen;
				case DXShaderType.CarShadowMapShader: return CarShadowMapShader;
				case DXShaderType.WorldDepthShader: return WorldDepthShader;
				case DXShaderType.WorldNormalMapDepth: return WorldNormalMapDepth;
				case DXShaderType.CarShaderDepth: return CarShaderDepth;
				case DXShaderType.GlossyWindowDepth: return GlossyWindowDepth;
				case DXShaderType.TreeDepthShader: return TreeDepthShader;
				case DXShaderType.shadow_map_mesh_depth: return shadow_map_mesh_depth;
				case DXShaderType.NormalMapNoFog: return NormalMapNoFog;
				default: return DefaultShader;
			}
		}
	}
}
