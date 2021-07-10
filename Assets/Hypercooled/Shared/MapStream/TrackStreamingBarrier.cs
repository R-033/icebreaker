using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackStreamingBarrier // 0x00034112
	{
		public Vector2 Point1;
		public Vector2 Point2;

		public const int SizeOf = 0x10;
	}
}
