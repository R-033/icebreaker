using System;
using System.Runtime.InteropServices;

namespace Hypercooled.PreProcessing
{
	public class Assertions
	{
		public static void AssertTypeSize(Type type, int size)
		{
			var actual = Marshal.SizeOf(type);

			if (actual != size)
			{
				throw new Exception($"Size of type {type} is {actual} bytes, which does not equal {size} bytes");
			}
		}

		public static void AssertInheritance(Type derived, Type based)
		{
			var temp = derived;

			while (temp.BaseType != typeof(object))
			{
				if (temp.BaseType.Equals(based)) return;
				else temp = temp.BaseType;
			}

			throw new Exception($"Type {derived} does not derive from type {based}");
		}

		public static void AssertInterfaces(Type type, Type interFace)
		{
			if (!interFace.IsAssignableFrom(type))
			{
				throw new Exception($"Type {type} does not implement interface {interFace}");
			}
		}

		public static void AssertAllSupports()
		{
			SPreProcess.AssertTypeSize();
			SPreProcess.AssertInterfaces();
			SPreProcess.AssertInheritance();

			CPreProcess.AssertTypeSize();
			CPreProcess.AssertInterfaces();
			CPreProcess.AssertInheritance();

			MWPreProcess.AssertTypeSize();
			MWPreProcess.AssertInterfaces();
			MWPreProcess.AssertInheritance();

			U2PreProcess.AssertTypeSize();
			U2PreProcess.AssertInterfaces();
			U2PreProcess.AssertInheritances();
		}
	}
}
