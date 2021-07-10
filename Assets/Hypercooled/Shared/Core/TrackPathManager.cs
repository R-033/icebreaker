using Newtonsoft.Json;
using Newtonsoft.Json.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public abstract class TrackPathManager
	{
		public class Point
		{
			public Vector2 Position { get; set; }
			public short LaneIndex { get; set; }
			public sbyte FlagA { get; set; }
			public sbyte FlagB { get; set; }
			public short NeighborPoint1 { get; set; }
			public short NeighborPoint2 { get; set; }
			public short NeighborPoint3 { get; set; }
			public short Unused { get; set; }
			public float Distance { get; set; }
		}

		public class Lane
		{

		}

		public class Zone
		{
			public int Type { get; set; }
			public Vector2 Position { get; set; }
			public Vector2 Direction { get; set; }
			public float Elevation { get; set; }
			public byte ZoneSource { get; set; }
			public byte CachedIndex { get; set; }
			public short VisitInfo { get; set; }
			public int Padding { get; set; }
			public Vector2 BBoxMin { get; set; }
			public Vector2 BBoxMax { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint ZoneName1 { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint ZoneName2 { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint ZoneName3 { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint ZoneName4 { get; set; }

			public List<Vector2> Points { get; }

			public Zone()
			{
				this.Points = new List<Vector2>();
			}
		}

		public class Area
		{

		}
		
		public class Barrier
		{
			public Vector2 PointStart { get; set; }
			public Vector2 PointEnd { get; set; }
			public bool IsEnabled { get; set; }
			public bool PlayerBarrier { get; set; }
			public bool LeftHanded { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint BarrierName1 { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint BarrierName2 { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint BarrierName3 { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint BarrierName4 { get; set; }
		}

		[JsonIgnore()] public abstract bool SupportsPoints { get; }
		[JsonIgnore()] public abstract bool SupportsLanes { get; }
		[JsonIgnore()] public abstract bool SupportsZones { get; }
		[JsonIgnore()] public abstract bool SupportsAreas { get; }
		[JsonIgnore()] public abstract bool SupportsBarriers { get; }

		public List<Point> Points { get; }
		public List<Lane> Lanes { get; }
		public List<Zone> Zones { get; }
		public List<Area> Areas { get; }
		public List<Barrier> Barriers { get; }

		// todo : more stuff
		public abstract void Load(BinaryReader br);
		public abstract void Save(BinaryWriter bw);

		public TrackPathManager()
		{
			this.Points = new List<Point>();
			this.Lanes = new List<Lane>();
			this.Zones = new List<Zone>();
			this.Areas = new List<Area>();
			this.Barriers = new List<Barrier>();
		}
	}
}
