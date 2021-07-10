using CoreExtensions.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.World
{
	public class VVRoadNode : VVResolvable // RNnd
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Data
		{
			public Vector3 Position;
			public short Index;
			public short ProfileIndex;
			public byte NumSegments;
			public byte Padding;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = kMaxSegments)]
			public ushort[] SegmentIndices;

			public const int SizeOf = 0x20;
		}

		private const int kMaxSegments = 0x07;
		private ushort[] m_segmentIndices;

		public override UppleUTag Tag => UppleUTag.WRoadNode;
		public override int SizeOfThis => Data.SizeOf;
		public override bool DynamicSize => false;

		public Vector3 Position { get; set; }
		public short Index { get; set; }
		public short ProfileIndex { get; set; }
		public byte NumSegments { get; set; }
		public ushort[] SegmentIndices => this.m_segmentIndices;

		public VVRoadNode()
		{
			this.m_segmentIndices = new ushort[kMaxSegments];
		}

		public bool AddNewSegment(ushort index)
		{
			if (this.NumSegments == kMaxSegments)
			{
				return false;
			}
			else
			{
				this.m_segmentIndices[this.NumSegments++] = index;
				return true;
			}
		}
		public bool RemoveLane(int index)
		{
			if (index < 0 || index >= this.NumSegments)
			{
				return false;
			}
			else
			{
				--this.NumSegments;

				if (index != this.NumSegments)
				{
					Array.Copy(this.m_segmentIndices, index + 1, this.m_segmentIndices, index, this.NumSegments - index);
				}

				this.m_segmentIndices[this.NumSegments] = 0xAAAA;
				return true;
			}
		}

		public override void Read(BinaryReader br)
		{
			var data = br.ReadStruct<Data>();

			this.Position = data.Position;
			this.Index = data.Index;
			this.ProfileIndex = data.ProfileIndex;
			this.NumSegments = data.NumSegments;
			this.m_segmentIndices = data.SegmentIndices;
		}

		public override void Write(BinaryWriter bw)
		{
			bw.WriteUnmanaged(this.Position);
			bw.Write(this.Index);
			bw.Write(this.ProfileIndex);
			bw.Write(this.NumSegments);
			bw.Write((byte)0xAA);

			for (int i = 0; i < this.NumSegments; ++i)
			{
				bw.Write(this.SegmentIndices[i]);
			}

			for (int i = this.NumSegments; i < kMaxSegments; ++i)
			{
				bw.Write((ushort)0xAAAA);
			}
		}
	}
}
