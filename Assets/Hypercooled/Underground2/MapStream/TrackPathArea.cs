using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TrackPathArea // 0x0003414C
	{
		public ushort TrackID;
		public byte NumPoints;

		[MarshalAs(UnmanagedType.U1)]
		public bool InReverse;

		[MarshalAs(UnmanagedType.U1)]
		public bool SomeBool;

		public byte SomeByte;
		public short Padding;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
		public Vector2[] Points;

		public int PathIndex;
		public int Pointer;

		public const int SizeOf = 0x110;
	}
}
