using System.IO;

namespace Hypercooled.Shared.World
{
	public abstract class VVResolvable
	{
		public static readonly VVResolvable[] Empty = new VVResolvable[0];

		public abstract UppleUTag Tag { get; }
		public abstract int SizeOfThis { get; }
		public abstract bool DynamicSize { get; }

		public abstract void Read(BinaryReader br);
		public abstract void Write(BinaryWriter bw);

		public override string ToString() => $"Tag: {this.Tag}";
	}
}
