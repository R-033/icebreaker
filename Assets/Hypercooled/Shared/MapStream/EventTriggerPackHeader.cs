using System.Runtime.InteropServices;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct EventTriggerPackHeader // 0x00036001
	{
		public long Padding;
		public int Version;
		public int ScenerySectionNumber;
		public int NumEventTriggers;
		public int EndianSwapped;
		public int EventTreePointer;
		public int EventArray;

		public const int SizeOf = 0x20;
	}
}
