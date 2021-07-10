using Newtonsoft.Json;
using Newtonsoft.Json.Unity;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackPathBarrier // 0x0003414D
	{
		public Vector2 Point1;
		public Vector2 Point2;

		[MarshalAs(UnmanagedType.U1)]
		public bool IsEnabled;

		public byte NumKeys;
		public byte Flag1;
		public byte Flag2;

		[JsonConverter(typeof(BinStringConverter))]
		public uint BarrierKey1;

		[JsonConverter(typeof(BinStringConverter))]
		public uint BarrierKey2;

		[JsonConverter(typeof(BinStringConverter))]
		public uint BarrierKey3;

		[JsonConverter(typeof(BinStringConverter))]
		public uint BarrierKey4;

		public const int SizeOf = 0x24;
	}
}
