using System;

namespace CoreExtensions.Management
{
	/// <summary>
	/// Provides all major extensions for <see cref="Exception"/>.
	/// </summary>
	public static class ExceptionX
	{
		/// <summary>
		/// Gets the innermost <see cref="Exception"/> message.
		/// </summary>
		/// <param name="e"><see cref="Exception"/> to analyze.</param>
		/// <returns>Innermost exception message as a string.</returns>
		public static string GetLowestMessage(this Exception e)
		{
			while (e.InnerException != null) e = e.InnerException;
			return e.Message;
		}

		/// <summary>
		/// Gets the innermost <see cref="Exception"/> HResult.
		/// </summary>
		/// <param name="e"><see cref="Exception"/> to analyze.</param>
		/// <returns>Innermost exception HResult as a 4-byte integer.</returns>
		public static int GetLowestHResult(this Exception e)
		{
			while (e.InnerException != null) e = e.InnerException;
			return e.HResult;
		}

		/// <summary>
		/// Gets the innermost <see cref="Exception"/> StackTrace.
		/// </summary>
		/// <param name="e"><see cref="Exception"/> to analyze.</param>
		/// <returns>Innermost exception StackTrace as a string.</returns>
		public static string GetLowestStackTrace(this Exception e)
		{
			while (e.InnerException != null) e = e.InnerException;
			return e.StackTrace;
		}
	}
}
