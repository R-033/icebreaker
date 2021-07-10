using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LightMaterial
	{
		public float DiffuseMinLevel;
		public float DiffuseMinRed;
		public float DiffuseMinGreen;
		public float DiffuseMinBlue;
		public float DiffuseMaxLevel;
		public float DiffuseMaxRed;
		public float DiffuseMaxGreen;
		public float DiffuseMaxBlue;
		public float DiffuseMinAlpha;
		public float DiffuseMaxAlpha;
		public float SpecularPower;
		public float SpecularMinLevel;
		public float SpecularMinRed;
		public float SpecularMinGreen;
		public float SpecularMinBlue;
		public float SpecularMaxLevel;
		public float SpecularMaxRed;
		public float SpecularMaxGreen;
		public float SpecularMaxBlue;
		public float EnvmapPower;
		public float EnvmapMinLevel;
		public float EnvmapMinRed;
		public float EnvmapMinGreen;
		public float EnvmapMinBlue;
		public float EnvmapMaxLevel;
		public float EnvmapMaxRed;
		public float EnvmapMaxGreen;
		public float EnvmapMaxBlue;

		public const int SizeOf = 0x70;
	};
}
