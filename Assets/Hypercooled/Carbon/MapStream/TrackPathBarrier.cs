using Newtonsoft.Json;
using Newtonsoft.Json.Unity;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackPathBarrier // 0x0003414D
	{
		public Vector2 Point1;
		public Vector2 Point2;

		[MarshalAs(UnmanagedType.U1)]
		public bool IsEnabled;

		public byte Padding;

		[MarshalAs(UnmanagedType.U1)]
		public bool PlayerBarrier;

		[MarshalAs(UnmanagedType.U1)]
		public bool LeftHanded;

		[JsonConverter(typeof(BinStringConverter))]
		public uint BarrierKey;

		public const int SizeOf = 0x18;
	}
}
