using System.Runtime.InteropServices;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TopologyBarrierGroup // 0x0003412F
	{
		public uint Key;
		public int NumberOfBarriers;

		[MarshalAs(UnmanagedType.I1)]
		public bool IsEnabled;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x17)]
		public string Name;

		public const int SizeOf = 0x20;
	}
}
