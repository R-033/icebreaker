using Hypercooled.Shared.Core;
using Hypercooled.Shared.Runtime;
using Hypercooled.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Hypercooled.Managed.Mono
{
	public class SceneryObjectMonoWrapper : ObjectMonoWrapper<SceneryObject>
	{
		private MeshFilter m_meshFilter;
		private MeshRenderer m_meshRenderer;
		private SceneryObject m_wrappedScenery;

		public override SceneryObject Wrapped => this.m_wrappedScenery;

		public TrackLOD LOD;
		public bool HasOverride;
		public bool IsSmackable;

		public Bounds BBox;
		public string Hierarchy;

		public string UG2Flags;
		public Shared.MapStream.SceneryInstanceFlags Flags;

		public List<string> SceneryGroups = new List<string>();

		private void Update()
		{
			this.SetDisplayedLOD(this.LOD);
		}

		public override void Initialize(SceneryObject sceneryObject)
		{
			if (this.m_meshFilter is null) this.m_meshFilter = this.gameObject.GetComponent<MeshFilter>();
			if (this.m_meshRenderer is null) this.m_meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

			this.m_wrappedScenery = sceneryObject;

			this.transform.position = sceneryObject.Position;
			this.transform.localScale = sceneryObject.Scale;
			this.transform.rotation = sceneryObject.Rotation;

			this.HasOverride = sceneryObject.HasOverride;
			this.IsSmackable = sceneryObject.IsSmackable;

			this.BBox = sceneryObject.BoundingBox;

			if (sceneryObject is Underground2.Core.SceneryObject underground2)
			{
				this.UG2Flags = $"0x{underground2.InstanceFlags:X4}";
			}
			else if (sceneryObject is Carbon.Core.SceneryObject carbon)
			{
				this.Hierarchy = carbon.HierarchyName;
				this.Flags = carbon.InstanceFlags;
			}
			else if (sceneryObject is MostWanted.Core.SceneryObject mostwanted)
			{
				this.Hierarchy = mostwanted.HierarchyName;
				this.Flags = mostwanted.InstanceFlags;
			}

			foreach (var key in sceneryObject.SceneryGroups)
			{
				this.SceneryGroups.Add(key.BinString());
			}

			// todo: initialize meshes
		}
		public override void ResetData()
		{
			this.m_wrappedScenery = null;

			this.transform.position = Vector3.zero;
			this.transform.localScale = Vector3.one;
			this.transform.rotation = Quaternion.identity;

			this.SceneryGroups.Clear();
		}
		public override void UpdateData()
		{
			this.m_wrappedScenery.Position = this.transform.position;
			this.m_wrappedScenery.Scale = this.transform.localScale;
			this.m_wrappedScenery.Rotation = this.transform.rotation;

			this.m_wrappedScenery.HasOverride = this.HasOverride;
			this.m_wrappedScenery.IsSmackable = this.IsSmackable;

			this.m_wrappedScenery.FixupData();
		}

		public void SetDisplayedLOD(TrackLOD lod)
		{
			switch (lod)
			{
				case TrackLOD.A:
					SetMeshAndMaterials(this.m_wrappedScenery.ModelLodA);
					return;

				case TrackLOD.B:
					SetMeshAndMaterials(this.m_wrappedScenery.ModelLodB);
					return;

				case TrackLOD.C:
					SetMeshAndMaterials(this.m_wrappedScenery.ModelLodC);
					return;

				case TrackLOD.D:
					SetMeshAndMaterials(this.m_wrappedScenery.ModelLodD);
					return;

				default:
					return; // bruh
			}

			void SetMeshAndMaterials(ModelObject modelObject)
			{
				if (modelObject is null)
				{
					// default cube and mats here
				}
				else
				{
					this.m_meshFilter.sharedMesh = modelObject.Mesh.GetUnityMesh();
					this.m_meshRenderer.sharedMaterials = modelObject.Materials;
				}
			}
		}
	}
}
