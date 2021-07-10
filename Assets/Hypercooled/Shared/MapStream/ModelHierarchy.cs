using Newtonsoft.Json;
using Newtonsoft.Json.Unity;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.MapStream
{
	public enum ModelHierarchyFlags : byte
	{
		Internal = 0x1,
		EndianSwapped = 0x2,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ModelHierarchyNode
	{
		public uint ModelName;
		public uint SolidKey;
		public int Padding;
		public ModelHierarchyFlags Flags;
		public byte ParentIndex;
		public byte NumChildren;
		public byte ChildIndex;

		public const int SizeOf = 0x10;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ModelHierarchyHeader // 0x0003410C
	{
		public uint Key;
		public byte NumNodes;
		public byte Flags;
		public ushort Padding;

		public const int SizeOf = 0x08;
	}

	public class ModelHierarchy
	{
		public class Node
		{
			[JsonConverter(typeof(BinStringConverter))]
			public uint ModelName { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint SolidName { get; set; }

			public List<Node> Children { get; }

			public Node()
			{
				this.Children = new List<Node>();
			}
		}

		[JsonConverter(typeof(BinStringConverter))]
		public uint Key { get; set; }

		public List<Node> Nodes { get; }

		public ModelHierarchy()
		{
			this.Nodes = new List<Node>();
		}
	}
}
