using Hypercooled.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public abstract class SceneryObject
	{
		public ushort SectionNumber { get; set; }

		public Vector3 Position { get; set; }
		public Vector3 Scale { get; set; }
		public Quaternion Rotation { get; set; }

		public Matrix4x4 Transform { get; set; }
		public Bounds BoundingBox { get; set; }

		public bool IsSmackable { get; set; }
		public bool HasOverride { get; set; }
		public ushort ExcludeFlags { get; set; }

		public uint MeshKeyLodA { get; set; }
		public uint MeshKeyLodB { get; set; }
		public uint MeshKeyLodC { get; set; }
		public uint MeshKeyLodD { get; set; }

		public ushort SectionLodA { get; set; }
		public ushort SectionLodB { get; set; }
		public ushort SectionLodC { get; set; }
		public ushort SectionLodD { get; set; }

		public ModelObject ModelLodA { get; set; }
		public ModelObject ModelLodB { get; set; }
		public ModelObject ModelLodC { get; set; }
		public ModelObject ModelLodD { get; set; }

		public float Radius { get; set; }

		public Vector3 SpawnerPosition { get; set; }
		public Vector3 SpawnerScale { get; set; }
		public Quaternion SpawnerRotation { get; set; }

		public HashSet<uint> SceneryGroups { get; set; }

		public string MeshNameLodA
		{
			get => this.MeshKeyLodA.BinString();
			set => this.MeshKeyLodA = value.BinHash();
		}
		public string MeshNameLodB
		{
			get => this.MeshKeyLodB.BinString();
			set => this.MeshKeyLodB = value.BinHash();
		}
		public string MeshNameLodC
		{
			get => this.MeshKeyLodC.BinString();
			set => this.MeshKeyLodC = value.BinHash();
		}
		public string MeshNameLodD
		{
			get => this.MeshKeyLodD.BinString();
			set => this.MeshKeyLodD = value.BinHash();
		}

		public SceneryObject()
		{
			this.SceneryGroups = new HashSet<uint>();
		}

		public abstract void Serialize(BinaryWriter bw);
		public abstract void Deserialize(BinaryReader br);
		public abstract void FixupData();

		public string GetPartialName()
		{
			if (this.MeshKeyLodA == 0)
			{
				if (this.MeshKeyLodB == 0)
				{
					if (this.MeshKeyLodC == 0)
					{
						return this.MeshNameLodD;
					}
					else
					{
						return this.MeshNameLodC;
					}
				}
				else
				{
					return this.MeshNameLodB;
				}
			}
			else
			{
				return this.MeshNameLodA;
			}
		}

		public static SceneryObject GetSceneryObject(Managed.Game game)
		{
			switch (game)
			{
				case Managed.Game.Underground1: return null;
				case Managed.Game.Underground2: return new Underground2.Core.SceneryObject();
				case Managed.Game.MostWanted: return new MostWanted.Core.SceneryObject();
				case Managed.Game.Carbon: return new Carbon.Core.SceneryObject();
				default: return null;
			}
		}
	}
}
