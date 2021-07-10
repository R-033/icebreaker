using System;



namespace Hypercooled.Utils.DDS
{
    [Flags()]
    public enum DDS_HEADER_FLAGS : uint
    {
        DDS_HEIGHT = 0x00000002,  // DDSD_HEIGHT
        DDS_WIDTH = 0x00000004,  // DDSD_WIDTH
        PITCH = 0x00000008,  // DDSD_PITCH
        TEXTURE = 0x00001007,  // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT
        MIPMAP = 0x00020000,  // DDSD_MIPMAPCOUNT
        LINEARSIZE = 0x00080000,  // DDSD_LINEARSIZE
        VOLUME = 0x00800000,  // DDSD_DEPTH
    }
}
