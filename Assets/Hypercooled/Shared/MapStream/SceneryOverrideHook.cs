using System.Runtime.InteropServices;

namespace Hypercooled.Shared.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SceneryOverrideHook // 0x00034105 (U2) | 0x00034106 (MW & C)
	{
		public ushort OverrideInfoIndex;
		public ushort InstanceIndex;

		public const int SizeOf = 0x04;
	}
}
