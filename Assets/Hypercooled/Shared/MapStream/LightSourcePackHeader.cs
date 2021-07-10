using System.Runtime.InteropServices;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LightSourcePackHeader // 0x00135001
	{
		public long Padding;
		public short Version;
		public byte EndianSwapped;
		public byte Pad;
		public int ScenerySectionNumber;
		public int LightTreePointer;
		public int NumTreeNodes;
		public int LightArrayPointer;
		public int NumLights;

		public const int SizeOf = 0x20;
	}
}
