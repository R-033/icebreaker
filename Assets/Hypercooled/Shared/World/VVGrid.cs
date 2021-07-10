using CoreExtensions.IO;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.World
{
	public class VVGrid : VVResolvable // CGrd
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Data
		{
			public Vector3 Min;
			public float One; // 1
			public float EdgeSize;
			public float InvertedEdgeSize; // 1 / EdgeSize
			public int NumRows;
			public int NumColumns;
			public int Pointer;

			[MarshalAs(UnmanagedType.U1)]
			public bool AllocatedNodesLocally;

			[MarshalAs(UnmanagedType.U1)]
			public bool NodesPopulated;

			public short Padding;

			public const int SizeOf = 0x28;
		}

		public override UppleUTag Tag => UppleUTag.WGrid;
		public override int SizeOfThis => Data.SizeOf;
		public override bool DynamicSize => false;

		public Vector3 Minimum { get; set; }
		public float EdgeSize { get; set; }
		public int NumRows { get; set; }
		public int NumColumns { get; set; }
		public bool AllocatedNodesLocally { get; set; }
		public bool NodesPopulated { get; set; }

		public override void Read(BinaryReader br)
		{
			var data = br.ReadUnmanaged<Data>();

			this.Minimum = data.Min;
			this.EdgeSize = data.EdgeSize;
			this.NumRows = data.NumRows;
			this.NumColumns = data.NumColumns;
			this.AllocatedNodesLocally = data.AllocatedNodesLocally;
			this.NodesPopulated = data.NodesPopulated;
		}

		public override void Write(BinaryWriter bw)
		{
			var data = new Data()
			{
				Min = this.Minimum,
				One = 1.0f,
				EdgeSize = this.EdgeSize,
				InvertedEdgeSize = 1 / this.EdgeSize,
				NumRows = this.NumRows,
				NumColumns = this.NumColumns,
				Pointer = 0,
				AllocatedNodesLocally = true,
				NodesPopulated = false,
				Padding = 0,
			};

			bw.WriteUnmanaged(data);
		}
	}
}
