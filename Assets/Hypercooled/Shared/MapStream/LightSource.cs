using Hypercooled.Shared.Structures;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.MapStream
{
	public enum LightSourceType : byte
	{
		ELIGHT_AMBIENT = 0,
		ELIGHT_FREE_DIRECTIONAL = 1,
		ELIGHT_OMNI = 2,
		ELIGHT_FREE_SPOT = 3,
		ELIGHT_EXCLUDE = 4,
		ELIGHT_FREE_SPOT_HEADLIGHT = 5,
		ELIGHT_NUM_LIGHTTYPES = 6,
	}

	public enum LightSourceAttenuation : byte
	{
		ELIGHT_ATTENUATION_NONE = 0,
		ELIGHT_ATTENUATION_NEAR = 1,
		ELIGHT_ATTENUATION_FAR = 2,
		ELIGHT_ATTENUATION_INVERSE = 4,
		ELIGHT_ATTENUATION_INVERSE_SQUARE = 8,
	}

	public enum LightSourceState : byte
	{
		ELIGHT_STATE_OFF = 0,
		ELIGHT_STATE_ON = 1,
	}

	public enum LightSourceShape : byte
	{
		ELIGHT_SHAPE_CIRCLE = 0,
		ELIGHT_SHAPE_RECTANGLE = 1,
		ELIGHT_SHAPE_SPHERE = 2,
		ELIGHT_SHAPE_AABB = 3,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LightSource // 0x00135003
	{
		public uint Key;
		public LightSourceType LightType;
		public LightSourceAttenuation AttenuationType;
		public LightSourceShape Shape;
		public LightSourceState State;
		public uint ExcludeNameHash;
		public Color8UI Color;
		public Vector3 Position;
		public float Size;
		public Vector3 Direction;
		public float Intensity;
		public float FarStart;
		public float FarEnd;
		public float Falloff;
		public ushort ScenerySectionNumber;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x22)]
		public string Name;

		public const int SizeOf = 0x60;
	}
}
