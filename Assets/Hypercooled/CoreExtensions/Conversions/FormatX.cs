using System;

namespace CoreExtensions.Conversions
{
	/// <summary>
	/// Provides extension for reverse formatting of the strings.
	/// </summary>
	public static class FormatX
	{
		/// <summary>
		/// Returns substring of a value from a specified format.
		/// Example: if value = "Array[100]" and format = "Array[{X}]", this returns (string)"100".
		/// </summary>
		/// <param name="value">String from which to get format</param>
		/// <param name="format">Format to which parse the string and return the value.</param>
		/// <returns>String got from format of the value passed.</returns>
		private static string GetFormattedString(string value, string format)
		{
			try
			{
				int formatstart = format.IndexOf('{');
				int formatend = format.Length - format.IndexOf('}') - 1;
				string result = value.Substring(formatstart, formatend);
				return result;
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Returns value of <see cref="IConvertible"/> type from a specified format.
		/// Example: if value = "Array[100]", format = "Array[{X}]", and type if 4-byte integer,
		/// this returns (int)100.
		/// </summary>
		/// <param name="value">String from which to get format</param>
		/// <param name="format">Format to which parse the string and return the value.</param>
		/// <param name="result">Result value of type <see cref="IConvertible"/> passed.</param>
		/// <returns>Value got from format of the string passed.</returns>
		public static bool GetFormattedValue<TypeID>(this string value, string format, out TypeID result)
			where TypeID : IConvertible
		{
			result = default;
			try
			{
				string str = GetFormattedString(value, format);
				result = CastX.StaticCast<TypeID>(str);
				return true;
			}
			catch (Exception) { return false; }
		}
	}
}
