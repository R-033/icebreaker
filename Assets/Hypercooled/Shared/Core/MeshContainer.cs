using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Hypercooled.Shared.Core
{
	public abstract class MeshContainer
	{
		public Solids.MeshContainerHeader Header { get; set; }
		public ConcurrentDictionary<uint, MeshObject> MeshMap { get; }

		public MeshContainer()
		{
			this.MeshMap = new ConcurrentDictionary<uint, MeshObject>();
		}

		public MeshContainer(ConcurrentDictionary<uint, MeshObject> meshMap)
		{
			this.MeshMap = meshMap;
		}

		public abstract uint[] GetAllKeys(BinaryReader br);
		public abstract void Load(BinaryReader br);
		public abstract void Save(BinaryWriter bw);

		protected void InternalSwapPolygonIndices(int[] polygons)
		{
			for (int i = 0; i < polygons.Length - 1; i += 3)
			{
				int temp = polygons[i + 1];
				polygons[i + 1] = polygons[i + 2];
				polygons[i + 2] = temp;
			}
		}

		public void MergeMeshesIntoMap(IDictionary<uint, MeshObject> map)
		{
			if (map is null) return;

			foreach (var pair in this.MeshMap)
			{
				if (!map.ContainsKey(pair.Key))
				{
					map.Add(pair);
				}
			}
		}

		public void SetContainerName(string name)
		{
			var header = this.Header;
			header.Descriptor = name ?? String.Empty;
			this.Header = header;
		}

		public static MeshContainer GetMeshContainer(Managed.Game game)
		{
			switch (game)
			{
				case Managed.Game.Underground1: return null;
				case Managed.Game.Underground2: return new Underground2.Core.MeshContainer();
				case Managed.Game.MostWanted: return new MostWanted.Core.MeshContainer();
				case Managed.Game.Carbon: return new Carbon.Core.MeshContainer();
				default: return null;
			}
		}
	}
}
