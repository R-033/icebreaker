using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public abstract class MaterialObject
	{
		public abstract string CollectionName { get; }
		public abstract uint Key { get; }

		public abstract Color DiffuseMinColor { get; }
		public abstract Color DiffuseMaxColor { get; }
		public abstract float DiffuseMinAlpha { get; }
		public abstract float DiffuseMaxAlpha { get; }

		public abstract float SpecularPower { get; }
		public abstract Color SpecularMinColor { get; }
		public abstract Color SpecularMaxColor { get; }

		public abstract float EnvmapPower { get; }
		public abstract Color EnvmapMinColor { get; }
		public abstract Color EnvmapMaxColor { get; }

		public static MaterialObject GetMaterialObject(Managed.Game game, BinaryReader br)
		{
			switch (game)
			{
				case Managed.Game.Underground1: return null;
				case Managed.Game.Underground2: return Underground2.Core.MaterialObject.Read(br);
				case Managed.Game.MostWanted: return MostWanted.Core.MaterialObject.Read(br);
				case Managed.Game.Carbon: return Carbon.Core.MaterialObject.Read(br);
				default: return null;
			}
		}
	}
}
