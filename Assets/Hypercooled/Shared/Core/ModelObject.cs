using System;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public class ModelObject
	{
		public MeshObject Mesh { get; }
		public Material[] Materials { get; }

		public ModelObject(MeshObject mesh, Material[] materials)
		{
			this.Mesh = mesh;
			this.Materials = materials;
		}

		public override bool Equals(object obj) => obj is ModelObject model && this == model;
		public override int GetHashCode() => this.Mesh is null ? 0 : (int)this.Mesh.Key;
		public override string ToString() => this.Mesh is null ? String.Empty : this.Mesh.CollectionName;

		public static bool operator==(ModelObject model1, ModelObject model2)
		{
			if (model1 is null) return model2 is null;
			if (model2 is null) return false;

			bool mesh1isnull = model1.Mesh is null;
			bool mesh2isnull = model2.Mesh is null;

			bool mats1isnull = model1.Materials is null;
			bool mats2isnull = model2.Materials is null;

			if (mesh1isnull != mesh2isnull) return false;
			if (mats1isnull != mats2isnull) return false;

			if (mats1isnull) return true;
			return model1.Mesh.Key == model2.Mesh.Key;
		}
		public static bool operator!=(ModelObject model1, ModelObject model2)
		{
			return !(model1 == model2);
		}
	}
}
