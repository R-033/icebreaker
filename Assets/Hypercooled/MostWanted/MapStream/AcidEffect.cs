using System.Numerics;
using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.MapStream
{
	public struct AcidEffectPackHeader 
	{
		public int EndianSwapped;
		public int Version;
		public int NumEffects;
		public int SectionNumber;

		public const int SizeOf = 0x10;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AcidEffect // 0x0003BC00
	{
		public uint Key;
		public int Padding1;
		public int SectionNumber;
		public int Padding2;
		public Matrix4x4 Transform;

		public const int SizeOf = 0x50;
	}
}
