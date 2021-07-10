using System.Runtime.InteropServices;

namespace Hypercooled.MostWanted.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryOverrideInfo // 0x00034108
	{
		public ushort SectionNumber;
		public ushort InstanceIndex;
		public ushort ExcludeFlags;

		public const int SizeOf = 0x06;
	}
}
