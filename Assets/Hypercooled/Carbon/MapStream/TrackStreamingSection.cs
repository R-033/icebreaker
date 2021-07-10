using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Carbon.MapStream
{
	public enum TrackStreamingStatus : int
	{
		Unloaded = 0,
		Allocated = 1,
		Loading = 2,
		Loaded = 3,
		Activated = 4,
	};

	public enum TrackStreamingFileType : int
	{
		TrackStreamingFilePermanent = 0,
		TrackStreamingFileRegion = 1,
		NumTrackStreamingFileTypes = 2,
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackStreamingSection // 0x00034110
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x08)]
		public string SectionName;

		public ushort SectionNumber;
		public byte WasRendered;
		public byte CurrentlyVisible;
		public TrackStreamingStatus Status;
		public TrackStreamingFileType FileType;
		public int FileOffset;
		public int Size;
		public int CompressedSize;
		public int PermSize;
		public int SectionPriority;
		public Vector2 Center;
		public float Radius;
		public uint Checksum;
		public int LastNeededTimestamp;
		public uint UnactivatedFrameCount;
		public int LoadedTime;
		public int BaseLoadingPriority;
		public int LoadingPriority;
		public int MemoryPointer;
		public int DiscBundlePointer;
		public int LoadedSize;
		public int BoundaryPointer;

		public const int SizeOf = 0x5C;
	}
}
