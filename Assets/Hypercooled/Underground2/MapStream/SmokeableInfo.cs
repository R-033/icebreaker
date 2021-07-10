using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.InteropServices;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Hypercooled.Underground2.MapStream
{
	public enum TerrainTypeInfo : uint
	{
		CAR = 0x00009955,
		DIRT = 0x001472B2,
		NONE = 0x001A076F,
		POST = 0x001B20E5,
		ROAD = 0x001C3745,
		ROCK = 0x001C378E,
		WOOD = 0x001EF6F8,
		GLASS = 0x02DA6F79,
		GRASS = 0x02DDB9BF,
		METAL = 0x03437A52,
		PLANT = 0x037D4B5E,
		DIRTROAD = 0x05C0CB78,
		XECS_PLASTIC_WALL = 0x07ACE830,
		ROAD_DRIFT = 0x0A32A3FD,
		DIRT_SHOULDER = 0x12E92957,
		XETT_ROAD_DR = 0x142EBC3E,
		XETT_ROAD_SK = 0x142EBE26,
		XETT_ROAD_SP = 0x142EBE2B,
		SEE_THROUGH_WALL = 0x1B0AE52B,
		XECI_CAR = 0x233EF8FD,
		CHAINLINK = 0x268B4950,
		XECS_SOLID_WALL = 0x2D174C3B,
		ROUGH_ROAD = 0x328803C9,
		XETT_DIRTRD_DR = 0x337B2D21,
		XETT_DIRTRD_SK = 0x337B2F09,
		XETT_DIRTRD_SP = 0x337B2F0E,
		XEM_ROADSKRED = 0x442F2967,
		ROAD_BRIDGE = 0x4BDCC871,
		XEBO_DIRT = 0x4C237E5F,
		XEBO_ROAD = 0x4C2B42F2,
		XETT_GRASS_SK = 0x5B2A22E0,
		XETT_GRASS_SP = 0x5B2A22E5,
		XETT_RRCSSW_DR = 0x5BB7A2DC,
		XETT_RRCSSW_SK = 0x5BB7A4C4,
		XETT_RRCSSW_SP = 0x5BB7A4C9,
		GRAVEL = 0x5E94FCE0,
		XETT_DEEPWATER_DR = 0x5F3196B9,
		XETT_DEEPWATER_SK = 0x5F3198A1,
		XETT_DEEPWATER_SP = 0x5F3198A6,
		XETT_PUDDLE_DR = 0x60F89076,
		XETT_PUDDLE_SK = 0x60F8925E,
		XETT_PUDDLE_SP = 0x60F89263,
		COBBLESTONE = 0x67EC38CF,
		PILLAR = 0x72F66B23,
		PUDDLE = 0x73CB0D7D,
		SOLID_WALL = 0x78E473C9,
		STAIRS = 0x7AB6DFD5,
		XETT_PUDDLE_DRSL = 0x815E90F5,
		SKID_DIRT = 0x906F30FC,
		SKID_ROAD = 0x9076F58F,
		XECS_WOOD_WALL = 0x91708F99,
		DEEP_WATER = 0x92D3247F,
		ROAD_WET = 0x9675E514,
		XECS_GLASS_WALL = 0x9D12C6FA,
		SKID_GRASS = 0x9E904149,
		SIDEWALK = 0xBBE72013,
		XEM_ROADSKBLUE = 0xCA0BB074,
		XEM_ROADSKYELW = 0xCA182E4D,
		PLASTIC = 0xD800626F,
		ROAD_SMOKE_1 = 0xD8DA78F3,
		ROAD_SMOKE_2 = 0xD8DA78F4,
		ROAD_SMOKE_3 = 0xD8DA78F5,
		WOOD_WALL = 0xE14FFD67,
		XETT_XETT_GRASS_DR = 0xE39B6DDC,
		XECS_GRASS = 0xF0F902F1,
		XETT_DEEPWATER_DRSL = 0xF1F233F8,
		METAL_GRATE = 0xF56B6604,
	}

	public enum SmokeableCategory : uint
	{
		NONE = 0x001A076F,
		CHAIR = 0x028FDAA6,
		GLASS = 0x02DA6F79,
		SANDWICH_BOARD = 0x059372F7,
		OIL_DRUM = 0x1F556CFA,
		GARBAGE_BAG = 0x41845B31,
		TRASHCAN = 0x56EAC253,
		PARKING_METER = 0x572CAE67,
		ROAD_SIGN_WOOD = 0x6955662D,
		WATER_DRUM = 0x79045559,
		MARKET_STALL = 0x85B72E02,
		TRAFFIC_CONE = 0x8965C322,
		METAL_BIG = 0x92718DC3,
		ROAD_SIGN_METAL = 0x9347D027,
		SHOPPING_CART = 0x964E2870,
		SAW_HORSE = 0xC8ED76AA,
		METAL_FENCE = 0xF55261B2,
		METAL_SMALL = 0xF641CD2A,
		WOODBOX = 0xFACC5741,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SmokeableInfo // 0x00034026
	{
		public long Padding1;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x18)]
		public string Name;

		public float Mass;
		public Vector3 Dimensions;
		public Matrix4x4 InertiaTensor;
		public Vector3 Direction;
		public int Padding2;

		public uint Key;
		public uint AnchorEmitterGroupKey;
		public uint DynamicEmitterGroupKey;
		public int NumChildren;

		public uint ChildPointer1;
		public uint ChildPointer2;
		public uint ChildPointer3;
		public uint ChildPointer4;

		public uint ChildKey1;
		public uint ChildKey2;
		public uint ChildKey3;
		public uint ChildKey4;

		public Vector3 CenterOfMass;

		public float Padding3;
		public float Lifetime;
		public float AnchorStrength;
		public float BreakingStrength;
		public float Bounciness;

		public int Padding4;

		[JsonConverter(typeof(StringEnumConverter))]
		public TerrainTypeInfo TerrainType;

		public int Padding5;

		[JsonConverter(typeof(StringEnumConverter))]
		public SmokeableCategory CategoryType;

		public float Friction;

		public float Padding6;
		public float Padding7;
		public float Padding8;

		public long DynamicEmitterList;
		public long AnchorEmitterList;

		public const int SizeOf = 0x100;
	}
}
