namespace Hypercooled.Utils
{
	public static class TrackHelper
	{
		public static bool IsScenerySectionDrivable(ushort sectionNumber, int lod)
		{
			var c = sectionNumber / 100 + 64;
			var i = sectionNumber % 100;

			return c >= 'A' && c <= 'T' && i >= 1 && i < lod;
		}
		public static bool IsLODScenerySectionNumber(ushort sectionNumber, int lod)
		{
			var c = sectionNumber / 100 + 64;
			var i = sectionNumber % 100;

			return c >= 'A' && c <= 'T' && i >= lod && i < (lod << 1);
		}
		public static bool IsRawAssetSectionNumber(ushort sectionNumber)
		{
			return (sectionNumber / 100 + 64) >= 'V';
		}
		public static ushort GetDrivableFromtLODNumber(ushort sectionNumber, int lod)
		{
			return (ushort)(sectionNumber - lod);
		}
		public static ushort GetLODFromtDrivableNumber(ushort sectionNumber, int lod)
		{
			return (ushort)(sectionNumber + lod);
		}
	}
}
