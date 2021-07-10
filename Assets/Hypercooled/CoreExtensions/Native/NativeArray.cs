using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CoreExtensions.Native
{
	/// <summary>
	/// An array with elements of unmanaged type provided that allocates memory on a native heap, i.e.
	/// with no GC.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerDisplay("Length = {Length}")]
	public struct NativeArray<T> where T : unmanaged
	{
		private unsafe T* _pointer;
		private bool _disposed;

		/// <summary>
		/// Gets length of the allocated array.
		/// </summary>
		public int Length { get; }

		/// <summary>
		/// Initializes new instance of <see cref="NativeArray{T}"/> with size and element type specified.
		/// After array is about to be destroyed, <see cref="Free"/> should be called to release natively
		/// allocated buffer.
		/// </summary>
		/// <param name="size">Size of array to allocate.</param>
		/// <exception cref="ArgumentOutOfRangeException">Size of allocation passed is less or
		/// equals zero.</exception>
		public NativeArray(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(size), "Size of allocated array should be bigger than 0");
			}

			unsafe
			{
				var len = sizeof(T);
				_pointer = (T*)Marshal.AllocHGlobal(len * size).ToPointer();
				Length = size;
				_disposed = false;
			}
		}

		/// <summary>
		/// Gets or sets element at index specified.
		/// </summary>
		/// <param name="index">Index of an element to get or set.</param>
		/// <returns>Element at index specified.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Index specified is out of range of array.</exception>
		public T this[int index]
		{
			get => Get(index);
			set => Set(index, value);
		}

		/// <summary>
		/// Gets element at index specified.
		/// </summary>
		/// <param name="index">Index of an element to get.</param>
		/// <returns>Element at index specified.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Index specified is out of range of array.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe T Get(int index)
		{
			if (index < 0 || index >= Length) throw new ArgumentOutOfRangeException(nameof(index));
			return *(_pointer + index);
		}

		/// <summary>
		/// Sets element at index specified.
		/// </summary>
		/// <param name="index">Index of an element to set.</param>
		/// <param name="value">Value to set at index specified.</param>
		/// <exception cref="ArgumentOutOfRangeException">Index specified is out of range of array.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void Set(int index, T value)
		{
			if (index < 0 || index >= Length) throw new ArgumentOutOfRangeException(nameof(index));
			*(_pointer + index) = value;
		}

		/// <summary>
		/// Gets element at index specified without bound checks. Faster than regular <see cref="Get(int)"/>,
		/// but unsafe and can overflow length of the array. Does not throw.
		/// </summary>
		/// <param name="index">Index of an element to set.</param>
		/// <returns>Element at index specified.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe T GetUnsafe(int index) => _pointer[index];

		/// <summary>
		/// Sets element at index specified without bound checks. Faster than regular <see cref="Set(int, T)"/>,
		/// but unsafe and can overflow length of the array. Does not throw.
		/// </summary>
		/// <param name="index">Index of an element to set.</param>
		/// <param name="value">Value to set at index specified.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void SetUnsafe(int index, T value) => _pointer[index] = value;

		/// <summary>
		/// Gets pointer of type <typeparamref name="T"/> to the beginning of allocated array.
		/// </summary>
		/// <returns></returns>
		public unsafe T* GetPointer() => _pointer;

		/// <summary>
		/// Frees/releases this <see cref="NativeArray{T}"/> instance. This should be called when array
		/// is not needed anymore or before it goes out of local range.
		/// </summary>
		public unsafe void Free()
		{
			if (!_disposed)
			{
				Marshal.FreeHGlobal(new IntPtr(_pointer));
				_disposed = true;
			}
		}
	}
}
