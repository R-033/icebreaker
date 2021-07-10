using CoreExtensions.Text;
using System;

namespace Hypercooled.Utils
{
	public static class GenericHelper
	{
		public static string GetEnumValueOrUIntHex<T>(uint value) where T : Enum
		{
			if (Enum.IsDefined(typeof(T), value))
			{
				return ((T)(object)value).ToString();
			}
			else
			{
				return value.FastToHexString(false);
			}
		}

		public static int CeilToTheNearestPow2(int value, int pow2)
		{
			var diff = pow2 - (value & (pow2 - 1));
			return value + (diff == pow2 ? 0 : diff);
		}

		public static uint CeilToTheNearestPow2(uint value, uint pow2)
		{
			var diff = pow2 - (value & (pow2 - 1));
			return value + (diff == pow2 ? 0 : diff);
		}

		public static long CeilToTheNearestPow2(long value, long pow2)
		{
			var diff = pow2 - (value & (pow2 - 1));
			return value + (diff == pow2 ? 0 : diff);
		}

		public static ulong CeilToTheNearestPow2(ulong value, ulong pow2)
		{
			var diff = pow2 - (value & (pow2 - 1));
			return value + (diff == pow2 ? 0 : diff);
		}
	}
}
