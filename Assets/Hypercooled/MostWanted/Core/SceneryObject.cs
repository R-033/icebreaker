using CoreExtensions.IO;
using Hypercooled.Managed;
using Hypercooled.Shared.MapStream;
using Hypercooled.Utils;
using System.IO;
using UnityEngine;

namespace Hypercooled.MostWanted.Core
{
	public class SceneryObject : Shared.Core.SceneryObject
	{
		public SceneryInstanceFlags InstanceFlags { get; set; }

		public uint HierarchyKey { get; set; }

		public string HierarchyName
		{
			get => this.HierarchyKey.BinString();
			set => this.HierarchyKey = value.BinHash();
		}

		public uint SpawnerReplacementKey { get; set; }
		public uint SpawnerCollisionKey { get; set; }
		public uint SpawnerAttributes { get; set; }
		public uint SpawnerExcludeFlags { get; set; }
		public uint SpawnerSpawnFlags { get; set; }

		public string SpawnerReplacementMesh
		{
			get => this.SpawnerReplacementKey.BinString();
			set => this.SpawnerReplacementKey = value.BinHash();
		}
		public string SpawnerCollisionName
		{
			get => this.SpawnerCollisionKey.VltString();
			set => this.SpawnerCollisionKey = value.VltHash();
		}

		public SceneryObject() : base()
		{
		}

		public SceneryObject(ushort sectionNumber, MapStream.SceneryInstance instance, MapStream.SceneryInfo info) : base()
		{
			this.SectionNumber = sectionNumber;
			this.InstanceFlags = instance.InstanceFlags;
			this.MeshKeyLodA = info.SolidMeshKey1;
			this.MeshKeyLodB = info.SolidMeshKey2;
			this.MeshKeyLodC = info.SolidMeshKey3;
			this.MeshKeyLodD = info.SolidMeshKey4;
			this.HierarchyKey = info.HierarchyKey;
			this.Radius = info.Radius;

			// position, scale and rotation go here
			this.Position = new Vector3(instance.Position.x, instance.Position.z, instance.Position.y);

			var vRight = new Vector3(instance.Rotation.Value11, instance.Rotation.Value12, instance.Rotation.Value13);
			var vForward = new Vector3(instance.Rotation.Value21, instance.Rotation.Value22, instance.Rotation.Value23);
			var vUpwards = new Vector3(instance.Rotation.Value31, instance.Rotation.Value32, instance.Rotation.Value33);
			vRight *= 0.0001220703125f;   // vRight /= 0x2000
			vUpwards *= 0.0001220703125f; // vUpwards /= 0x2000
			vForward *= 0.0001220703125f; // vForward /= 0x2000

			this.Scale = new Vector3(vRight.magnitude, vUpwards.magnitude, vForward.magnitude);

			vRight = vRight.normalized;
			vUpwards = vUpwards.normalized;
			vForward = vForward.normalized;

			if (Vector3.Dot(Vector3.Cross(vForward, vUpwards), vRight) < 0f)
			{
				vRight = -vRight;
				this.Scale = new Vector3(-this.Scale.x, this.Scale.y, this.Scale.z);
			}

			this.Rotation = MathExtensions.LookRotation(vRight.x, vRight.y, vRight.z, vForward.x, vForward.y, vForward.z, vUpwards.x, vUpwards.y, vUpwards.z);

			this.Transform = Matrix4x4.TRS(this.Position, this.Rotation, this.Scale);

			this.BoundingBox = new Bounds()
			{
				min = new Vector3(instance.BBoxMin.x, instance.BBoxMin.z, instance.BBoxMin.y),
				max = new Vector3(instance.BBoxMax.x, instance.BBoxMax.z, instance.BBoxMax.y),
			};
		}

		public override void Serialize(BinaryWriter bw)
		{
			/* 0x00 */ bw.WriteUnmanaged(this.Transform);
			/* 0x40 */ bw.WriteUnmanaged(this.BoundingBox);

			/* 0x58 */ bw.WriteUnmanaged(this.Position);
			/* 0x64 */ bw.WriteUnmanaged(this.Scale);
			/* 0x70 */ bw.WriteUnmanaged(this.Rotation);

			/* 0x80 */ bw.Write(0L);

			/* 0x88 */ bw.Write(this.SectionLodA);
			/* 0x8A */ bw.Write(this.SectionLodB);
			/* 0x8C */ bw.Write(this.SectionLodC);
			/* 0x8E */ bw.Write(this.SectionLodD);

			/* 0x90 */ bw.Write(this.MeshKeyLodA);
			/* 0x94 */ bw.Write(this.MeshKeyLodB);
			/* 0x98 */ bw.Write(this.MeshKeyLodC);
			/* 0x9C */ bw.Write(this.MeshKeyLodD);

			/* 0xA0 */ bw.Write(this.HasOverride);
			/* 0xA1 */ bw.Write(this.IsSmackable);
			/* 0xA2 */ bw.Write(this.ExcludeFlags);

			/* 0xA4 */ bw.Write(this.Radius);
			/* 0xA8 */ bw.WriteUnmanaged(this.SpawnerPosition);
			/* 0xB4 */ bw.WriteUnmanaged(this.SpawnerScale);
			/* 0xC0 */ bw.WriteUnmanaged(this.SpawnerRotation);

			/* 0xD0 */ bw.Write((uint)this.InstanceFlags);
			/* 0xD4 */ bw.Write(this.HierarchyKey);

			/* 0xD8 */ bw.Write(this.SpawnerReplacementKey);
			/* 0xDC */ bw.Write(this.SpawnerCollisionKey);
			/* 0xE0 */ bw.Write(this.SpawnerAttributes);
			/* 0xE4 */ bw.Write(this.SpawnerExcludeFlags);
			/* 0xE8 */ bw.Write(this.SpawnerSpawnFlags);
			/* 0xEC */ bw.Write(this.SceneryGroups.Count);

			foreach (var group in this.SceneryGroups)
			{
				bw.Write(group);
			}

			bw.FillBufferPow2(0x10);
		}

		public override void Deserialize(BinaryReader br)
		{
			/* 0x00 */ this.Transform = br.ReadUnmanaged<Matrix4x4>();
			/* 0x40 */ this.BoundingBox = br.ReadUnmanaged<Bounds>();

			/* 0x58 */ this.Position = br.ReadUnmanaged<Vector3>();
			/* 0x64 */ this.Scale = br.ReadUnmanaged<Vector3>();
			/* 0x70 */ this.Rotation = br.ReadUnmanaged<Quaternion>().normalized;

			/* 0x80 */ br.BaseStream.Position += 8;

			/* 0x88 */ this.SectionLodA = br.ReadUInt16();
			/* 0x8A */ this.SectionLodB = br.ReadUInt16();
			/* 0x8C */ this.SectionLodC = br.ReadUInt16();
			/* 0x8E */ this.SectionLodD = br.ReadUInt16();

			/* 0x90 */ this.MeshKeyLodA = br.ReadUInt32();
			/* 0x94 */ this.MeshKeyLodB = br.ReadUInt32();
			/* 0x98 */ this.MeshKeyLodC = br.ReadUInt32();
			/* 0x9C */ this.MeshKeyLodD = br.ReadUInt32();

			/* 0xA0 */ this.HasOverride = br.ReadBoolean();
			/* 0xA1 */ this.IsSmackable = br.ReadBoolean();
			/* 0xA2 */ this.ExcludeFlags = br.ReadUInt16();

			/* 0xA4 */ this.Radius = br.ReadSingle();
			/* 0xA8 */ this.SpawnerPosition = br.ReadUnmanaged<Vector3>();
			/* 0xB4 */ this.SpawnerScale = br.ReadUnmanaged<Vector3>();
			/* 0xC0 */ this.SpawnerRotation = br.ReadUnmanaged<Quaternion>();

			/* 0xD0 */ this.InstanceFlags = (SceneryInstanceFlags)br.ReadUInt32();
			/* 0xD4 */ this.HierarchyKey = br.ReadUInt32();

			/* 0xD8 */ this.SpawnerReplacementKey = br.ReadUInt32();
			/* 0xDC */ this.SpawnerCollisionKey = br.ReadUInt32();
			/* 0xE0 */ this.SpawnerAttributes = br.ReadUInt32();
			/* 0xE4 */ this.SpawnerExcludeFlags = br.ReadUInt32();
			/* 0xE8 */ this.SpawnerSpawnFlags = br.ReadUInt32();
			/* 0xEC */ uint sceneryGroupCount = br.ReadUInt32();

			for (uint i = 0; i < sceneryGroupCount; ++i)
			{
				this.SceneryGroups.Add(br.ReadUInt32());
			}

			br.AlignReaderPow2(0x10);
		}

		public override void FixupData()
		{

		}
	}
}
