using Newtonsoft.Json;
using Newtonsoft.Json.Unity;
using System.Runtime.InteropServices;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackObjectBounds // 0x00034191
	{
		[JsonConverter(typeof(BinStringConverter))]
		public uint Key;

		public int Padding1;
		public int Padding2;
		public int Padding3;

		public Matrix4x4 Matrix;
		public Vector3 Dimensions;
		public int Padding4;

		public const int SizeOf = 0x60;
	}
}
