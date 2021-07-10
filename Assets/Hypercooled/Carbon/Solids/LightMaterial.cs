using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LightMaterial
	{
		public float DiffusePower;
		public float DiffuseClamp;
		public float DiffuseFlakes;
		public float DiffuseVinylScale;
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
		public float SpecularFlakes;
		public float SpecularVinylScale;
		public float SpecularMinLevel;
		public float SpecularMinRed;
		public float SpecularMinGreen;
		public float SpecularMinBlue;
		public float SpecularMaxLevel;
		public float SpecularMaxRed;
		public float SpecularMaxGreen;
		public float SpecularMaxBlue;
		public float EnvmapPower;
		public float EnvmapClamp;
		public float EnvmapVinylScale;
		public float EnvmapMinLevel;
		public float EnvmapMinRed;
		public float EnvmapMinGreen;
		public float EnvmapMinBlue;
		public float EnvmapMaxLevel;
		public float EnvmapMaxRed;
		public float EnvmapMaxGreen;
		public float EnvmapMaxBlue;
		public float VinylLuminanceMinLevel;
		public float VinylLuminanceMaxLevel;

		public const int SizeOf = 0x98;
	};
}
