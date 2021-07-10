using System;
using System.Runtime.InteropServices;

namespace CoreExtensions.Conversions
{
	/// <summary>
	/// Provides all major extensions to cast objects.
	/// </summary>
	public static class CastX
	{
		/// <summary>
		/// Fast way to cast memory of one object to another. Does not guarantee to return exact copy.
		/// </summary>
		/// <param name="result">Object to be casted memory into.</param>
		/// <param name="source">Object to be casted memory from.</param>
		public static void MemoryCast(object result, object source)
		{
			var SourceType = source.GetType(); // get type of the source object
			var ResultType = result.GetType(); // get type of the result object

			foreach (var FieldInfo in SourceType.GetFields()) // for each register in source object
			{
				var ResultField = ResultType.GetField(FieldInfo.Name); // get field of result
				if (ResultField == null) continue; // skip if a null field / nonexistent field
				ResultField.SetValue(result, FieldInfo.GetValue(source)); // cast memory
			}

			foreach (var PropertyInfo in SourceType.GetProperties()) // get property of source object
			{
				var ResultProperty = ResultType.GetProperty(PropertyInfo.Name); // get property of result
				if (ResultProperty == null) continue; // skip if a null property / nonexistent property
				ResultProperty.SetValue(result, PropertyInfo.GetValue(source, null), null); // cast memory
			}
		}

		/// <summary>
		/// Casts any object to any type specified. Throws exception in case cast fails.
		/// </summary>
		/// <typeparam name="TypeID">Type to be converted to.</typeparam>
		/// <param name="value">Object passed to be casted.</param>
		/// <returns>Casted value of type specified. If casting fails, exception will be thrown.</returns>
		public static TypeID StaticCast<TypeID>(IConvertible value) where TypeID : IConvertible
			=> (TypeID)Convert.ChangeType(value, typeof(TypeID));

		/// <summary>
		/// Reinterprets pointer to the unmanaged object passed and returns instance of new unmanaged
		/// type specified.
		/// </summary>
		/// <typeparam name="T">Unmanaged type to reinterpret.</typeparam>
		/// <typeparam name="S">Unmanaged type to cast to.</typeparam>
		/// <param name="value">Value to reinterpret.</param>
		/// <returns>New unmanaged instance casted from object passed.</returns>
		public static unsafe S ReinterpretCast<T, S>(T value)
			where T : unmanaged
			where S : unmanaged
		{
			return *(S*)&value;
		}

		/// <summary>
		/// Reinterprests pointer to the managed object passed and returns instance of new managed
		/// type specified.
		/// </summary>
		/// <typeparam name="T">Managed type to reinterpret.</typeparam>
		/// <typeparam name="S">Managed type to cast to.</typeparam>
		/// <param name="value">Value to reinterpret.</param>
		/// <returns>New managed instance casted from object passed.</returns>
		public static unsafe S CastManaged<T, S>(T value)
		{
			var array = new Native.NativeArray<byte>(Marshal.SizeOf<T>());
			var pointer = (IntPtr)array.GetPointer();
			Marshal.StructureToPtr(value, pointer, false);
			S result = Marshal.PtrToStructure<S>(pointer);
			array.Free();
			return result;
		}

		/// <summary>
		/// Returns object of type <typeparamref name="T"/> from the byte array provided.
		/// </summary>
		/// <param name="array">Array of bytes to convert.</param>
		/// <param name="startIndex">Start index in the array at which get the instance.</param>
		/// <returns>Object of type <typeparamref name="T"/>.</returns>
		public static unsafe T ToUnmanaged<T>(byte[] array, int startIndex) where T : unmanaged
		{
			fixed (byte* ptr = array)
			{
				var size = sizeof(T);
				if (startIndex < 0) startIndex = 0;
				else if (startIndex + size > array.Length) startIndex = array.Length - size;
				return *(T*)(ptr + startIndex);
			}
		}
	}
}
