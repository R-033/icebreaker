using Hypercooled.Underground2.Solids;
using Hypercooled.Utils;
using System;
using UnityEngine;
using SS = Hypercooled.Shared.Solids;

namespace Hypercooled.Underground2.Core
{
	public class MeshObject : Shared.Core.MeshObject // 0x80134010
	{
		private string m_collectionName;
		private uint m_key;
		private MeshShaderInfo[] m_submeshes;

		public override string CollectionName
		{
			get
			{
				return this.m_collectionName;
			}
			set
			{
				this.m_collectionName = value ?? String.Empty;
				this.m_key = value.BinHash();
			}
		}
		public override uint Key => this.m_key;

		public override int SubMeshCount => this.m_submeshes.Length;
		public override Vector3 BBoxMin => this.SolidInformation.AABBMin;
		public override Vector3 BBoxMax => this.SolidInformation.AABBMax;
		public override SS.MeshEntryFlags Flags => this.MeshInformation.Flags;

		public SolidInfo SolidInformation { get; set; }
		public MeshInfoHeader MeshInformation { get; set; }
		public MeshShaderInfo[] SubMeshes
		{
			get => this.m_submeshes;
			set => this.m_submeshes = value ?? new MeshShaderInfo[0];
		}

		public override bool RecalculateNormals => (this.Flags & SS.MeshEntryFlags.HasFullVertices) == 0;
		public override bool RecalculateTangents => true;

		public MeshObject() : base()
		{
			this.m_submeshes = new MeshShaderInfo[0];
		}

		protected override unsafe Mesh InternalGenerateMesh()
		{
			var mesh = new Mesh();

			var buffer = this.GetVertexBuffer(0);

			const int reducedSize = MeshReducedVertex.SizeOf >> 2;
			const int fullSize = MeshVertex.SizeOf >> 2;

			int count = buffer.Length / (this.RecalculateNormals
				? reducedSize
				: fullSize);

			var vertices = new Vector3[count];
			var uvs = new Vector3[count];
			var colors = new Color32[count];

			if (this.RecalculateNormals)
			{
				fixed (float* ptr = buffer)
				{
					for (int i = 0, k = 0; i < count; ++i)
					{
						var vx = ptr[k++];
						var vz = ptr[k++];
						var vy = ptr[k++];
						vertices[i] = new Vector3(vx, vy, vz);

						colors[i] = *(Color32*)(ptr + k++);

						var vtx = ptr[k++];
						var vty = ptr[k++];
						uvs[i] = new Vector2(vtx, vty);
					}
				}

				mesh.SetVertices(vertices);
				mesh.SetColors(colors);
				mesh.SetUVs(0, uvs);
			}
			else
			{
				var normals = new Vector3[count];

				fixed (float* ptr = buffer)
				{
					for (int i = 0, k = 0; i < count; ++i)
					{
						var vx = ptr[k++];
						var vz = ptr[k++];
						var vy = ptr[k++];
						vertices[i] = new Vector3(vx, vy, vz);

						var vnx = ptr[k++];
						var vnz = ptr[k++];
						var vny = ptr[k++];
						normals[i] = new Vector3(vnx, vny, vnz);

						colors[i] = *(Color32*)(ptr + k++);

						var vtx = ptr[k++];
						var vty = ptr[k++];
						uvs[i] = new Vector2(vtx, vty);
					}
				}

				mesh.SetVertices(vertices);
				mesh.SetNormals(normals);
				mesh.SetColors(colors);
				mesh.SetUVs(0, uvs);
			}

			mesh.subMeshCount = this.m_submeshes.Length;

			for (int i = 0; i < this.m_submeshes.Length; ++i)
			{
				var submesh = this.m_submeshes[i];
				mesh.SetTriangles(this.Triangles, submesh.PolygonIndex, submesh.NumPolygons, i);
			}

			if (this.RecalculateNormals) mesh.RecalculateNormals();
			if (this.RecalculateTangents) mesh.RecalculateTangents();

			return mesh;
		}
	}
}
