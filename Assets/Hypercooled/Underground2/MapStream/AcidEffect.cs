using System.Numerics;
using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AcidEffectPackHeader
	{
		public int SectionNumber;
		public int Padding;
		public int Version;
		public int NumEffects;

		public const int SizeOf = 0x10;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AcidEffect // 0x00035020
	{
		public long Padding;
		public uint Key;
		public uint EmitterKey;
		public int State;
		public int Padding0x14;
		public int Padding0x18;
		public int Padding0x1C;
		public int Padding0x20;
		public int Padding0x24;
		public short Padding0x28;
		public ushort SectionNumber;
		public uint SceneryGroupHash;
		public long Padding0x30;
		public long Padding0x38;
		public Matrix4x4 Transform;
		public long Padding0x80;
		public long Padding0x88;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
		public string Name;

		public const int SizeOf = 0xD0;
	}
}
