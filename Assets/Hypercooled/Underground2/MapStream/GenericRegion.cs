using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
{
	public enum RegionType : int
	{
		NullRegion = -1,
		RegionRain = 0,
		RegionFog = 1,
		RegionSnow = 2,
		RegionWind = 3,
		RegionClear = 4,
		RegionTunnel = 5,
		RegionHorizonColor = 6,
		RegionFogVolume = 7,
		RegionBloom = 8,
		NumRegionTypes = 9,
		AllRegions = 10,
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct GenericRegion // 0x00034250
	{
		public long Padding;
		public RegionType Type;
		public float Red;
		public float Green;
		public float Blue;
		public float FogStart;
		public float Parameter;
		public Vector3 Position;
		public float Radius;
		public float Intensity;
		public float FarFalloffStart;
		public float FogFalloff;
		public Vector2 FogFalloffVec;
		public Vector2 ScreenPos;
		public int Shape;
		public int Blend;
		public float Modifier;
		public uint InFlags;
		public float Effect;
		public int Dynamic;

		public const int SizeOf = 0x64;
	}
}
