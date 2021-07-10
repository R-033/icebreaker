using System;



namespace Hypercooled.Utils.DDS
{
	[Flags()]
	public enum DDS_SURFACE : uint
	{
		SURFACE_FLAGS_CUBEMAP = 0x00000008, // DDSCAPS_COMPLEX
		SURFACE_FLAGS_TEXTURE = 0x00001000, // DDSCAPS_TEXTURE
		SURFACE_FLAGS_MIPMAP = 0x00400008, // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
		SURFACE_FLAGS_ALL = SURFACE_FLAGS_MIPMAP | SURFACE_FLAGS_TEXTURE,
	}
}
