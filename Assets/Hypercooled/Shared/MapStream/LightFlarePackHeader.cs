using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LightFlarePackHeader // 0x00135101
	{
		public long Padding;
		public uint Version;
		public uint Key;
		public long NamePartition1; // this always empty, so ignore
		public long NamePartition2;
		public long NamePartition3;
		public long NamePartition4;
		public Vector4 BBoxMin;
		public Vector4 BBoxMax;
		public ushort NumLightFlares;
		public byte EndianSwapped;
		public byte Pad;
		public uint ScenerySectionNumber;
		public long LightFlarePointer;

		public const int SizeOf = 0x60;
	}
}
