using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.Structures
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Color32F
	{
		public float R;
		public float G;
		public float B;
		public float A;

		public const int SizeOf = 0x10;

		public Color32F(float r, float g, float b) : this(r, g, b, 1.0f)
		{
		}

		public Color32F(float r, float g, float b, float a)
		{
			this.R = r;
			this.G = g;
			this.B = b;
			this.A = a;
		}

		public static Color ToColor(Color32F color)
		{
			return new Color(color.R, color.G, color.B, color.A);
		}

		public static Color32F ToColor(Color color)
		{
			return new Color32F(color.r, color.g, color.b, color.a);
		}
	}
}
