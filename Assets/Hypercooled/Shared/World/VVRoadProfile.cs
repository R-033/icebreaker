using CoreExtensions.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.World
{
	public class VVRoadProfile : VVResolvable // RNpf
	{
		public enum RoadLaneType : byte
		{
			Racing = 0x0,
			Traffic = 0x1,
			Drag = 0x2,
			Cop = 0x3,
			CopReckless = 0x4,
			Reset = 0x5,
			StartingGrid = 0x6,
			Any = 0x7,
			Max = 0x8,
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Data
		{
			public byte NumZones;
			public byte MiddleZone; // NumZones / 2
			public short Padding;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = kMaxLanes)]
			public VVRoadBits[] WRoadLanes;

			public const int SizeOf = 0x40;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VVRoadBits
		{
			public uint Bits;

			public const int SizeOf = 0x04;
		}

		public class VVRoadLane
		{
			public RoadLaneType Type { get; set; }
			public float Width { get; set; }
			public float Offset { get; set; }
		}

		private const int kMaxLanes = 0x0F;
		private VVRoadLane[] m_roadLanes;

		public override UppleUTag Tag => UppleUTag.WRoadProfile;
		public override int SizeOfThis => Data.SizeOf;
		public override bool DynamicSize => false;

		public byte NumLanes { get; set; }
		public VVRoadLane[] Lanes => this.m_roadLanes;

		public VVRoadProfile()
		{
			this.m_roadLanes = new VVRoadLane[kMaxLanes];
		}

		public bool AddNewLane()
		{
			if (this.NumLanes == kMaxLanes)
			{
				return false;
			}
			else
			{
				this.m_roadLanes[this.NumLanes++] = new VVRoadLane();
				return true;
			}
		}
		public bool RemoveLane(int index)
		{
			if (index < 0 || index >= this.NumLanes)
			{
				return false;
			}
			else
			{
				--this.NumLanes;

				if (index != this.NumLanes)
				{
					Array.Copy(this.m_roadLanes, index + 1, this.m_roadLanes, index, this.NumLanes - index);
				}

				this.m_roadLanes[this.NumLanes] = null;
				return true;
			}
		}

		public override void Read(BinaryReader br)
		{
			var data = br.ReadStruct<Data>();
			this.NumLanes = data.NumZones;

			if (this.m_roadLanes is null || this.m_roadLanes.Length != kMaxLanes)
			{
				this.m_roadLanes = new VVRoadLane[kMaxLanes];
			}

			for (int i = 0; i < this.NumLanes; ++i)
			{
				var bits = data.WRoadLanes[i].Bits;

				this.m_roadLanes[i] = new VVRoadLane()
				{
					Type = (RoadLaneType)(bits & 0x0F),          // : 4
					Width = ((bits << 14) >> 18) * 0.012208521f, // : 14
					Offset = (bits >> 18) * 0.012208521f,        // : 14
				};
			}
		}

		public override void Write(BinaryWriter bw)
		{
			bw.Write(this.NumLanes);
			bw.Write((byte)(this.NumLanes >> 1));
			bw.Write((ushort)0xAAAA);

			for (int i = 0; i < this.NumLanes; ++i)
			{
				var lane = this.m_roadLanes[i];
				var bits = (uint)lane.Type;
				bits |= ((uint)(lane.Width * 81.91000367694f) << 18) >> 14;
				bits |= (uint)(lane.Offset * 81.91000367694f) << 18;

				bw.Write(bits);
			}

			for (int i = this.NumLanes; i < kMaxLanes; ++i)
			{
				bw.Write(0xFEBFFAFF); // but why...
			}
		}
	}
}
