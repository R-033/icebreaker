using UnityEngine;

namespace Hypercooled.Underground2.Core
{
	public static class ShaderFactory
	{
		public static Shader DefaultShader { get; }
		public static Shader WorldShader { get; set; }
		// avail : more shaders here

		static ShaderFactory()
		{
			DefaultShader = Shader.Find("Okano/Double-sided Standard Lit");
		}
	}
}
