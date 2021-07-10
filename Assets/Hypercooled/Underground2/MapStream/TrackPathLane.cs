using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackPathLanePointer
	{
		public ushort LaneIndex;

		[MarshalAs(UnmanagedType.U1)]
		public bool IsInReverse;

		public byte Unused;
		public int Pointer;

		public const int SizeOf = 0x08;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackPathLane // 0x00034149
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
		public string Name;
		public uint Key;
		public int Index;
		public int TrackPathPointTablePointer;
		public short StartPointIndex;
		public short EndPointIndex;
		public short NumPoints;
		public byte NumNextReverseLanes;
		public byte NumNextForwardLanes;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
		public TrackPathLanePointer[] NextReverseLanes;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
		public TrackPathLanePointer[] NextForwardLanes;

		public float Single0x64;
		public int ProcessType;
		public float StartToEndDistance;
		public float Single0x70;
		public float StartPointDistance;
		public float EndPointDistance;
		public float Neg1OrPos1BasedOnProcessType;
		public float ReversedStartPointDistanceIfProcessTypeIs1;
		public float ReversedEndPointDistanceIfProcessTypeIs1;
		public Vector2 StartPointPosition;
		public Vector2 EndPointPosition;
		public Vector2 BBoxMin;
		public Vector2 BBoxMax;
		public int QuicksplinePointer;

		public byte NumTrackIDsForward;
		public byte NumTrackIDsReverse;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x07)]
		public ushort[] TrackIDs;

		public ushort NumSomeNumbers;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x0F)]
		public ushort[] SomeNumbers;

		public const int SizeOf = 0xDC;
	}
}
