using CoreExtensions.IO;
using System.IO;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.World
{
	public class VVRoadNetworkInfo : VVResolvable // RNhd
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Data // RNhd
		{
			public ushort NumProfiles;
			public ushort NumNodes;
			public ushort NumSegments;
			public ushort NumIntersections;
			public ushort NumRoads;
			public ushort NumJunctions;
			public ushort Padding;

			public const int SizeOf = 0x0E;
		}

		public override UppleUTag Tag => UppleUTag.WRoadNetworkInfo;
		public override int SizeOfThis => Data.SizeOf;
		public override bool DynamicSize => false;

		public ushort NumProfiles { get; set; }
		public ushort NumNodes { get; set; }
		public ushort NumSegments { get; set; }
		public ushort NumIntersections { get; set; }
		public ushort NumRoads { get; set; }
		public ushort NumJunctions { get; set; }

		public override void Read(BinaryReader br)
		{
			var data = br.ReadUnmanaged<Data>();

			this.NumProfiles = data.NumProfiles;
			this.NumNodes = data.NumNodes;
			this.NumSegments = data.NumSegments;
			this.NumIntersections = data.NumIntersections;
			this.NumRoads = data.NumRoads;
			this.NumJunctions = data.NumJunctions;
		}

		public override void Write(BinaryWriter bw)
		{
			var data = new Data()
			{
				NumProfiles = this.NumProfiles,
				NumNodes = this.NumNodes,
				NumSegments = this.NumSegments,
				NumIntersections = this.NumIntersections,
				NumRoads = this.NumRoads,
				NumJunctions = this.NumJunctions,
				Padding = 0,
			};

			bw.WriteUnmanaged(data);
		}
	}
}
