using Hypercooled.Managed.Mono;
using Hypercooled.Shared.Core;
using Hypercooled.Utils;
using System;
using System.IO;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hypercooled.Shared.Runtime
{
	public abstract class TrackSection
	{
		private static readonly string ms_sceneryRootName = "SceneryObjects";
		private static readonly string ms_lightRootName = "LightObjects";
		private static readonly string ms_flareRootName = "FlareObjects";
		private static readonly string ms_triggerRootName = "TriggerObjects";
		private static readonly string ms_emitterRootName = "EmitterObjects";

		protected GameObject m_root;

		protected GameObject m_sceneryRoot;
		protected GameObject m_lightRoot;
		protected GameObject m_flareRoot;
		protected GameObject m_triggerRoot;
		protected GameObject m_emitterRoot;
		protected GameObject m_topologyRoot;

		public static int MaxSceneryObjectsPerFrame { get; set; } = 100;
		public static int MaxLightObjectsPerFrame { get; set; } = 100;
		public static int MaxFlareObjectsPerFrame { get; set; } = 100;
		public static int MaxTriggerObjectsPerFrame { get; set; } = 100;
		public static int MaxEmitterObjectsPerFrame { get; set; } = 100;

		public ushort SectionNumber { get; }
		public TrackStreamer Streamer { get; }
		public TrackState State { get; private set; }
		public TrackVisibility Visibility { get; set; }

		public TrackSection DrivableSibling { get; set; }
		public TrackSection LODSibling { get; set; }
		public VisibleSectionUserInfo UserInfo { get; protected set; }
		public TopologyManager Topology { get; protected set; }

		public ConcurrentDictionary<uint, AnimObject> AnimationMap { get; }
		public ConcurrentDictionary<uint, MeshObject> MeshMap { get; }
		public ConcurrentDictionary<uint, TextureObject> TextureMap { get; }

		public List<SceneryObjectMonoWrapper> RenderedScenery { get; }
		public List<LightObjectMonoWrapper> RenderedLights { get; }
		public List<FlareObjectMonoWrapper> RenderedFlares { get; }
		public List<TriggerObjectMonoWrapper> RenderedTriggers { get; }
		public List<EmitterObjectMonoWrapper> RenderedEmitters { get; }

		public TrackSection(ushort sectionNumber, TrackStreamer streamer)
		{
			this.SectionNumber = sectionNumber;
			this.Streamer = streamer;

			this.m_root = new GameObject(this.SectionNumber.ToString());
			this.m_root.transform.parent = streamer.gameObject.transform;
			this.m_root.SetActive(false);

			this.AnimationMap = new ConcurrentDictionary<uint, AnimObject>();
			this.MeshMap = new ConcurrentDictionary<uint, MeshObject>();
			this.TextureMap = new ConcurrentDictionary<uint, TextureObject>();

			this.RenderedScenery = new List<SceneryObjectMonoWrapper>();
			this.RenderedLights = new List<LightObjectMonoWrapper>();
			this.RenderedFlares = new List<FlareObjectMonoWrapper>();
			this.RenderedTriggers = new List<TriggerObjectMonoWrapper>();
			this.RenderedEmitters = new List<EmitterObjectMonoWrapper>();

			this.UserInfo = new VisibleSectionUserInfo(streamer.Project.GameType, sectionNumber);
			this.Streamer.TrackSections.TryAdd(this.SectionNumber, this);
		}

		protected abstract BlockReader GetAssetBlockReader();
		protected abstract void ProcessLoadedParsers(BlockReader br);
		protected abstract void InstantiateTopology();
		protected abstract void UnloadOverrideResources();

		private void UnloadInternalResources(bool pushToPools)
		{
			if (pushToPools)
			{
				for (int i = 0; i < this.RenderedScenery.Count; ++i)
				{
					this.Streamer.PushSceneryObjectToPool(this.RenderedScenery[i]);
				}

				for (int i = 0; i < this.RenderedLights.Count; ++i)
				{
					this.Streamer.PushLightObjectToPool(this.RenderedLights[i]);
				}

				for (int i = 0; i < this.RenderedFlares.Count; ++i)
				{
					this.Streamer.PushFlareObjectToPool(this.RenderedFlares[i]);
				}

				for (int i = 0; i < this.RenderedTriggers.Count; ++i)
				{
					this.Streamer.PushTriggerObjectToPool(this.RenderedTriggers[i]);
				}

				for (int i = 0; i < this.RenderedEmitters.Count; ++i)
				{
					this.Streamer.PushEmitterObjectToPool(this.RenderedEmitters[i]);
				}
			}

			var set = new HashSet<uint>();

			foreach (var key in this.TextureMap.Keys)
			{
				this.Streamer.TextureReferencer.Release(key);
			}

			foreach (var pair in this.MeshMap)
			{
				this.Streamer.MeshReferencer.Release(pair.Key);

				foreach (var subMeshInfo in pair.Value.SubMeshInfos)
				{
					if (set.Add(subMeshInfo.MaterialKey))
					{
						this.Streamer.MaterialReferencer.Release(subMeshInfo.MaterialKey);
					}
				}
			}

			Object.Destroy(this.m_root);
		}
		private void CreateParentRoots()
		{
			this.m_sceneryRoot = new GameObject(ms_sceneryRootName);
			this.m_sceneryRoot.transform.parent = this.m_root.transform;
			this.m_sceneryRoot.SetActive(false);

			this.m_lightRoot = new GameObject(ms_lightRootName);
			this.m_lightRoot.transform.parent = this.m_root.transform;
			this.m_lightRoot.SetActive(false);

			this.m_flareRoot = new GameObject(ms_flareRootName);
			this.m_flareRoot.transform.parent = this.m_root.transform;
			this.m_flareRoot.SetActive(false);

			this.m_triggerRoot = new GameObject(ms_triggerRootName);
			this.m_triggerRoot.transform.parent = this.m_root.transform;
			this.m_triggerRoot.SetActive(false);

			this.m_emitterRoot = new GameObject(ms_emitterRootName);
			this.m_emitterRoot.transform.parent = this.m_root.transform;
			this.m_emitterRoot.SetActive(false);
		}
		private void RenderSceneryObject(int index)
		{
			var sceneryObject = this.UserInfo.SceneryObjects[index];
			var renderedScenery = this.Streamer.PullSceneryObjectFromPool();

			renderedScenery.gameObject.transform.parent = this.m_sceneryRoot.transform;
			renderedScenery.gameObject.name = this.GetNameForScenery(index, sceneryObject);
			renderedScenery.gameObject.SetActive(true);

			renderedScenery.Initialize(sceneryObject);
			this.RenderedScenery.Add(renderedScenery);

			sceneryObject.ModelLodA = this.GetModelObjectByKey(sceneryObject.MeshKeyLodA);
			sceneryObject.ModelLodB = this.GetModelObjectByKey(sceneryObject.MeshKeyLodB);
			sceneryObject.ModelLodC = this.GetModelObjectByKey(sceneryObject.MeshKeyLodC);
			sceneryObject.ModelLodD = this.GetModelObjectByKey(sceneryObject.MeshKeyLodD);
			renderedScenery.SetDisplayedLOD(TrackLOD.A);
		}
		private void RenderLightObject(int index)
		{
			var lightObject = this.UserInfo.LightObjects[index];
			var renderedLight = this.Streamer.PullLightObjectFromPool();

			renderedLight.gameObject.transform.parent = this.m_lightRoot.transform;
			renderedLight.gameObject.name = lightObject.CollectionName;
			renderedLight.gameObject.SetActive(true);

			renderedLight.Initialize(lightObject);
			this.RenderedLights.Add(renderedLight);
		}
		private void RenderFlareObject(int index)
		{
			var flareObject = this.UserInfo.FlareObjects[index];
			var renderedFlare = this.Streamer.PullFlareObjectFromPool();

			renderedFlare.gameObject.transform.parent = this.m_flareRoot.transform;
			renderedFlare.gameObject.name = flareObject.CollectionName;
			renderedFlare.gameObject.SetActive(true);

			renderedFlare.Initialize(flareObject);
			this.RenderedFlares.Add(renderedFlare);
		}
		private void RenderTriggerObject(int index)
		{
			var triggerObject = this.UserInfo.TriggerObjects[index];
			var renderedTrigger = this.Streamer.PullTriggerObjectFromPool();

			renderedTrigger.gameObject.transform.parent = this.m_triggerRoot.transform;
			renderedTrigger.gameObject.name = triggerObject.CollectionName;
			renderedTrigger.gameObject.SetActive(true);

			renderedTrigger.Initialize(triggerObject);
			this.RenderedTriggers.Add(renderedTrigger);
		}
		private void RenderEmitterObject(int index)
		{
			var emitterObject = this.UserInfo.EmitterObjects[index];
			var renderedEmitter = this.Streamer.PullEmitterObjectFromPool();

			renderedEmitter.gameObject.transform.parent = this.m_emitterRoot.transform;
			renderedEmitter.gameObject.name = emitterObject.CollectionName;
			renderedEmitter.gameObject.SetActive(true);

			renderedEmitter.Initialize(emitterObject);
			this.RenderedEmitters.Add(renderedEmitter);
		}
		private ModelObject GetModelObjectByKey(uint key)
		{
			if (key == 0) return null;

			var mesh = this.Streamer.GetMeshObjectOrNull(key);

			if (mesh is null)
			{
				// todo : default cube
				return null;
			}

			var materials = new Material[mesh.SubMeshInfos.Length];

			for (int i = 0; i < materials.Length; ++i)
			{
				materials[i] = this.Streamer.MaterialReferencer.GetNoRef(mesh.SubMeshInfos[i].MaterialKey);
			}

			return new ModelObject(mesh, materials);
		}

		protected void LoaderVisibleSectionUserInfo(string path)
		{
			this.UserInfo = VisibleSectionUserInfo.Deserialize(path);
		}

		public void GenerateManagedTextures()
		{
			foreach (var pair in this.TextureMap)
			{
				if (!this.Streamer.TextureReferencer.TryLock(pair.Key))
				{
					this.Streamer.TextureReferencer.Add(pair.Key, pair.Value);
				}
			}

			// todo : cleanup mayb?
		}
		public void GenerateManagedMeshes()
		{
			foreach (var pair in this.MeshMap)
			{
				if (!this.Streamer.MeshReferencer.TryLock(pair.Key))
				{
					this.Streamer.MeshReferencer.Add(pair.Key, pair.Value);
				}
			}

			// todo: cleanup mayb?
		}
		public void GenerateManagedMaterials()
		{
			var set = new HashSet<uint>();

			foreach (var mesh in this.MeshMap.Values)
			{
				foreach (var subMeshInfo in mesh.SubMeshInfos)
				{
					var key = subMeshInfo.MaterialKey;

					if (set.Add(key) && !this.Streamer.MaterialReferencer.TryLock(key))
					{
						this.Streamer.MaterialReferencer.Add(key, this.Streamer.PleaseGiveMeMaterial(subMeshInfo));
					}
				}
			}
		}
		public string GetNameForScenery(int index)
		{
			return $"{index:000}_{this.UserInfo.SceneryObjects[index].GetPartialName()}";
		}
		public string GetNameForScenery(int index, SceneryObject sceneryObject)
		{
			return $"{index:000}_{sceneryObject.GetPartialName()}";
		}

		public async Task LoadAssets()
		{
			this.State = TrackState.Loading;

			var folder = Path.Combine(this.Streamer.Project.Folder, Namings.StreamFolder, this.SectionNumber.ToString());
			var assets = Path.Combine(folder, Namings.Assets + Namings.HyperExt);
			if (!File.Exists(assets)) return;

			using (var sr = new StreamReader(assets))
			{
				while (!sr.EndOfStream)
				{
					var line = sr.ReadLine();
					if (String.IsNullOrWhiteSpace(line)) continue;

					var reader = this.GetAssetBlockReader();
					var path = Path.Combine(folder, line);

					using (var ms = new MemoryStream(File.ReadAllBytes(path)))
					using (var br = new BinaryReader(ms))
					{
						await reader.LoadChunksAsync(BlockReader.ReaderOptions.GetDefault(br));
					}

					this.ProcessLoadedParsers(reader);
				}
			}

			this.State = TrackState.Loaded;
		}
		public async Task SaveAssets()
		{
			await Task.Delay(0); // todo lol
		}
		public IEnumerator Activate()
		{
			this.State = TrackState.Activating;
			this.CreateParentRoots();
			this.ChangeVisibilities();

			for (int i = 0, k = 0; i < this.UserInfo.SceneryObjects.Count; ++i, ++k)
			{
				this.RenderSceneryObject(i);

				if (k == MaxSceneryObjectsPerFrame)
				{
					k = 0;
					yield return null;
				}
			}

			for (int i = 0, k = 0; i < this.UserInfo.LightObjects.Count; ++i, ++k)
			{
				this.RenderLightObject(i);

				if (k == MaxLightObjectsPerFrame)
				{
					k = 0;
					yield return null;
				}
			}

			for (int i = 0, k = 0; i < this.UserInfo.FlareObjects.Count; ++i, ++k)
			{
				this.RenderFlareObject(i);

				if (k == MaxFlareObjectsPerFrame)
				{
					k = 0;
					yield return null;
				}
			}

			for (int i = 0, k = 0; i < this.UserInfo.TriggerObjects.Count; ++i, ++k)
			{
				this.RenderTriggerObject(i);

				if (k == MaxTriggerObjectsPerFrame)
				{
					k = 0;
					yield return null;
				}
			}

			for (int i = 0, k = 0; i < this.UserInfo.EmitterObjects.Count; ++i, ++k)
			{
				this.RenderEmitterObject(i);

				if (k == MaxEmitterObjectsPerFrame)
				{
					k = 0;
					yield return null;
				}
			}

			this.InstantiateTopology();

			this.m_root.SetActive(true);
			this.State = TrackState.Activated;
			yield return null;
		}
		public void Unload()
		{
			if (this.State == TrackState.Unloaded) return;

			this.UnloadOverrideResources();
			this.UnloadInternalResources(true);

			this.State = TrackState.Unloaded;
			this.Streamer.TrackSections.TryRemove(this.SectionNumber, out _); // remove itself
		}
		public void UnsafeUnload()
		{
			if (this.State == TrackState.Unloaded) return;

			this.UnloadOverrideResources();
			this.UnloadInternalResources(false);

			this.State = TrackState.Unloaded;
			this.Streamer.TrackSections.TryRemove(this.SectionNumber, out _); // remove itself
		}
		public void ChangeVisibilities()
		{
			if (this.m_sceneryRoot != null) this.m_sceneryRoot.SetActive((this.Visibility & TrackVisibility.SceneryObjects) != 0);
			if (this.m_lightRoot != null) this.m_lightRoot.SetActive((this.Visibility & TrackVisibility.LightObjects) != 0);
			if (this.m_flareRoot != null) this.m_flareRoot.SetActive((this.Visibility & TrackVisibility.FlareObjects) != 0);
			if (this.m_triggerRoot != null) this.m_triggerRoot.SetActive((this.Visibility & TrackVisibility.TriggerObjects) != 0);
			if (this.m_emitterRoot != null) this.m_emitterRoot.SetActive((this.Visibility & TrackVisibility.EmitterObjects) != 0);
			if (this.m_topologyRoot != null) this.m_topologyRoot.SetActive((this.Visibility & TrackVisibility.TopologyObjects) != 0);
		}

		public void AddSceneryObject()
		{

		}
		public void RemoveSceneryObject(int index)
		{

		}
		public void CopySceneryObject(int index)
		{

		}
		// etc
	}
}
