using CoreExtensions.IO;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.World
{
	public class VVRoadSegment : VVResolvable // RNsg
	{
		public enum RoadSegmentFlags : ushort
		{
			IsDecision = 0x01,
			TrafficNotAllowed = 0x02,
			RaceRouteForward = 0x04,
			//0x08
			IsEntrance = 0x10,
			CopsXorTraffic = 0x20,
			IsOneWay = 0x40,
			IsShortcut = 0x80,
			//0x100 IsProfileInverted?
			IsEndInverted = 0x200,
			IsStartInverted = 0x400,
			IsSideRoute = 0x800,
			CrossesBarrier = 0x1000,
			CrossesDriveThruBarrier = 0x2000,
			//0x4000
			IsInRace = 0x8000,
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Data
		{
			public ushort NodeIndex1;
			public ushort NodeIndex2;
			public ushort Length;
			public short RoadID;
			public short Index;
			public RoadSegmentFlags Flags;
			public ushort EndHandleLength;
			public ushort StartHandleLength;
			public byte EndHandleX;
			public byte EndHandleZ;
			public byte EndHandleY;
			public byte StartHandleX;
			public byte StartHandleZ;
			public byte StartHandleY;

			public const int SizeOf = 0x16;
		}

		public override UppleUTag Tag => UppleUTag.WRoadSegment;
		public override int SizeOfThis => Data.SizeOf;
		public override bool DynamicSize => false;

		public ushort NodeIndex1 { get; set; }
		public ushort NodeIndex2 { get; set; }
		public ushort Length { get; set; }
		public short RoadID { get; set; }
		public short Index { get; set; }
		public RoadSegmentFlags Flags { get; set; }
		public ushort StartHandleLength { get; set; }
		public ushort EndHandleLength { get; set; }
		public Vector3 StartHandle { get; set; }
		public Vector3 EndHandle { get; set; }

		public VVRoadSegment()
		{
		}

		public override void Read(BinaryReader br)
		{
			var data = br.ReadUnmanaged<Data>();

			this.NodeIndex1 = data.NodeIndex1;
			this.NodeIndex2 = data.NodeIndex2;
			this.Length = data.Length;
			this.RoadID = data.RoadID;
			this.Index = data.Index;
			this.Flags = data.Flags;
			this.StartHandleLength = data.StartHandleLength;
			this.EndHandleLength = data.EndHandleLength;
			this.StartHandle = new Vector3(data.StartHandleX, data.StartHandleY, data.StartHandleZ);
			this.EndHandle = new Vector3(data.EndHandleX, data.EndHandleY, data.EndHandleZ);
		}

		public override void Write(BinaryWriter bw)
		{
			var data = new Data()
			{
				NodeIndex1 = this.NodeIndex1,
				NodeIndex2 = this.NodeIndex2,
				Length = this.Length,
				RoadID = this.RoadID,
				Index = this.Index,
				Flags = this.Flags,
				StartHandleLength = this.StartHandleLength,
				EndHandleLength = this.EndHandleLength,
				StartHandleX = (byte)this.StartHandle.x,
				StartHandleY = (byte)this.StartHandle.y,
				StartHandleZ = (byte)this.StartHandle.z,
				EndHandleX = (byte)this.EndHandle.x,
				EndHandleY = (byte)this.EndHandle.y,
				EndHandleZ = (byte)this.EndHandle.z,
			};

			bw.WriteUnmanaged(data);
		}
	}
}
