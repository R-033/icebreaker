using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
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

		public const int SizeOf = 0x50;

		public static string GetSectionNameFromID(ushort id)
		{
			char c = (char)(id / 100 + 0x40);
			int i = id % 100;
			return $"{c}{i}";
		}

		public static ushort GetSectionIDFromName(string name)
		{
			if (name is null || name.Length < 2) return 0;
			char c = name[0];
			if (!System.Int32.TryParse(name.Substring(1), out int i)) return 0;
			return (ushort)((ushort)(c - 0x40 * 100) + (ushort)i);
		}
	}
}
