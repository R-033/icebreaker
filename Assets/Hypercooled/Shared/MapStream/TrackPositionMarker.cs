using Newtonsoft.Json;
using Newtonsoft.Json.Unity;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackPositionMarker // 0x00034146
	{
		public long Padding;

		[JsonConverter(typeof(BinStringConverter))]
		public uint Key;

		public int Param;
		public Vector4 Position;
		public int TrackNumber;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x09)]
		[JsonConverter(typeof(ByteArrayConverter))]
		public byte[] SceneryGroupNumbers;

		public byte NumSceneryGroups;
		public ushort Angle;

		public const int SizeOf = 0x30;
	}
}
