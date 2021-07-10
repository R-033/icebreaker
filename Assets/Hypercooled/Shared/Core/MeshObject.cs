using Hypercooled.Shared.Solids;
using System;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Hypercooled.Shared.Core
{
	public abstract class MeshObject
	{
		public class SubMeshInfo
		{
			public uint DiffuseKey { get; }
			public uint NormalKey { get; }
			public uint HeightKey { get; }
			public uint SpecularKey { get; }
			public uint OpacityKey { get; }
			public uint LightKey { get; }
			public uint DXShaderType { get; }
			public uint MaterialKey { get; }

			public SubMeshInfo(uint diffuse, uint normal, uint height, uint specular, uint opacity, uint light, uint dxShader)
			{
				this.DiffuseKey = diffuse;
				this.NormalKey = normal;
				this.HeightKey = height;
				this.SpecularKey = specular;
				this.OpacityKey = opacity;
				this.LightKey = light;
				this.DXShaderType = dxShader;

				this.MaterialKey = (uint)Tuple.Create(diffuse, normal, height, specular, opacity, light, dxShader).GetHashCode();
			}
		}

		private Mesh m_unityMesh;
		private float[][] m_buffer;
		private int[] m_triangles;
		private SolidTexture[] m_textures;
		private SolidMaterial[] m_materials;
		private SolidMarker[] m_markers;
		private SubMeshInfo[] m_subMeshInfos;

		private static readonly float[][] ms_defaultbuf;

		public abstract string CollectionName { get; set; }
		public abstract uint Key { get; }

		public float[][] Buffer
		{
			get => this.m_buffer;
			set => this.m_buffer = value ?? ms_defaultbuf;
		}
		public SolidTexture[] Textures
		{
			get => this.m_textures;
			set => this.m_textures = value ?? new SolidTexture[0];
		}
		public SolidMaterial[] Materials
		{
			get => this.m_materials;
			set => this.m_materials = value ?? new SolidMaterial[0];
		}
		public SolidMarker[] Markers
		{
			get => this.m_markers;
			set => this.m_markers = value ?? new SolidMarker[0];
		}
		public int[] Triangles
		{
			get => this.m_triangles;
			set => this.m_triangles = value ?? new int[0];
		}
		public SubMeshInfo[] SubMeshInfos
		{
			get => this.m_subMeshInfos;
			set => this.m_subMeshInfos = value ?? new SubMeshInfo[0];
		}

		public abstract int SubMeshCount { get; }
		public abstract Vector3 BBoxMin { get; }
		public abstract Vector3 BBoxMax { get; }
		public abstract MeshEntryFlags Flags { get; }

		public abstract bool RecalculateNormals { get; }
		public abstract bool RecalculateTangents { get; }
		protected abstract Mesh InternalGenerateMesh();

		static MeshObject()
		{
			ms_defaultbuf = new float[0][];
		}
		public MeshObject()
		{
			this.m_buffer = ms_defaultbuf;
			this.m_triangles = new int[0];
			this.m_textures = new SolidTexture[0];
			this.m_materials = new SolidMaterial[0];
			this.m_markers = new SolidMarker[0];
		}

		public Mesh GetUnityMesh()
		{
			if (this.m_unityMesh == null)
			{
				this.m_unityMesh = this.InternalGenerateMesh();
			}

			return this.m_unityMesh;
		}
		public void DestroyUnityMesh()
		{
			Object.Destroy(this.m_unityMesh);
		}
		public float[] GetVertexBuffer(int index)
		{
			return this.m_buffer[index];
		}
		public void SetVertexBuffer(int index, float[] buffer)
		{
			this.m_buffer[index] = buffer;
		}
	}
}
