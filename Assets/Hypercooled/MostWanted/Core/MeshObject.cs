using Hypercooled.MostWanted.Solids;
using Hypercooled.Utils;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using SS = Hypercooled.Shared.Solids;

namespace Hypercooled.MostWanted.Core
{
	public class MeshObject : Shared.Core.MeshObject // 0x80134010
	{
		private class StructInfo
		{
			public bool HasUV1 { get; set; }
			public bool HasUV2 { get; set; }
			public bool HasUV3 { get; set; }
			public bool HasUV4 { get; set; }
			public bool HasTangents { get; set; }

			public Vector3[] Vertices { get; set; }
			public Vector3[] Normals { get; set; }
			public Color32[] Colors { get; set; }
			public Vector2[] UV1s { get; set; }
			public Vector2[] UV2s { get; set; }
			public Vector2[] UV3s { get; set; }
			public Vector2[] UV4s { get; set; }
			public Vector4[] Tangents { get; set; }
		}

		private string m_collectionName;
		private uint m_key;
		private MeshShaderInfo[] m_submeshes;
		private string[] m_vltMaterials;

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
		public string[] VltMaterials
		{
			get => this.m_vltMaterials;
			set => this.m_vltMaterials = value ?? new string[0];
		}

		public override bool RecalculateNormals => false;
		public override bool RecalculateTangents => (this.Flags & SS.MeshEntryFlags.HasFullVertices) == 0;

		public MeshObject() : base()
		{
			this.m_submeshes = new MeshShaderInfo[0];
			this.m_vltMaterials = new string[0];
		}

		protected override Mesh InternalGenerateMesh()
		{
			int count = 0;

			foreach (var submesh in this.m_submeshes)
			{
				count += submesh.NumVertices;
			}

			var info = this.GetGeneralStructInfo();

			info.Vertices = new Vector3[count];
			info.Normals = new Vector3[count];
			info.Colors = new Color32[count];

			if (info.HasUV1) info.UV1s = new Vector2[count];
			if (info.HasUV2) info.UV2s = new Vector2[count];
			if (info.HasUV3) info.UV3s = new Vector2[count];
			if (info.HasUV4) info.UV4s = new Vector2[count];
			if (info.HasTangents) info.Tangents = new Vector4[count];

			this.ReadVertexBufferInfo(info);

			var mesh = new Mesh();

			mesh.SetVertices(info.Vertices);
			mesh.SetNormals(info.Normals);
			mesh.SetColors(info.Colors);

			if (info.HasUV1) mesh.SetUVs(0, info.UV1s);
			if (info.HasUV2) mesh.SetUVs(1, info.UV2s);
			if (info.HasUV3) mesh.SetUVs(2, info.UV3s);
			if (info.HasUV4) mesh.SetUVs(3, info.UV4s);

			if (info.HasTangents) mesh.SetTangents(info.Tangents);
			else mesh.RecalculateTangents();

			this.ReadIndexBufferInfo(mesh, count);
			return mesh;
		}

		private StructInfo GetGeneralStructInfo()
		{
			var result = new StructInfo();

			foreach (var submesh in this.m_submeshes)
			{
				switch (submesh.DXShader)
				{
					case DXShaderType.WorldBoneShader:
					case DXShaderType.WorldReflectShader:
					case DXShaderType.WorldNormalMap:
						result.HasUV3 = true;
						result.HasUV4 = true;
						goto case DXShaderType.skyshader;

					case DXShaderType.skyshader:
						result.HasUV2 = true;
						goto case DXShaderType.WorldShader;

					case DXShaderType.CarShader:
						result.HasTangents = true;
						goto case DXShaderType.WorldShader;

					case DXShaderType.WorldShader:
					case DXShaderType.GlossyWindow:
					case DXShaderType.billboardshader:
					default:
						result.HasUV1 = true;
						break;
				}
			}

			return result;
		}

		private void ReadIndexBufferInfo(Mesh mesh, int total)
		{
			var currentShader = (DXShaderType)(-1);
			int currentOffset = 0;

			mesh.SetIndexBufferParams(this.Triangles.Length, IndexFormat.UInt32);
			mesh.SetIndexBufferData(this.Triangles, 0, 0, this.Triangles.Length, (MeshUpdateFlags)0x0D);

			mesh.subMeshCount = this.m_submeshes.Length;

			for (int i = 0, k = 0; i < this.m_submeshes.Length; ++i)
			{
				var submesh = this.m_submeshes[i];
				var shader = submesh.DXShader;

				// this is what happens when you give child a keyboard
				if (shader != currentShader)
				{
					currentOffset = k;
					currentShader = shader;
				}

				var descriptor = new SubMeshDescriptor()
				{
					baseVertex = currentOffset,
					firstVertex = 0,
					vertexCount = total,
					indexStart = submesh.PolygonIndex,
					indexCount = submesh.PolygonSize,
					topology = MeshTopology.Triangles,
					bounds = new Bounds()
					{
						min = submesh.BoundsMin,
						max = submesh.BoundsMax,
					},
				};

				mesh.SetSubMesh(i, descriptor, (MeshUpdateFlags)0x0D);
				k += submesh.NumVertices;
			}
		}

		private unsafe void ReadVertexBufferInfo(StructInfo info)
		{
			int currentOffset = 0;
			int currentBuffer = -1;
			var currentShader = (DXShaderType)(-1);

			for (int i = 0, vertIndex = 0; i < this.m_submeshes.Length; ++i)
			{
				var submesh = this.m_submeshes[i];
				var shader = submesh.DXShader;

				if (shader != currentShader)
				{
					++currentBuffer;
					currentOffset = 0;
					currentShader = shader;
				}

				var buffer = this.GetVertexBuffer(currentBuffer);

				fixed (float* ptr = buffer)
				{
					switch (shader)
					{
						case DXShaderType.WorldBoneShader:
						case DXShaderType.WorldReflectShader:
						case DXShaderType.WorldNormalMap:
							currentOffset = this.ReadWorldReflectSubMesh(ptr, currentOffset, vertIndex, submesh.NumVertices, info);
							break;

						case DXShaderType.CarShader:
							currentOffset = this.ReadCarShaderSubMesh(ptr, currentOffset, vertIndex, submesh.NumVertices, info);
							break;

						case DXShaderType.skyshader:
							currentOffset = this.ReadSkyShaderSubMesh(ptr, currentOffset, vertIndex, submesh.NumVertices, info);
							break;

						case DXShaderType.WorldShader:
						case DXShaderType.GlossyWindow:
						case DXShaderType.billboardshader:
						default:
							currentOffset = this.ReadDefaultSubMesh(ptr, currentOffset, vertIndex, submesh.NumVertices, info);
							break;
					}
				}

				vertIndex += submesh.NumVertices;
			}
		}

		private unsafe int ReadDefaultSubMesh(float* ptr, int offset, int start, int count, StructInfo info)
		{
			for (int i = start; i < start + count; ++i)
			{
				var vx = ptr[offset++];
				var vz = ptr[offset++];
				var vy = ptr[offset++];
				info.Vertices[i] = new Vector3(vx, vy, vz);

				var vnx = ptr[offset++];
				var vnz = ptr[offset++];
				var vny = ptr[offset++];
				info.Normals[i] = new Vector3(vnx, vny, vnz);

				info.Colors[i] = *(Color32*)(ptr + offset++);

				var vt1x = ptr[offset++];
				var vt1y = ptr[offset++];
				info.UV1s[i] = new Vector2(vt1x, vt1y);
			}

			return offset;
		}

		private unsafe int ReadSkyShaderSubMesh(float* ptr, int offset, int start, int count, StructInfo info)
		{
			for (int i = start; i < start + count; ++i)
			{
				var vx = ptr[offset++];
				var vz = ptr[offset++];
				var vy = ptr[offset++];
				info.Vertices[i] = new Vector3(vx, vy, vz);

				var vnx = ptr[offset++];
				var vnz = ptr[offset++];
				var vny = ptr[offset++];
				info.Normals[i] = new Vector3(vnx, vny, vnz);

				info.Colors[i] = *(Color32*)(ptr + offset++);

				var vt1x = ptr[offset++];
				var vt1y = ptr[offset++];
				info.UV1s[i] = new Vector2(vt1x, vt1y);

				var vt2x = ptr[offset++];
				var vt2y = ptr[offset++];
				info.UV2s[i] = new Vector2(vt2x, vt2y);
			}

			return offset;
		}

		private unsafe int ReadCarShaderSubMesh(float* ptr, int offset, int start, int count, StructInfo info)
		{
			for (int i = start; i < start + count; ++i)
			{
				var vx = ptr[offset++];
				var vz = ptr[offset++];
				var vy = ptr[offset++];
				info.Vertices[i] = new Vector3(vx, vy, vz);

				var vnx = ptr[offset++];
				var vnz = ptr[offset++];
				var vny = ptr[offset++];
				info.Normals[i] = new Vector3(vnx, vny, vnz);

				info.Colors[i] = *(Color32*)(ptr + offset++);

				var vt1x = ptr[offset++];
				var vt1y = ptr[offset++];
				info.UV1s[i] = new Vector2(vt1x, vt1y);

				var vsx = ptr[offset++];
				var vsz = ptr[offset++];
				var vsy = ptr[offset++];
				var vsw = 1.0f; // ?????
				info.Tangents[i] = new Vector4(vsx, vsy, vsz, vsw);
			}

			return offset;
		}

		private unsafe int ReadWorldReflectSubMesh(float* ptr, int offset, int start, int count, StructInfo info)
		{
			for (int i = start; i < start + count; ++i)
			{
				var vx = ptr[offset++];
				var vz = ptr[offset++];
				var vy = ptr[offset++];
				info.Vertices[i] = new Vector3(vx, vy, vz);

				var vnx = ptr[offset++];
				var vnz = ptr[offset++];
				var vny = ptr[offset++];
				info.Normals[i] = new Vector3(vnx, vny, vnz);

				info.Colors[i] = *(Color32*)(ptr + offset++);

				var vt1x = ptr[offset++];
				var vt1y = ptr[offset++];
				info.UV1s[i] = new Vector2(vt1x, vt1y);

				var vt2x = ptr[offset++];
				var vt2y = ptr[offset++];
				info.UV2s[i] = new Vector2(vt2x, vt2y);

				var vt3x = ptr[offset++];
				var vt3y = ptr[offset++];
				info.UV3s[i] = new Vector2(vt3x, vt3y);

				var vt4x = ptr[offset++];
				var vt4y = ptr[offset++];
				info.UV4s[i] = new Vector2(vt4x, vt4y);
			}

			return offset;
		}
	}
}
