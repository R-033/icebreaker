using System.IO;

namespace Hypercooled.Shared.Core
{
	public abstract class TrackRouteManager
	{
		// todo : more shared stuff
		public abstract void Load(BinaryReader br);
		public abstract void Save(BinaryWriter bw);
	}
}
