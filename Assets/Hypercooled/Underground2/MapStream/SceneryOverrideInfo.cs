using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryOverrideInfo // 0x00034107
	{
		public ushort SectionNumber;
		public ushort InstanceIndex;
		public ushort ExcludeFlags;
		public ushort SecondaryFlags;

		public const int SizeOf = 0x08;
	}
}
