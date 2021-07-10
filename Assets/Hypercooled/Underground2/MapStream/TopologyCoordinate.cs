using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
{
	public enum TopologyMaterial : byte
	{
		NONE = 0,
		ROAD = 1,
		ROAD_WET = 2,
		ROAD_DRIFT = 3,
		ROAD_SMOKE_1 = 4,
		ROAD_SMOKE_2 = 5,
		ROAD_SMOKE_3 = 6,
		BRIDGE = 7,
		DIRT = 8,
		GRAVEL = 9,
		ROUGH_ROAD = 10,
		COBBLESTONE = 11,
		STAIRS = 12,
		PUDDLE = 13,
		DEEP_WATER = 14,
		GRASS = 15,
		SIDEWALK = 16,
		WOOD = 17,
		PLASTIC = 18,
		GLASS = 19,
		SOLID_WALL = 20,
		SEE_THROUGH_WALL = 21,
		PLANT = 22,
		POST = 23,
		PILLAR = 24,
		METAL_GRATE = 25,
		METAL = 26,
		CHAINLINK = 27,
		CAR = 28,
	};

	[Flags()]
	public enum TopologyQuadForm : byte
	{
		OVERLAPPING = 0x01,
		VERTICAL = 0x02,
		INFINITELY_HIGH = 0x04,
		UPSIDE_DOWN = 0x08,
		HAS_4_VERTICES = 0x10,
		USED_BY_NEIGHBOUR = 0x20,
		OVERLAPPING_ABOVE = 0x40,
	}
	
	[StructLayout(LayoutKind.Sequential)]
	public struct TopologyCoordinate // 0x00034134
	{
		public byte BarrierIndex;
		public byte ShapeForm;
		public TopologyMaterial Material;
		public TopologyQuadForm Flags;
		public short YAddon;
		public ushort FFFF;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
		public short[] XCoordinates;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
		public short[] ZCoordinates;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
		public short[] YCoordinates;

		public const int SizeOf = 0x20;

		private int InternalGetMinXIndex()
		{
			int result = 3;
			var comparer = this.XCoordinates[3];

			for (int i = 2; i >= 0; --i)
			{
				var value = this.XCoordinates[i];

				if (value < comparer)
				{
					result = i;
					comparer = value;
				}
			}
			
			return result;
		}
		private int InternalGetMinZIndex()
		{
			int result = 3;
			var comparer = this.ZCoordinates[3];

			for (int i = 2; i >= 0; --i)
			{
				var value = this.ZCoordinates[i];

				if (value < comparer)
				{
					result = i;
					comparer = value;
				}
			}

			return result;
		}
		private int InternalGetMaxXIndex()
		{
			int result = 3;
			var comparer = this.XCoordinates[3];

			for (int i = 2; i >= 0; --i)
			{
				var value = this.XCoordinates[i];

				if (value > comparer)
				{
					result = i;
					comparer = value;
				}
			}

			return result;
		}
		private int InternalGetMaxZIndex()
		{
			int result = 3;
			var comparer = this.ZCoordinates[3];

			for (int i = 2; i >= 0; --i)
			{
				var value = this.ZCoordinates[i];

				if (value > comparer)
				{
					result = i;
					comparer = value;
				}
			}

			return result;
		}

		public Vector3 GetCoordinate1Vector3()
		{
			return new Vector3(this.XCoordinates[0] / 8.0f, (float)(this.YCoordinates[0] * 0.00390625f + YAddon), this.ZCoordinates[0] / 8.0f);
		}
		public Vector3 GetCoordinate2Vector3()
		{
			return new Vector3(this.XCoordinates[1] / 8.0f, (float)(this.YCoordinates[1] * 0.00390625f + YAddon), this.ZCoordinates[1] / 8.0f);
		}
		public Vector3 GetCoordinate3Vector3()
		{
			return new Vector3(this.XCoordinates[2] / 8.0f, (float)(this.YCoordinates[2] * 0.00390625f + YAddon), this.ZCoordinates[2] / 8.0f);
		}
		public Vector3 GetCoordinate4Vector3()
		{
			return new Vector3(this.XCoordinates[3] / 8.0f, (float)(this.YCoordinates[3] * 0.00390625f + YAddon), this.ZCoordinates[3] / 8.0f);
		}

		public Vector2 GetCoordinate1Vector2()
		{
			return new Vector2(this.XCoordinates[0] / 8.0f, this.ZCoordinates[0] / 8.0f);
		}
		public Vector2 GetCoordinate2Vector2()
		{
			return new Vector2(this.XCoordinates[1] / 8.0f, this.ZCoordinates[1] / 8.0f);
		}
		public Vector2 GetCoordinate3Vector2()
		{
			return new Vector2(this.XCoordinates[2] / 8.0f, this.ZCoordinates[2] / 8.0f);
		}
		public Vector2 GetCoordinate4Vector2()
		{
			return new Vector2(this.XCoordinates[3] / 8.0f, this.ZCoordinates[3] / 8.0f);
		}

		public byte GetShapeValue()
		{
			var xMin = this.InternalGetMinXIndex();
			var zMin = this.InternalGetMinZIndex();
			var xMax = this.InternalGetMaxXIndex();
			var zMax = this.InternalGetMaxZIndex();
			return (byte)((xMin) | (zMin << 2) | (xMax << 4) | (zMax << 6));
		}

		public short GetMinXValue()
		{
			return this.XCoordinates[(this.ShapeForm >> 0) & 0x03];
		}
		public short GetMinZValue()
		{
			return this.ZCoordinates[(this.ShapeForm >> 2) & 0x03];
		}
		public short GetMaxXValue()
		{
			return this.XCoordinates[(this.ShapeForm >> 4) & 0x03];
		}
		public short GetMaxZValue()
		{
			return this.ZCoordinates[(this.ShapeForm >> 6) & 0x03];
		}

		public int GetMinXIndex()
		{
			return (this.ShapeForm >> 0) & 0x03;
		}
		public int GetMinZIndex()
		{
			return (this.ShapeForm >> 2) & 0x03;
		}
		public int GetMaxXIndex()
		{
			return (this.ShapeForm >> 4) & 0x03;
		}
		public int GetMaxZIndex()
		{
			return (this.ShapeForm >> 6) & 0x03;
		}

		public static TopologyCoordinate CreateNew()
		{
			return new TopologyCoordinate()
			{
				XCoordinates = new short[4],
				YCoordinates = new short[4],
				ZCoordinates = new short[4],
			};
		}
	}
}
