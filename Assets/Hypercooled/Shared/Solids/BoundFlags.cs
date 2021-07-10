using System;

namespace Hypercooled.Shared.Solids
{
	[Flags()]
	public enum BoundFlags : ushort
	{
		BoundsDisabled = 1 << 0,
		BoundsPrimVsWorld = 1 << 1,
		BoundsPrimVsObjects = 1 << 2,
		BoundsPrimVsGround = 1 << 3,
		BoundsMeshVsGround = 1 << 4,
		BoundsInternal = 1 << 5,
		BoundsBox = 1 << 6,
		BoundsSphere = 1 << 7,
		BoundsConstraint_Conical = 1 << 8,
		BoundsConstraint_Prismatic = 1 << 9,
		BoundsJointFemale = 1 << 10,
		BoundsJointMale = 1 << 11,
		BoundsMalePost = 1 << 12,
		BoundsJointInvert = 1 << 13,
		BoundsPrimVsOwnParts = 1 << 14,
	}
}
