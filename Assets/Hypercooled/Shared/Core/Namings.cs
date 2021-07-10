using Hypercooled.Utils;
using System;

namespace Hypercooled.Shared.Core
{
	public static class Namings
	{
		public static readonly string MainProj = "Main";

		public static readonly string Assets = "Assets";
		public static readonly string UserInfo = "UserInfo";
		public static readonly string Topology = "Topology";

		public static readonly string VisibleManager = "VisibleManager";
		public static readonly string GeometryPack = "GeometryPack";
		public static readonly string TexturesPack = "TexturesPack";
		public static readonly string TexAnimsPack = "TexAnimsPack";

		public static readonly string GlobalFolder = "GLOBAL";
		public static readonly string LocationFolder = "LOCATION";
		public static readonly string StreamFolder = "STREAM";

		public static readonly string GlobalFile = "GLOBALB.LZC";
		public static readonly string IngameCommon = "INGAMECOMMON.LZC";
		public static readonly string IngameFile = "INGAMEB.LZC";
		public static readonly string Loc4Dyntex = "LOC4DYNTEX.BIN";

		public static readonly string BinExt = ".bin";
		public static readonly string HyperExt = ".hyper";

		public static readonly uint Hypercooled = "Hypercooled".BinHash();
		public static readonly string Watermark = $"Hypercooled | {DateTime.Now:MM/dd/yyyy}";
		public static readonly string Padding = "Padding Block";
	}
}
