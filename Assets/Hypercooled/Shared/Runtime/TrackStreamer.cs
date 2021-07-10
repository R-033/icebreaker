using CoreExtensions.IO;
using Hypercooled.Managed.Mono;
using Hypercooled.Shared.Core;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hypercooled.Shared.Runtime
{
	public abstract class TrackStreamer : MonoBehaviour
	{
		private static readonly Action<TextureObject> ms_destroyTexture = (texture) => texture.DestroyUnityTexture();
		private static readonly Action<MeshObject> ms_destroyMesh = (mesh) => mesh.DestroyUnityMesh();
		private static readonly Action<Material> ms_destroyMaterial = (material) => Object.Destroy(material);
		private static readonly ushort[] ms_emptySections = new ushort[0];

		private static readonly string ms_poolableSceneryObject = "PoolableSceneryObject";
		private static readonly string ms_poolableLightObject = "PoolableLightObject";
		private static readonly string ms_poolableFlareObject = "PoolableFlareObject";
		private static readonly string ms_poolableTriggerObject = "PoolableTriggerObject";
		private static readonly string ms_poolableEmitterObject = "PoolableEmitterObject";

		private GameObject m_poolableRoot;

		public static bool InstantiatePools { get; set; } = true;

		public static int SceneryObjectPoolCapacity { get; set; } = 1000;
		public static int LightObjectPoolCapacity { get; set; } = 100;
		public static int FlareObjectPoolCapacity { get; set; } = 100;
		public static int TriggerObjectPoolCapacity { get; set; } = 100;
		public static int EmitterObjectPoolCapacity { get; set; } = 100;

		public static GameObject SceneryObjectPrefab { get; set; }
		public static GameObject LightObjectPrefab { get; set; }
		public static GameObject FlareObjectPrefab { get; set; }
		public static GameObject TriggerObjectPrefab { get; set; }
		public static GameObject EmitterObjectPrefab { get; set; }

		public TrackMode Mode { get; private set; } = TrackMode.None;
		public TrackState State { get; private set; } = TrackState.Empty;
		public ProjectHeader Project { get; private set; }
		public int LODOffset => this.VisibleSectionManager?.LODOffset ?? 10;

		public Camera RenderCamera { get; set; }
		public ushort RenderingSection { get; private set; }
		public Coroutine RenderingRoutine { get; protected set; }
		public List<ushort> RequestedSections { get; } = new List<ushort>();
		public ushort[] CurrentSections { get; private set; } = ms_emptySections;

		// Pools
		public DynamicPool<SceneryObjectMonoWrapper> SceneryObjectPool { get; } = new DynamicPool<SceneryObjectMonoWrapper>(SceneryObjectPoolCapacity);
		public DynamicPool<LightObjectMonoWrapper> LightObjectPool { get; } = new DynamicPool<LightObjectMonoWrapper>(LightObjectPoolCapacity);
		public DynamicPool<FlareObjectMonoWrapper> FlareObjectPool { get; } = new DynamicPool<FlareObjectMonoWrapper>(FlareObjectPoolCapacity);
		public DynamicPool<TriggerObjectMonoWrapper> TriggerObjectPool { get; } = new DynamicPool<TriggerObjectMonoWrapper>(TriggerObjectPoolCapacity);
		public DynamicPool<EmitterObjectMonoWrapper> EmitterObjectPool { get; } = new DynamicPool<EmitterObjectMonoWrapper>(EmitterObjectPoolCapacity);

		// Referencers
		public Referencer<TextureObject> TextureReferencer { get; } = new Referencer<TextureObject>(ms_destroyTexture);
		public Referencer<MeshObject> MeshReferencer { get; } = new Referencer<MeshObject>(ms_destroyMesh);
		public Referencer<Material> MaterialReferencer { get; } = new Referencer<Material>(ms_destroyMaterial);

		// Global maps
		public ConcurrentDictionary<uint, MaterialObject> LightMaterials { get; } = new ConcurrentDictionary<uint, MaterialObject>();
		public ConcurrentDictionary<uint, MeshObject> GlobalMeshes { get; } = new ConcurrentDictionary<uint, MeshObject>();
		public ConcurrentDictionary<uint, TextureObject> GlobalTextures { get; } = new ConcurrentDictionary<uint, TextureObject>();
		public ConcurrentDictionary<uint, AnimObject> GlobalAnimations { get; } = new ConcurrentDictionary<uint, AnimObject>();
		public ConcurrentDictionary<uint, CollisionObject> GlobalCollisions { get; } = new ConcurrentDictionary<uint, CollisionObject>();

		// Location
		public VisibleSectionManager VisibleSectionManager { get; private set; }

		// Sections
		public ConcurrentDictionary<ushort, TrackSection> TrackSections { get; } = new ConcurrentDictionary<ushort, TrackSection>();

		// Abstract methods
		protected abstract void CloseOverride();
		protected abstract Action<string> GetAssetParser(string assetName);
		protected abstract TrackSection CreateNewSection(ushort sectionNumber);
		public abstract void EnableSceneryGroup(string group);
		public abstract void DisableSceneryGroup(string group);
		public abstract Material PleaseGiveMeMaterial(MeshObject.SubMeshInfo subMeshInfo);

		// Private methods
		private void InitPoolable()
		{
			this.m_poolableRoot = new GameObject("PoolableRoot");
			this.m_poolableRoot.transform.parent = this.gameObject.transform;
			this.m_poolableRoot.SetActive(false);
		}
		private void PreparePools()
		{
			InitSinglePool(ms_poolableSceneryObject, SceneryObjectPrefab, this.SceneryObjectPool);
			InitSinglePool(ms_poolableLightObject, LightObjectPrefab, this.LightObjectPool);
			InitSinglePool(ms_poolableFlareObject, FlareObjectPrefab, this.FlareObjectPool);
			InitSinglePool(ms_poolableTriggerObject, TriggerObjectPrefab, this.TriggerObjectPool);
			InitSinglePool(ms_poolableEmitterObject, EmitterObjectPrefab, this.EmitterObjectPool);

			void InitSinglePool<T>(string defaultName, GameObject prefab, DynamicPool<T> pool) where T : MonoBehaviour
			{
				for (int i = 0; i < pool.Count; ++i)
				{
					var obj = Object.Instantiate(prefab, this.m_poolableRoot.transform);

					obj.SetActive(false);
					obj.name = defaultName;

					pool.Push(obj.GetComponent<T>());
				}
			}
		}
		private void LoadGlobalAssets()
		{
			var folder = Path.Combine(this.Project.Folder, Namings.GlobalFolder);

			if (!Directory.Exists(folder))
			{
				Debug.LogError($"Global folder {folder} does not exist!");
				return;
			}

			foreach (var global in this.Project.Global)
			{
				var path = Path.Combine(folder, global);
				if (!File.Exists(path)) continue;

				using (var br = IOHelper.GetBinaryReader(path, false))
				{
					while (br.BaseStream.Position < br.BaseStream.Length)
					{
						var offset = br.BaseStream.Position;
						var info = br.ReadUnmanaged<BinBlock.Info>();

						var block = new BinBlock(info, offset);
						br.BaseStream.Position = offset;

						if (block.ID == (uint)BinBlockID.LightMaterials)
						{
							var material = MaterialObject.GetMaterialObject(this.Project.GameType, br);
							this.LightMaterials.TryAdd(material.Key, material);
						}
						else if (block.ID == (uint)BinBlockID.TexturePack)
						{
							var texturePack = TextureContainer.GetTextureContainer(this.Project.GameType);
							texturePack.Load(br);
							texturePack.MergeTexturesIntoMap(this.GlobalTextures);
							texturePack.MergeAnimationsIntoMap(this.GlobalAnimations);
						}
						else if (block.ID == (uint)BinBlockID.GeometryPack)
						{
							var geometryPack = MeshContainer.GetMeshContainer(this.Project.GameType);
							geometryPack.Load(br);
							geometryPack.MergeMeshesIntoMap(this.GlobalMeshes);
						}

						br.BaseStream.Position = block.End;
					}
				}
			}
		}
		private void LoadLocationAssets()
		{
			var folder = Path.Combine(this.Project.Folder, Namings.LocationFolder);

			if (!Directory.Exists(folder))
			{
				Debug.LogError($"Location folder {folder} does not exist!");
				return;
			}

			foreach (var extra in this.Project.Extras)
			{
				var parser = this.GetAssetParser(extra);
				if (parser is null) continue;

				var path = Path.Combine(folder, extra);
				parser.Invoke(path);
			}
		}
		private void InitAllReferencers()
		{
			foreach (var pair in this.GlobalTextures)
			{
				this.TextureReferencer.Add(pair.Key, pair.Value);
			}

			foreach (var pair in this.GlobalMeshes)
			{
				this.MeshReferencer.Add(pair.Key, pair.Value);
			}

			foreach (var mesh in this.GlobalMeshes.Values)
			{
				foreach (var subMeshInfo in mesh.SubMeshInfos)
				{
					var key = subMeshInfo.MaterialKey;

					if (!this.MaterialReferencer.Contains(key))
					{
						this.MaterialReferencer.Add(key, this.PleaseGiveMeMaterial(subMeshInfo));
					}
				}
			}
		}
		private void CloseInternal()
		{
			if (this.RenderingRoutine != null)
			{
				this.StopCoroutine(this.RenderingRoutine);
			}

			Object.Destroy(this.m_poolableRoot);

			this.LightMaterials.Clear();
			this.GlobalTextures.Clear();
			this.GlobalMeshes.Clear();
			this.GlobalAnimations.Clear();
			this.GlobalCollisions.Clear();

			this.TextureReferencer.Clear();
			this.MeshReferencer.Clear();
			this.MaterialReferencer.Clear();

			var array = this.TrackSections.Keys.ToArray();

			foreach (var section in array)
			{
				this.TrackSections[section].UnsafeUnload();
			}
		}
		private void FreezeTimeScale()
		{
			Time.timeScale = 0.0f;
		}
		private void UnfreezeTimeScale()
		{
			Time.timeScale = 1.0f;
		}
		private void ChangeSectionVisibilities()
		{
			foreach (var section in this.TrackSections.Values)
			{
				section.ChangeVisibilities();
			}
		}
		private void FillInSectionsToLoad(ushort sectionNumber, HashSet<ushort> sectionsToLoad)
		{
			if (sectionNumber == 0) return;
			int loadingSectionIndex = -1;

			// Check if we have to load LoadingSection instead of DrivableScenerySection
			for (int i = 0; i < this.VisibleSectionManager.Loadings.Count; ++i)
			{
				var loading = this.VisibleSectionManager.Loadings[i];

				for (int k = 0; k < loading.DrivableSections.Count; ++k)
				{
					if (sectionNumber == loading.DrivableSections[k])
					{
						loadingSectionIndex = i;
						break;
					}
				}

				if (loadingSectionIndex != -1) break;
			}

			// Make a HashSet of all VisibleSections that have to be loaded
			if (loadingSectionIndex == -1)
			{
				if (this.VisibleSectionManager.Drivables.TryGetValue(sectionNumber, out var drivable))
				{
					for (int i = 0; i < drivable.VisibleSections.Count; ++i)
					{
						sectionsToLoad.Add(drivable.VisibleSections[i]);
					}
				}
			}
			else
			{
				var loading = this.VisibleSectionManager.Loadings[loadingSectionIndex];

				for (int i = 0; i < loading.DrivableSections.Count; ++i)
				{
					var number = loading.DrivableSections[i];

					if (!this.VisibleSectionManager.Drivables.TryGetValue(number, out var drivable))
					{
						continue;
					}

					for (int k = 0; k < drivable.VisibleSections.Count; ++k)
					{
						sectionsToLoad.Add(drivable.VisibleSections[k]);
					}
				}

				for (int i = 0; i < loading.ExtraSections.Count; ++i)
				{
					var number = loading.ExtraSections[i];
					sectionsToLoad.Add(number);

					if (TrackHelper.IsScenerySectionDrivable(number, this.LODOffset))
					{
						number = TrackHelper.GetLODFromtDrivableNumber(number, this.LODOffset);
						sectionsToLoad.Add(number);
					}
				}
			}
		}
		private void FixupLoadedSections(List<ushort> loadedSections)
		{
			foreach (var sectionNumber in loadedSections)
			{
				var section = this.TrackSections[sectionNumber];

				if (TrackHelper.IsScenerySectionDrivable(section.SectionNumber, this.LODOffset))
				{
					var siblingNumber = TrackHelper.GetLODFromtDrivableNumber(section.SectionNumber, this.LODOffset);
					section.LODSibling = this.GetSectionOrNull(siblingNumber);
					section.DrivableSibling = section;
				}
				else if (TrackHelper.IsLODScenerySectionNumber(section.SectionNumber, this.LODOffset))
				{
					var siblingNumber = TrackHelper.GetDrivableFromtLODNumber(section.SectionNumber, this.LODOffset);
					section.DrivableSibling = this.GetSectionOrNull(siblingNumber);
					section.LODSibling = section;
				}
				else
				{
					section.LODSibling = section;
				}
			}
		}
		private void PushWrapperToPool<T, S>(T monoWrapper, string defaultName, DynamicPool<T> pool) where T : ObjectMonoWrapper<S>
		{
			if (!pool.Push(monoWrapper))
			{
				Object.Destroy(monoWrapper.gameObject);
			}
			else
			{
				monoWrapper.gameObject.SetActive(false);
				monoWrapper.gameObject.name = defaultName;
				monoWrapper.gameObject.transform.parent = this.m_poolableRoot.transform;
				monoWrapper.ResetData();
			}
		}
		private T PullWrapperFromPool<T, S>(GameObject prefab, DynamicPool<T> pool) where T : ObjectMonoWrapper<S>
		{
			if (pool.Pull(out var result))
			{
				return result;
			}
			else
			{
				return Object.Instantiate(prefab, this.m_poolableRoot.transform).GetComponent<T>();
			}
		}
		private async void LoadSectionsAsync(HashSet<ushort> sectionsToLoad, List<ushort> loadedSections, Action callback)
		{
			if (sectionsToLoad.Count == 0)
			{
				callback?.Invoke();
				return;
			}

			var tasks = new List<Task>(sectionsToLoad.Count);

			foreach (var sectionNumber in sectionsToLoad)
			{
				if (!this.Project.Sections.Contains(sectionNumber))
				{
					continue;
				}

				var section = this.CreateNewSection(sectionNumber);
				this.TrackSections.TryAdd(sectionNumber, section);

				loadedSections.Add(sectionNumber);
				tasks.Add(section.LoadAssets());
			}

			await Task.WhenAll(tasks);
			callback?.Invoke();
		}
		private async void SaveSectionsAsync(List<ushort> sectionToUnload, Action callback)
		{
			if (sectionToUnload.Count == 0)
			{
				callback?.Invoke();
				return;
			}

			var tasks = new List<Task>(sectionToUnload.Count);

			foreach (var sectionNumber in sectionToUnload)
			{
				tasks.Add(this.TrackSections[sectionNumber].SaveAssets());
			}

			await Task.WhenAll(tasks);
			callback?.Invoke();
		}

		// Protected methods
		protected void LoaderVisibleSectionManager(string file)
		{
			var text = File.ReadAllText(Path.Combine(this.Project.Folder, Namings.LocationFolder, file));
			this.VisibleSectionManager = JsonConvert.DeserializeObject<VisibleSectionManager>(text);

			// Fixup
			foreach (var boundary in this.VisibleSectionManager.Boundaries)
			{
				boundary.Value.SectionNumber = boundary.Key;
			}
			foreach (var drivable in this.VisibleSectionManager.Drivables)
			{
				drivable.Value.SectionNumber = drivable.Key;
			}
			foreach (var specific in this.VisibleSectionManager.Specifics)
			{
				specific.Value.TrackID = specific.Key;
			}
		}
		
		// Public methods
		public void Close()
		{
			this.State = TrackState.Empty;
			this.CloseOverride();
			this.CloseInternal();
		}
		public async void Load(ProjectHeader header, Action callback)
		{
			this.Project = header;

			await Task.Run(() =>
			{
				this.LoadGlobalAssets();
				this.LoadLocationAssets();
			});

			this.InitAllReferencers();
			this.State = TrackState.Unloaded;
			callback?.Invoke();
		}
		public TrackSection GetDefaultMeshSection() => this.GetSectionOrNull(2400);
		public TrackSection GetDefaultTextureSection() => this.GetSectionOrNull(2500);
		public TrackSection GetDefaultScenerySection() => this.GetSectionOrNull(2600);
		public TrackSection GetSectionOrNull(ushort sectionNumber)
		{
			return this.TrackSections.TryGetValue(sectionNumber, out var result) ? result : null;
		}

		public ushort GetCurrentSectionWeAreIn()
		{
			if (this.RenderCamera is null) return 0;

			var point = this.RenderCamera.transform.position;
			var current = this.VisibleSectionManager.GetDrivableBoundaryByPoint(point);

			if (current is null) return 0;
			if (current.DepthSection1 == 0) return current.SectionNumber;

			var depthName = this.VisibleSectionManager.GetDepthNameByPoint(point);

			if (depthName == 0) return current.SectionNumber;
			if (depthName == current.DepthName1) return current.DepthSection1;
			if (depthName == current.DepthName2) return current.DepthSection2;

			return current.SectionNumber;
		}
		public bool AreEditingSectionsTheSame()
		{
			this.RequestedSections.Sort();

			if (this.RequestedSections.Count != this.CurrentSections.Length) return false;

			for (int i = 0; i < this.CurrentSections.Length; ++i)
			{
				if (this.RequestedSections[i] != this.CurrentSections[i])
				{
					return false;
				}
			}

			return true;
		}
		public bool AreAllRequestedSectionsValid()
		{
			for (int i = 0; i < this.RequestedSections.Count; ++i)
			{
				if (!this.Project.Sections.Contains(this.RequestedSections[i]))
				{
					return false;
				}
			}

			return true;
		}

		public MaterialObject GetMaterialObjectOrNull(uint key)
		{
			if (this.LightMaterials.TryGetValue(key, out var result)) return result;
			else return null;
		}
		public TextureObject GetTextureObjectOrNull(uint key)
		{
			if (this.GlobalTextures.TryGetValue(key, out var result))
			{
				return result;
			}

			return this.TextureReferencer.GetNoRef(key);
		}
		public MeshObject GetMeshObjectOrNull(uint key)
		{
			return this.MeshReferencer.GetNoRef(key);
		}

		public void PushSceneryObjectToPool(SceneryObjectMonoWrapper sceneryObject)
		{
			this.PushWrapperToPool<SceneryObjectMonoWrapper, SceneryObject>(sceneryObject, ms_poolableSceneryObject, this.SceneryObjectPool);
		}
		public void PushLightObjectToPool(LightObjectMonoWrapper lightObject)
		{
			this.PushWrapperToPool<LightObjectMonoWrapper, LightObject>(lightObject, ms_poolableLightObject, this.LightObjectPool);
		}
		public void PushFlareObjectToPool(FlareObjectMonoWrapper flareObject)
		{
			this.PushWrapperToPool<FlareObjectMonoWrapper, FlareObject>(flareObject, ms_poolableFlareObject, this.FlareObjectPool);
		}
		public void PushTriggerObjectToPool(TriggerObjectMonoWrapper triggerObject)
		{
			this.PushWrapperToPool<TriggerObjectMonoWrapper, TriggerObject>(triggerObject, ms_poolableTriggerObject, this.TriggerObjectPool);
		}
		public void PushEmitterObjectToPool(EmitterObjectMonoWrapper emitterObject)
		{
			this.PushWrapperToPool<EmitterObjectMonoWrapper, EmitterObject>(emitterObject, ms_poolableEmitterObject, this.EmitterObjectPool);
		}
		public SceneryObjectMonoWrapper PullSceneryObjectFromPool()
		{
			return this.PullWrapperFromPool<SceneryObjectMonoWrapper, SceneryObject>(SceneryObjectPrefab, this.SceneryObjectPool);
		}
		public LightObjectMonoWrapper PullLightObjectFromPool()
		{
			return this.PullWrapperFromPool<LightObjectMonoWrapper, LightObject>(LightObjectPrefab, this.LightObjectPool);
		}
		public FlareObjectMonoWrapper PullFlareObjectFromPool()
		{
			return this.PullWrapperFromPool<FlareObjectMonoWrapper, FlareObject>(FlareObjectPrefab, this.FlareObjectPool);
		}
		public TriggerObjectMonoWrapper PullTriggerObjectFromPool()
		{
			return this.PullWrapperFromPool<TriggerObjectMonoWrapper, TriggerObject>(TriggerObjectPrefab, this.TriggerObjectPool);
		}
		public EmitterObjectMonoWrapper PullEmitterObjectFromPool()
		{
			return this.PullWrapperFromPool<EmitterObjectMonoWrapper, EmitterObject>(EmitterObjectPrefab, this.EmitterObjectPool);
		}

		public void SwitchTrackMode(TrackMode mode)
		{
			if (this.Mode != mode)
			{
				this.Mode = mode;

				if (this.Mode == TrackMode.Streaming)
				{
					this.LoadStreaming(0);
				}
				else if (this.Mode == TrackMode.Editing)
				{
					// todo
				}
			}
		}
		public void PleaseChangeAllVisibilities(TrackVisibility visibility)
		{
			foreach (var section in this.TrackSections.Values)
			{
				section.Visibility = visibility;
			}
		}
		public void EnableOverlay(ProjectHeader.Overlay overlay, bool keepCurrent)
		{
			if (!keepCurrent) this.RequestedSections.Clear();
			this.RequestedSections.AddRange(overlay.Sections);

			this.RenderCamera.transform.position = overlay.Position;
			this.RenderCamera.transform.rotation = overlay.Rotation;
		}

		public IEnumerator LoadStreaming(ushort sectionNumber)
		{
			// Stop time because we have to load everything
			this.FreezeTimeScale();

			// Set current controls
			this.State = TrackState.Loading;
			this.RenderingSection = sectionNumber;
			
			// Sections to load and unload
			var loadedSections = new List<ushort>();
			var sectionsToUnload = new List<ushort>();
			var sectionsToLoad = new HashSet<ushort>() { 2400, 2500, 2600 };
			
			// Add requested section and its sibling
			sectionsToLoad.Add(sectionNumber);
			sectionsToLoad.Add(TrackHelper.GetLODFromtDrivableNumber(sectionNumber, this.LODOffset));
			
			// Get all sections that have to be loaded
			this.FillInSectionsToLoad(sectionNumber, sectionsToLoad);
			
			// Determine what sections have to be unloaded
			foreach (var section in this.TrackSections.Keys)
			{
				if (!sectionsToLoad.Contains(section))
				{
					sectionsToUnload.Add(section);
				}
			}
			
			// Determine what sections are already loaded
			foreach (var section in this.TrackSections.Keys)
			{
				sectionsToLoad.Remove(section);
			}
			
			// Load sections here
			this.LoadSectionsAsync(sectionsToLoad, loadedSections, () => this.State = TrackState.Loaded);
						
			// Wait for sections to get loaded
			while (this.State != TrackState.Loaded) yield return null;

			// Fixup loaded sections
			this.FixupLoadedSections(loadedSections);

			// Set state to activating
			this.State = TrackState.Activating;

			// Generate all managed textures first
			foreach (var section in loadedSections)
			{
				this.TrackSections[section].GenerateManagedTextures();
			}

			// Skip one frame
			yield return null;

			// Generate all managed meshes second
			foreach (var section in loadedSections)
			{
				this.TrackSections[section].GenerateManagedMeshes();
			}

			// Skip one frame
			yield return null;

			// Geneerate all managed materials third
			foreach (var section in loadedSections)
			{
				this.TrackSections[section].GenerateManagedMaterials();
				yield return null;
			}

			// Activate all loaded sections one by one
			foreach (var section in loadedSections)
			{
				var trackSection = this.TrackSections[section];
				yield return this.StartCoroutine(trackSection.Activate());
			}

			// Unload and save sections that are not needed anymore
			foreach (var section in sectionsToUnload)
			{
				this.TrackSections[section].Unload();
				//yield return null;
			}

			// Unfreeze time on done
			this.UnfreezeTimeScale();
			this.State = TrackState.Activated;
		}
		public IEnumerator LoadEditing(ushort[] sectionNumbers)
		{
			// Stop time because we have to load everything
			this.FreezeTimeScale();

			// Set current controls
			this.State = TrackState.Loading;
			this.CurrentSections = sectionNumbers;

			// Initialize unloading and loading
			var loadedSections = new List<ushort>();
			var sectionsToUnload = new List<ushort>();
			var sectionsToLoad = new HashSet<ushort>() { 2500, 2400, 2600 };

			// Iterate through every requested section to load
			foreach (var sectionNumber in sectionNumbers)
			{
				// Addition sections we should load with the current one
				var addonSections = new HashSet<ushort>();

				// Initialize drivable and LOD numbers
				ushort drinumber = 0;
				ushort lodnumber = 0;

				// Assign drivable and LOD numbers
				if (TrackHelper.IsScenerySectionDrivable(sectionNumber, this.LODOffset))
				{
					drinumber = sectionNumber;
					lodnumber = TrackHelper.GetLODFromtDrivableNumber(sectionNumber, this.LODOffset);
				}
				else if (TrackHelper.IsLODScenerySectionNumber(sectionNumber, this.LODOffset))
				{
					drinumber = TrackHelper.GetDrivableFromtLODNumber(sectionNumber, this.LODOffset);
					lodnumber = sectionNumber;
				}
				else
				{
					lodnumber = sectionNumber;
				}

				// Add requested section and its sibling
				sectionsToLoad.Add(drinumber);
				sectionsToLoad.Add(lodnumber);

				// Get supposedly required sections to load
				this.FillInSectionsToLoad(drinumber, addonSections);

				// Add additional geometry and textures sections
				foreach (var section in addonSections)
				{
					if (TrackHelper.IsRawAssetSectionNumber(section))
					{
						sectionsToLoad.Add(section);
					}
				}
			}

			// Determine what sections have to be unloaded
			foreach (var sectionNumber in this.TrackSections.Keys)
			{
				if (!sectionsToLoad.Contains(sectionNumber))
				{
					sectionsToUnload.Add(sectionNumber);
				}
			}

			// Determine what sections are already loaded
			foreach (var sectionNumber in this.TrackSections.Keys)
			{
				sectionsToLoad.Remove(sectionNumber);
			}

			// Load sections here
			this.LoadSectionsAsync(sectionsToLoad, loadedSections, () => this.State = TrackState.Loaded);

			// Wait for sections to get loaded
			while (this.State != TrackState.Loaded) yield return null;

			// Fixup loaded sections
			this.FixupLoadedSections(loadedSections);

			// Set state to activating
			this.State = TrackState.Activating;

			// Generate all managed textures first
			foreach (var sectionNumber in loadedSections)
			{
				this.TrackSections[sectionNumber].GenerateManagedTextures();
			}

			// Skip one frame
			yield return null;

			// Generate all managed meshes second
			foreach (var sectionNumber in loadedSections)
			{
				this.TrackSections[sectionNumber].GenerateManagedMeshes();
			}

			// Skip one frame
			yield return null;

			// Geneerate all managed materials third
			foreach (var sectionNumber in loadedSections)
			{
				this.TrackSections[sectionNumber].GenerateManagedMaterials();
				yield return null;
			}

			// Activate all loaded sections one by one
			foreach (var sectionNumber in loadedSections)
			{
				var section = this.TrackSections[sectionNumber];
				yield return this.StartCoroutine(section.Activate());
			}

			// Wait until all sections are saved
			{
				bool doneUnloading = false;
				this.SaveSectionsAsync(sectionsToUnload, () => doneUnloading = true);
				while (!doneUnloading) yield return null;
			}

			// Unload and save sections that are not needed anymore
			foreach (var sectionNumber in sectionsToUnload)
			{
				this.TrackSections[sectionNumber].Unload();
				//yield return null;
			}

			// Unfreeze time on done
			this.UnfreezeTimeScale();
			this.State = TrackState.Activated;
		}

		private void OnDrawGizmos()
		{
			var current = Gizmos.color;
			Gizmos.color = Color.yellow;

			foreach (var section in this.TrackSections.Values)
			{
				if (section is Carbon.Runtime.TrackSection carbon)
				{
					if (!(carbon.HeliManager is null))
					{
						foreach (var coordinate in carbon.HeliManager.HeliCoordinates)
						{
							Gizmos.DrawLine(coordinate.Point1, coordinate.Point2);
							Gizmos.DrawLine(coordinate.Point2, coordinate.Point3);
							Gizmos.DrawLine(coordinate.Point3, coordinate.Point1);
						}
					}
				}
				else if (section is MostWanted.Runtime.TrackSection mostwanted)
				{
					if (!(mostwanted.HeliManager is null))
					{
						foreach (var coordinate in mostwanted.HeliManager.HeliCoordinates)
						{
							Gizmos.DrawLine(coordinate.Point1, coordinate.Point2);
							Gizmos.DrawLine(coordinate.Point2, coordinate.Point3);
							Gizmos.DrawLine(coordinate.Point3, coordinate.Point1);
						}
					}
				}
			}

			Gizmos.color = current;
		}
		private void Start()
		{
			this.InitPoolable();

			if (InstantiatePools)
			{
				this.PreparePools();
			}
		}
		private void Update()
		{
			if (this.State == TrackState.Empty)
			{
				return;
			}
			if (this.Mode == TrackMode.Streaming)
			{
				var section = this.GetCurrentSectionWeAreIn();

				if ((this.State == TrackState.Unloaded ||
					this.State == TrackState.Activated) &&
					section != this.RenderingSection)
				{
					this.RenderingRoutine = this.StartCoroutine(this.LoadStreaming(section));
				}

				// more?
			}
			if (this.Mode == TrackMode.Editing)
			{
				if ((this.State == TrackState.Unloaded ||
					this.State == TrackState.Activated) &&
					this.AreAllRequestedSectionsValid() &&
					!this.AreEditingSectionsTheSame())
				{
					this.RenderingRoutine = this.StartCoroutine(this.LoadEditing(this.RequestedSections.ToArray()));
				}

				// more?
			}

			this.ChangeSectionVisibilities();
		}
	}
}
