namespace Hypercooled.Utils.DDS
{
    /// <summary>
    /// DDS static class with methods to make <see cref="DDS_PIXELFORMAT"/>.
    /// </summary>
    internal static class DDS_CONST
    {
        // Standard MAKEFOURCC that requires bytes to be passed
        public static uint MAKEFOURCC(byte c1, byte c2, byte c3, byte c4)
        {
            return (uint)(c1) | ((uint)(c2) << 8) | ((uint)(c3) << 16) | ((uint)(c4) << 24);
        }

        // MAKEFOURCC that uses ReinterpretCast to cast any passed objects to it into uint
        public static uint MAKEFOURCC_R(object c1, object c2, object c3, object c4)
        {
			var a1 = System.Convert.ToUInt32(c1);
            var a2 = System.Convert.ToUInt32(c2);
            var a3 = System.Convert.ToUInt32(c3);
            var a4 = System.Convert.ToUInt32(c4);
            return a1 | a2 << 8 | a3 << 16 | a4 << 24;
        }

        public static void DDSPF_DXT1(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('D', 'X', 'T', '1');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_DXT2(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('D', 'X', 'T', '2');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_DXT3(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('D', 'X', 'T', '3');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_DXT4(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('D', 'X', 'T', '4');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_DXT5(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('D', 'X', 'T', '5');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_DX10(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('D', 'X', '1', '0');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_BC4_UNORM(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('B', 'C', '4', 'U');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_BC4_SNORM(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('B', 'C', '4', 'S');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_BC5_UNORM(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('B', 'C', '5', 'U');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_BC5_SNORM(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('B', 'C', '5', 'S');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_R8G8_B8G8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('R', 'G', 'B', 'G');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_G8R8_G8B8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('G', 'R', 'G', 'B');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_YUY2(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.FOURCC;
            _PIXELFORMAT.dwFourCC = MAKEFOURCC_R('Y', 'U', 'Y', '2');
            _PIXELFORMAT.dwRGBBitCount = 0;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_PAL4(DDS_PIXELFORMAT _PIXELFORMAT)
		{
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.PAL4;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 4;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_PAL4A(DDS_PIXELFORMAT _PIXELFORMAT)
		{
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.PAL4A;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 4;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_PAL8(DDS_PIXELFORMAT _PIXELFORMAT)
		{
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.PAL8;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 8;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_PAL8A(DDS_PIXELFORMAT _PIXELFORMAT)
		{
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.PAL8A;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 8;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_A8R8G8B8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.RGBA;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 32;
            _PIXELFORMAT.dwRBitMask = 0x00FF0000;
            _PIXELFORMAT.dwGBitMask = 0x0000FF00;
            _PIXELFORMAT.dwBBitMask = 0x000000FF;
            _PIXELFORMAT.dwABitMask = 0xFF000000;
        }

        public static void DDSPF_X8R8G8B8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.RGB;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 32;
            _PIXELFORMAT.dwRBitMask = 0x00FF0000;
            _PIXELFORMAT.dwGBitMask = 0x0000FF00;
            _PIXELFORMAT.dwBBitMask = 0x000000FF;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_A8B8G8R8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.RGBA;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 32;
            _PIXELFORMAT.dwRBitMask = 0x000000FF;
            _PIXELFORMAT.dwGBitMask = 0x0000FF00;
            _PIXELFORMAT.dwBBitMask = 0x00FF0000;
            _PIXELFORMAT.dwABitMask = 0xFF000000;
        }

        public static void DDSPF_X8B8G8R8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.RGB;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 32;
            _PIXELFORMAT.dwRBitMask = 0x000000FF;
            _PIXELFORMAT.dwGBitMask = 0x0000FF00;
            _PIXELFORMAT.dwBBitMask = 0x00FF0000;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_G16R16(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.RGB;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 32;
            _PIXELFORMAT.dwRBitMask = 0x0000FFFF;
            _PIXELFORMAT.dwGBitMask = 0xFFFF0000;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_R5G6B5(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.RGB;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 16;
            _PIXELFORMAT.dwRBitMask = 0x0000F800;
            _PIXELFORMAT.dwGBitMask = 0x000007E0;
            _PIXELFORMAT.dwBBitMask = 0x0000001F;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_A1R5G5B5(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.RGBA;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 16;
            _PIXELFORMAT.dwRBitMask = 0x00007C00;
            _PIXELFORMAT.dwGBitMask = 0x000003E0;
            _PIXELFORMAT.dwBBitMask = 0x0000001F;
            _PIXELFORMAT.dwABitMask = 0x00008000;
        }

        public static void DDSPF_R8G8B8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.RGB;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 24;
            _PIXELFORMAT.dwRBitMask = 0x00FF0000;
            _PIXELFORMAT.dwGBitMask = 0x0000FF00;
            _PIXELFORMAT.dwBBitMask = 0x000000FF;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_L8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.LUMINANCE;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 8;
            _PIXELFORMAT.dwRBitMask = 0x00FF;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_L16(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.LUMINANCE;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 16;
            _PIXELFORMAT.dwRBitMask = 0xFFFF;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_A8L8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.LUMINANCEA;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 16;
            _PIXELFORMAT.dwRBitMask = 0x00FF;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0xFF00;
        }

        public static void DDSPF_A8L8_ALT(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.LUMINANCEA;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 8;
            _PIXELFORMAT.dwRBitMask = 0x00FF;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0xFF00;
        }

        public static void DDSPF_A8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.ALPHA;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 8;
            _PIXELFORMAT.dwRBitMask = 0;
            _PIXELFORMAT.dwGBitMask = 0;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0x00FF;
        }

        public static void DDSPF_V8U8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.BUMPDUDV;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 16;
            _PIXELFORMAT.dwRBitMask = 0x00FF;
            _PIXELFORMAT.dwGBitMask = 0xFF00;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_V16U16(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.BUMPDUDV;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 32;
            _PIXELFORMAT.dwRBitMask = 0x0000FFFF;
            _PIXELFORMAT.dwGBitMask = 0xFFFF0000;
            _PIXELFORMAT.dwBBitMask = 0;
            _PIXELFORMAT.dwABitMask = 0;
        }

        public static void DDSPF_Q8W8V8U8(DDS_PIXELFORMAT _PIXELFORMAT)
        {
            _PIXELFORMAT.dwSize = 0x20;
            _PIXELFORMAT.dwFlags = (uint)DDS_TYPE.BUMPDUDV;
            _PIXELFORMAT.dwFourCC = 0;
            _PIXELFORMAT.dwRGBBitCount = 32;
            _PIXELFORMAT.dwRBitMask = 0x000000FF;
            _PIXELFORMAT.dwGBitMask = 0x0000FF00;
            _PIXELFORMAT.dwBBitMask = 0x00FF0000;
            _PIXELFORMAT.dwABitMask = 0xFF000000;
        }
    }
}
