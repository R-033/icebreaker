using System.IO;

namespace Hypercooled.Shared.Core
{
	public abstract class CollisionObject
	{
		public abstract string CollectionName { get; set; }
		public abstract uint Key { get; }

		public abstract void Serialize(BinaryWriter bw);
		public abstract void Deserialize(BinaryReader br);
	}
}
