using System;

namespace Hypercooled.Shared.Solids
{
	[Flags()]
	public enum SolidInfoFlags : short
	{
		ESolidCompressedVerts = 0x01,
		ESolidShadowMap = 0x08,
		ESolidVertexAnimation = 0x10,
		ESolidRandomizeStartFrame = 0x20,
		ESolidIsLit = 0x40,
		ESolidIsWindy = 0x80,
		ESolidDuplicateName = 0x100,
		ESolidDuplicateNameError = 0x200,
		ESolidDuplicated = 0x400,
		ESolidWantSpotlightContext = 0x800,
		ESolidMorphInitialized = 0x1000,
		ESolidSkinInfoCreated = 0x2000,
		ESolidPixelDamageCleared = 0x4000,
	};
}
