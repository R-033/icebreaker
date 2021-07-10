using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreExtensions.Conversions
{
	/// <summary>
	/// Provides all major extensions for <see cref="List{T}"/>.
	/// </summary>
	public static class ListX
	{
		/// <summary>
		/// Resizes <see cref="List{T}"/> to a count specified.
		/// </summary>
		/// <typeparam name="TypeID"><see cref="Type"/> of elements in this list.</typeparam>
		/// <param name="list">This <see cref="List{T}"/> to resize.</param>
		/// <param name="count">Number of items to resize to.</param>
		public static void Resize<TypeID>(this List<TypeID> list, int count) where TypeID : new()
		{
			if (count <= 0)
			{
				list.Clear();
			}
			else if (count < list.Count)
			{
				list.RemoveRange(count, list.Count - count);
			}
			else
			{
				for (int a1 = list.Count; a1 < count; ++a1) list.Add(new TypeID());
			}
		}

		/// <summary>
		/// Resizes <see cref="List{T}"/> to a count specified and invokes action passed to create
		/// new instances and populate the list with it.
		/// </summary>
		/// <typeparam name="TypeID"><see cref="Type"/> of elements in this list.</typeparam>
		/// <param name="list">This <see cref="List{T}"/> to resize.</param>
		/// <param name="count">Number of items to resize to.</param>
		/// <param name="defaultConstructor">Action to create new objects and populate the list
		/// with in case it was resized beyond its previous count.</param>
		public static void Resize<TypeID>(this List<TypeID> list, int count, Func<TypeID> defaultConstructor)
		{
			if (count <= 0)
			{
				list.Clear();
			}
			else if (count < list.Count)
			{
				list.RemoveRange(count, list.Count - count);
			}
			else
			{
				if (defaultConstructor is null) throw new Exception("Default constructor passed was null");
				for (int a1 = list.Count; a1 < count; ++a1) list.Add(defaultConstructor());
			}
		}

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified
		/// predicate, and removes the first occurrence within the entire <see cref="List{T}"/>.
		/// </summary>
		/// <typeparam name="TypeID"><see cref="Type"/> of elements in this list.</typeparam>
		/// <param name="list">This list to remove element in.</param>
		/// <param name="predicate">The <see cref="Predicate{T}"/> delegate that defines the
		/// conditions of the element to remove.</param>
		/// <returns>True if removing was successful; false otherwise.</returns>
		public static bool RemoveWith<TypeID>(this List<TypeID> list, Predicate<TypeID> predicate)
		{
			int index = list.FindIndex(predicate);
			if (index != -1) { list.RemoveAt(index); return true; }
			else return false;
		}

		/// <summary>
		/// Checks whether all elements in <see cref="List{T}"/> are unique.
		/// </summary>
		/// <typeparam name="TypeID"><see cref="Type"/> of elements in this list.</typeparam>
		/// <param name="list">This list to check elements in.</param>
		/// <returns>True if all elements are unique; false otherwise.</returns>
		public static bool AllUnique<TypeID>(this List<TypeID> list)
		{
			var diffChecker = new HashSet<TypeID>();
			return list.All(diffChecker.Add);
		}
	}
}
