using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.Structures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Color8UI
	{
		public byte R;
		public byte G;
		public byte B;
		public byte A;

		public const int SizeOf = 0x04;

		public Color8UI(byte r, byte g, byte b) : this(r, g, b, Byte.MaxValue)
		{
		}

		public Color8UI(byte r, byte g, byte b, byte a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public static Color ToColor(Color8UI color)
		{
			float mult = 0.00392157f;
			return new Color(color.R * mult, color.G * mult, color.B * mult, color.A * mult);
		}

		public static Color32F ToColor32F(Color8UI color)
		{
			float mult = 0.00392157f;
			return new Color32F(color.R * mult, color.G * mult, color.B * mult, color.A * mult);
		}

		public static Color8UI ToColor8UI(Color color)
		{
			float mult = 255.0f;
			return new Color8UI((byte)(color.r * mult), (byte)(color.g * mult), (byte)(color.b * mult), (byte)(color.a * mult));
		}

		public static Color8UI ToColor8UI(Color32F color)
		{
			float mult = 255.0f;
			return new Color8UI((byte)(color.R * mult), (byte)(color.G * mult), (byte)(color.B * mult), (byte)(color.A * mult));
		}
	}
}
