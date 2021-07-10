using System;
using System.IO;
using UnityEngine;

namespace Hypercooled.Utils
{
	public static class IOHelper
	{
		public static BinaryReader GetBinaryReader(string path, bool toMemoryStream)
		{
			var stream = toMemoryStream
				? (new MemoryStream(File.ReadAllBytes(path)) as Stream)
				: (new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x10000) as Stream);

			return new BinaryReader(stream);
		}

		public static BinaryWriter GetBinaryWriter(string path, bool toMemoryStream)
		{
			var stream = toMemoryStream
				? (new MemoryStream(0x10000) as Stream)
				: (new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, 0x10000) as Stream);

			return new BinaryWriter(stream);
		}

		public static StreamReader GetStreamReader(string path, bool toMemoryStream)
		{
			var stream = toMemoryStream
				? (new MemoryStream(File.ReadAllBytes(path)) as Stream)
				: (new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x10000) as Stream);

			return new StreamReader(stream);
		}

		public static StreamWriter GetStreamWriter(string path, bool toMemoryStream)
		{
			var stream = toMemoryStream
				? (new MemoryStream(0x10000) as Stream)
				: (new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write, 0x10000) as Stream);

			return new StreamWriter(stream);
		}

		public static void AssertFileExists(string file)
		{
			if (!File.Exists(file))
			{
				throw new FileNotFoundException($"File {file} does not exist");
			}
		}
	}
}
