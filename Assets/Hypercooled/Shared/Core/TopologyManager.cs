using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public abstract class TopologyManager
	{
		public abstract ushort SectionNumber { get; }
		public List<TopologyObject> Objects { get; }

		public TopologyManager()
		{
			this.Objects = new List<TopologyObject>();
		}

		public abstract void Load(BinaryReader br);
		public abstract void Save(BinaryWriter bw);
		public abstract void Generate(GameObject gameObject);

		public abstract void FromGroupNames(List<string> groups);
		public abstract void FromGroupIndices(Dictionary<string, int> groups);

		protected bool DoWeWriteTopologyNames()
		{
			for (int i = 0; i < this.Objects.Count; ++i)
			{
				if (!String.IsNullOrEmpty(this.Objects[i].CollectionName))
				{
					return true;
				}
			}

			return false;
		}
	}
}
