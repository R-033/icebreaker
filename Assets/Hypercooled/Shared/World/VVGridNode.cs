using CoreExtensions.IO;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.World
{
	public class VVGridNode : VVResolvable // CGcn
	{
		public enum ElemType
		{
			Instance = 0x0,
			Trigger = 0x1,
			Object = 0x2,
			RoadSegment = 0x3,
			ElemTypeCount = 0x4
		};

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VVGridNodeHeader
		{
			public int Pointer;
			public ushort NodeIndex;
			public ushort Island; // Pad?

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
			public byte[] ElementCounts;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
			public ushort[] ElementOffsets;

			public const int SizeOf = 0x14;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VVGridNodeInstance
		{
			public ushort Index;
			public ushort Instance;

			public const int SizeOf = 0x04;

			public static readonly VVGridNodeInstance[] Empty = new VVGridNodeInstance[0];
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VVGridNodeRoadSegment
		{
			public int RoadSegment;

			public const int SizeOf = 0x04;

			public static readonly VVGridNodeRoadSegment[] Empty = new VVGridNodeRoadSegment[0];
		}

		public override UppleUTag Tag => UppleUTag.WGridNode;
		public override int SizeOfThis => this.InternalGetSizeOfThis();
		public override bool DynamicSize => true;

		public ushort NodeIndex { get; set; }
		public ushort Island { get; set; }

		public List<VVGridNodeInstance> Instances { get; set; }
		public List<VVGridNodeRoadSegment> RoadSegments { get; set; }

		public VVGridNode()
		{
			this.Instances = new List<VVGridNodeInstance>();
			this.RoadSegments = new List<VVGridNodeRoadSegment>();
		}

		public override void Read(BinaryReader br)
		{
			var header = br.ReadStruct<VVGridNodeHeader>();

			this.NodeIndex = header.NodeIndex;
			this.Island = header.Island;

			var start = br.BaseStream.Position;
			var end = br.BaseStream.Position;

			if (header.ElementCounts[0] > 0)
			{
				br.BaseStream.Position = start + header.ElementOffsets[0];

				for (int i = 0; i < header.ElementCounts[0]; ++i)
				{
					this.Instances.Add(br.ReadUnmanaged<VVGridNodeInstance>());
				}
			}
			if (header.ElementCounts[1] > 0)
			{
				UnityEngine.Debug.LogWarning($"WGridNode at 0x{br.BaseStream.Position:X8} has nonzero Trigger count!");
				br.BaseStream.Position = start + header.ElementOffsets[1];

				for (int i = 0; i < header.ElementCounts[1]; ++i)
				{
					
				}
			}
			if (header.ElementCounts[2] > 0)
			{
				UnityEngine.Debug.LogWarning($"WGridNode at 0x{br.BaseStream.Position:X8} has nonzero Object count!");
				br.BaseStream.Position = start + header.ElementOffsets[2];

				for (int i = 0; i < header.ElementCounts[2]; ++i)
				{
					
				}
			}
			if (header.ElementCounts[3] > 0)
			{
				br.BaseStream.Position = start + header.ElementOffsets[3];

				for (int i = 0; i < header.ElementCounts[3]; ++i)
				{
					this.RoadSegments.Add(br.ReadUnmanaged<VVGridNodeRoadSegment>());
				}
			}

			end += header.ElementCounts[0] * VVGridNodeInstance.SizeOf;
			//end += header.ElementCounts[1] * ???;
			//end += header.ElementCounts[2] * ???;
			end += header.ElementCounts[3] * VVGridNodeRoadSegment.SizeOf;

			br.BaseStream.Position = end;
		}

		public override void Write(BinaryWriter bw)
		{
			var header = new VVGridNodeHeader()
			{
				Pointer = 0,
				NodeIndex = this.NodeIndex,
				Island = this.Island,
				ElementCounts = new byte[0x04],
				ElementOffsets = new ushort[0x04],
			};

			header.ElementCounts[0] = (byte)this.Instances.Count;
			header.ElementCounts[3] = (byte)this.RoadSegments.Count;

			header.ElementOffsets[0] = 0;
			header.ElementOffsets[3] = (ushort)(this.Instances.Count * VVGridNodeInstance.SizeOf);
			header.ElementOffsets[1] = header.ElementOffsets[3];
			header.ElementOffsets[2] = header.ElementOffsets[3];

			bw.WriteStruct(header);

			for (int i = 0; i < this.Instances.Count; ++i)
			{
				bw.WriteUnmanaged(this.Instances[i]);
			}

			for (int i = 0; i < this.RoadSegments.Count; ++i)
			{
				bw.WriteUnmanaged(this.RoadSegments[i]);
			}
		}

		private int InternalGetSizeOfThis()
		{
			int result = VVGridNodeHeader.SizeOf;
			result += this.Instances.Count * VVGridNodeInstance.SizeOf;
			result += this.RoadSegments.Count * VVGridNodeRoadSegment.SizeOf;

			return result;
		}
	}
}
