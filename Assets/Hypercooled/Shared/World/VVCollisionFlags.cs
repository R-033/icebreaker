using System;

namespace Hypercooled.Shared.World
{
	[Flags()]
	public enum VVCollisionFlags : byte
	{
		YVecNotUp = 0x01,
		Dynamic = 0x02,
		Disabled = 0x04,
		NoTrafficFlag = 0x40,
		NoCopFlag = 0x80,
		ExclusionBits = NoTrafficFlag | NoCopFlag,
	};
}
