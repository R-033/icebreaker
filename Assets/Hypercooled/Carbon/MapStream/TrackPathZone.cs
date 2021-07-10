using CoreExtensions.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Unity;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Carbon.MapStream
{
	public enum TrackPathZoneType : int
	{
		TRACK_PATH_ZONE_RESET = 0x0,
		TRACK_PATH_ZONE_RESET_TO_POINT = 0x1,
		TRACK_PATH_ZONE_GUIDED_RESET = 0x2,
		TRACK_PATH_ZONE_TUNNEL = 0x3,
		TRACK_PATH_ZONE_OVERPASS = 0x4,
		TRACK_PATH_ZONE_OVERPASS_SMALL = 0x5,
		TRACK_PATH_ZONE_STREAMER_PREDICTION = 0x6,
		TRACK_PATH_ZONE_GARAGE = 0x7,
		TRACK_PATH_ZONE_HIDDEN = 0x8,
		TRACK_PATH_ZONE_TRAFFIC_PATTERN = 0x9,
		TRACK_PATH_ZONE_DYNAMIC = 0xA,
		TRACK_PATH_ZONE_NEIGHBOURHOOD = 0xB,
		TRACK_PATH_ZONE_JUMP_CAM = 0xC,
		TRACK_PACH_ZONE_NO_COP_SPAWN = 0xD,
		TRACK_PATH_ZONE_PURSUIT_START = 0xE,
		TRACK_PATH_ZONE_HIGHWAY = 0xF,
		TRACK_PATH_ZONE_CANYON_DROP = 0x10,
		TRACK_PATH_ZONE_VERTIGO_CAMERA = 0x11,
		NUM_TRACK_PATH_ZONES = 0x12,
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackPathZoneInternal
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public TrackPathZoneType ZoneType;

		public Vector2 Position;
		public Vector2 Direction;
		public float Elevation;
		public byte ZoneSource;
		public byte CachedIndex;
		public short VisitInfo;
		public int Padding;
		public Vector2 BBoxMin;
		public Vector2 BBoxMax;

		[JsonConverter(typeof(BinStringConverter))]
		public uint ZoneKey1;

		[JsonConverter(typeof(BinStringConverter))]
		public uint ZoneKey2;

		[JsonConverter(typeof(BinStringConverter))]
		public uint ZoneKey3;

		[JsonConverter(typeof(BinStringConverter))]
		public uint ZoneKey4;

		public ushort NumPoints;
		public ushort TotalSize;

		public const int SizeOf = 0x44;
	}

	public class TrackPathZone // 0x0003414A
	{
		private Vector2[] m_points;

		public TrackPathZoneInternal ZoneData { get; set; }

		public Vector2[] Points
		{
			get => this.m_points;
			set => this.m_points = value is null ? new Vector2[0] : value;
		}

		public TrackPathZone()
		{
			this.m_points = new Vector2[0];
		}

		public void Read(BinaryReader br)
		{
			this.ZoneData = br.ReadUnmanaged<TrackPathZoneInternal>();
			this.m_points = new Vector2[this.ZoneData.NumPoints];

			for (int i = 0; i < this.ZoneData.NumPoints; ++i) this.m_points[i] = br.ReadUnmanaged<Vector2>();
		}
	}
}
