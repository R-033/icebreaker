using Hypercooled.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Hypercooled.Shared.Core
{
	public class Optimizer
	{
		[Flags()]
		public enum OptimizationFlags
		{
			/// <summary>
			/// Removes empty files from section folders.
			/// </summary>
			RemoveEmptyFiles = 1 << 0,

			/// <summary>
			/// Merges all asset files into one.
			/// </summary>
			MergeAssets = 1 << 1,

			/// <summary>
			/// Merges all mesh containers and texture containers by section.
			/// </summary>
			MergeContainers = 1 << 2,

			/// <summary>
			/// Removes duplicate meshes and textures by moving them to shared sections.
			/// </summary>
			RemoveDuplicates = 1 << 3,

			/// <summary>
			/// Attempts to compress all RGBA textures to their Palette counterparts.
			/// </summary>
			RGBAtoPalette = 1 << 4,

			/// <summary>
			/// Recalculates normals, tangents and bounds in all MeshObjects.
			/// </summary>
			RecalculateMeshes = 1 << 5,
		}

		private class FileAsset
		{
			public List<MeshContainer> MeshContainers { get; }
			public List<TextureContainer> TextureContainers { get; }
			public VisibleSectionUserInfo UserInfo { get; set; }

			public FileAsset()
			{
				this.MeshContainers = new List<MeshContainer>();
				this.TextureContainers = new List<TextureContainer>();
			}
		}

		public class AssetTable
		{
			public Dictionary<ushort, HashSet<uint>> MeshPerSectionOld { get; }
			public Dictionary<ushort, HashSet<uint>> MeshPerSectionNew { get; }

			public Dictionary<ushort, HashSet<uint>> TexturePerSectionOld { get; }
			public Dictionary<ushort, HashSet<uint>> TexturePerSectionNew { get; }

			public Dictionary<uint, ushort> MeshToSectionTable { get; }
			public Dictionary<uint, ushort> TextureToSectionTable { get; }

			public HashSet<uint> NewSharedMeshes { get; }
			public HashSet<uint> NewSharedTextures { get; }

			public Dictionary<ushort, bool> MeshesNeedRefactoring { get; }
			public Dictionary<ushort, bool> TexturesNeedRefactoring { get; }

			public AssetTable()
			{
				this.MeshPerSectionOld = new Dictionary<ushort, HashSet<uint>>();
				this.MeshPerSectionNew = new Dictionary<ushort, HashSet<uint>>();

				this.TexturePerSectionOld = new Dictionary<ushort, HashSet<uint>>();
				this.TexturePerSectionNew = new Dictionary<ushort, HashSet<uint>>();

				this.MeshToSectionTable = new Dictionary<uint, ushort>();
				this.TextureToSectionTable = new Dictionary<uint, ushort>();

				this.NewSharedMeshes = new HashSet<uint>();
				this.NewSharedTextures = new HashSet<uint>();

				this.MeshesNeedRefactoring = new Dictionary<ushort, bool>();
				this.TexturesNeedRefactoring = new Dictionary<ushort, bool>();
			}
		}

		private const ushort kDefaultMeshSection = 2400;
		private const ushort kDefaultTextureSection = 2500;

		public ProjectHeader Project { get; }
		public AssetTable Table { get; private set; }

		public Optimizer(ProjectHeader project)
		{
			this.Project = project;
		}

		public IEnumerator OptimizeTables(OptimizationFlags flags)
		{
			this.Table = new AssetTable();

			yield return this.InternalMaybeMergeStuff(flags);

			if ((flags & OptimizationFlags.RemoveDuplicates) != 0)
			{
				yield return this.InternalRemoveDuplicates();
			}

			if ((flags & OptimizationFlags.RGBAtoPalette) != 0)
			{

			}

			if ((flags & OptimizationFlags.RecalculateMeshes) != 0)
			{

			}

			if ((flags & OptimizationFlags.RemoveEmptyFiles) != 0)
			{

			}

			yield return null;
		}

		private IEnumerator InternalMaybeMergeStuff(OptimizationFlags flags)
		{
			if ((flags & OptimizationFlags.MergeAssets) != 0)
			{
				if ((flags & OptimizationFlags.MergeContainers) != 0)
				{
					yield return this.InternalMergerYesAssetsYesContainers();
				}
				else
				{
					yield return this.InternalMergerYesAssetsNoContainers();
				}
			}
			else
			{
				if ((flags & OptimizationFlags.MergeContainers) != 0)
				{
					yield return this.InternalMergerNoAssetsYesContainers();
				}
				else
				{
					yield break; // no merging involved
				}
			}
		}

		private IEnumerator InternalMergerYesAssetsNoContainers()
		{
			foreach (var section in this.Project.Sections)
			{
				var name = section.ToString() + Namings.BinExt;
				var directory = Path.Combine(this.Project.Folder, Namings.StreamFolder, section.ToString());
				var assetPath = Path.Combine(directory, Namings.Assets + Namings.HyperExt);
				var comboPath = Path.Combine(directory, name);

				var assets = File.ReadAllLines(assetPath);

				using (var bw = IOHelper.GetBinaryWriter(comboPath, false))
				{
					foreach (var asset in assets)
					{
						var path = Path.Combine(directory, asset);
						if (!File.Exists(path)) continue;

						bw.Write(File.ReadAllBytes(path));
						bw.GeneratePaddingPow2(0x100);
						File.Delete(path);
					}
				}

				File.WriteAllText(assetPath, name);
				yield return null;
			}
		}
		private IEnumerator InternalMergerNoAssetsYesContainers()
		{
			foreach (var section in this.Project.Sections)
			{
				yield return null;
			}
		}
		private IEnumerator InternalMergerYesAssetsYesContainers()
		{
			foreach (var section in this.Project.Sections)
			{
				yield return null;
			}
		}

		private IEnumerator InternalRemoveDuplicates()
		{
			foreach (var section in this.Project.Sections)
			{
				this.Table.MeshPerSectionOld.Add(section, new HashSet<uint>());
				this.Table.MeshPerSectionNew.Add(section, new HashSet<uint>());
				this.Table.TexturePerSectionOld.Add(section, new HashSet<uint>());
				this.Table.TexturePerSectionNew.Add(section, new HashSet<uint>());
			}

			yield return this.LoadAllAssetInformations();

			this.DetermineNewTables();
			yield return null;

			//yield return this.RebuildAllAssets();
		}

		private void ReformatUserInformation(VisibleSectionUserInfo userInfo)
		{
			if (userInfo is null) return;

			foreach (var scenery in userInfo.SceneryObjects)
			{
				if (this.Table.MeshToSectionTable.TryGetValue(scenery.MeshKeyLodA, out var lodA))
				{
					scenery.SectionLodA = lodA;
				}
				if (this.Table.MeshToSectionTable.TryGetValue(scenery.MeshKeyLodB, out var lodB))
				{
					scenery.SectionLodB = lodB;
				}
				if (this.Table.MeshToSectionTable.TryGetValue(scenery.MeshKeyLodC, out var lodC))
				{
					scenery.SectionLodC = lodC;
				}
				if (this.Table.MeshToSectionTable.TryGetValue(scenery.MeshKeyLodD, out var lodD))
				{
					scenery.SectionLodD = lodD;
				}
			}
		}
		private void ReadAnimationContainer(BinaryReader br, TextureContainer textures)
		{
			switch (this.Project.GameType)
			{
				case Managed.Game.Underground1:
					return;

				case Managed.Game.Underground2:
					var ug2container = new Underground2.Core.AnimContainer(textures.AnimationMap);
					ug2container.Load(br);
					return;

				case Managed.Game.MostWanted:
					var mwcontainer = new MostWanted.Core.AnimContainer(textures.AnimationMap);
					mwcontainer.Load(br);
					return;

				case Managed.Game.Carbon:
					return;

				default: return;
			}
		}
		private void GetFileInformation(string path, HashSet<uint> meshes, HashSet<uint> textures)
		{
			using (var br = IOHelper.GetBinaryReader(path, false))
			{
				while (br.BaseStream.Position < br.BaseStream.Length)
				{
					var id = (BinBlockID)br.ReadUInt32();
					var size = br.ReadInt32();
					var offset = br.BaseStream.Position;

					if (id == BinBlockID.GeometryPack)
					{
						br.BaseStream.Position = offset;

						var container = this.RequestNewMeshContainer();
						var keys = container.GetAllKeys(br);

						foreach (var key in keys) meshes.Add(key);
					}
					else if (id == BinBlockID.TexturePack)
					{
						br.BaseStream.Position = offset;

						var container = this.RequestNewTextureContainer();
						var keys = container.GetAllKeys(br);

						foreach (var key in keys) textures.Add(key);
					}

					br.BaseStream.Position = offset + size;
				}
			}
		}
		private MeshContainer RequestNewMeshContainer()
		{
			return MeshContainer.GetMeshContainer(this.Project.GameType);
		}
		private TextureContainer RequestNewTextureContainer()
		{
			return TextureContainer.GetTextureContainer(this.Project.GameType);
		}

		private FileAsset LoadFileAsset(string path)
		{
			var fileAsset = new FileAsset();

			TextureContainer lastContainer = null;

			using (var br = IOHelper.GetBinaryReader(path, true))
			{
				while (br.BaseStream.Position < br.BaseStream.Length)
				{
					var id = (BinBlockID)br.ReadUInt32();
					var size = br.ReadInt32();
					var offset = br.BaseStream.Position;

					if (id == BinBlockID.GeometryPack)
					{
						br.BaseStream.Position = offset;

						var container = this.RequestNewMeshContainer();
						container.Load(br);

						fileAsset.MeshContainers.Add(container);
					}
					else if (id == BinBlockID.TexturePack)
					{
						br.BaseStream.Position = offset;

						var container = this.RequestNewTextureContainer();
						container.Load(br);

						fileAsset.TextureContainers.Add(container);
						lastContainer = container;
					}
					else if (id == BinBlockID.TextureAnimPack)
					{
						br.BaseStream.Position = offset;
						this.ReadAnimationContainer(br, lastContainer);
					}
					else if (id == BinBlockID.VisibleSectionUserInfo)
					{
						fileAsset.UserInfo = VisibleSectionUserInfo.Deserialize(br);
					}

					br.BaseStream.Position = offset + size;
				}
			}

			return fileAsset;
		}
		private void SaveFileAsset(string path, FileAsset fileAsset)
		{

		}

		private IEnumerator LoadAllAssetInformations()
		{
			foreach (var section in this.Project.Sections)
			{
				var directory = Path.Combine(this.Project.Folder, Namings.StreamFolder, section.ToString());
				var assetFile = Path.Combine(directory, Namings.Assets);
				this.LoadAssetFileInformation(section, directory, assetFile);
				yield return null;
			}
		}

		private void LoadAssetFileInformation(ushort section, string directory, string assetFile)
		{
			if (!File.Exists(assetFile)) return;

			using (var sr = IOHelper.GetStreamReader(assetFile, true))
			{
				while (!sr.EndOfStream)
				{
					var path = Path.Combine(directory, sr.ReadLine());
					if (!File.Exists(path)) continue;

					var meshes = this.Table.MeshPerSectionOld[section];
					var textures = this.Table.TexturePerSectionOld[section];

					this.GetFileInformation(path, meshes, textures);
				}
			}
		}

		private void DetermineNewTables()
		{
			var existingMeshes = new HashSet<uint>();
			var existingTextures = new HashSet<uint>();

			foreach (var section in this.Table.MeshPerSectionOld)
			{
				bool meshesNeedRefactoring = false;

				foreach (var key in section.Value)
				{
					if (existingMeshes.Add(key)) // if newly encountered mesh
					{
						this.Table.MeshPerSectionNew[section.Key].Add(key); // push to the current section
						this.Table.MeshToSectionTable[key] = section.Key;   // add to the section table map
					}
					else // else if this mesh was previously encounter
					{
						if (this.Table.MeshToSectionTable[key] != kDefaultMeshSection) // if it is not in default section
						{
							// Remove shared mesh from section and move it to default one
							this.Table.MeshPerSectionNew[this.Table.MeshToSectionTable[key]].Remove(key);
							this.Table.MeshToSectionTable[key] = kDefaultMeshSection;
							this.Table.MeshPerSectionNew[kDefaultMeshSection].Add(key);

							this.Table.NewSharedMeshes.Add(key);
							meshesNeedRefactoring = true;
						}
					}
				}

				this.Table.MeshesNeedRefactoring.Add(section.Key, meshesNeedRefactoring);
			}

			foreach (var section in this.Table.TexturePerSectionOld)
			{
				bool texturesNeedRefactoring = false;

				foreach (var key in section.Value)
				{
					if (existingTextures.Add(key)) // if newly encountered texture
					{
						this.Table.TexturePerSectionNew[section.Key].Add(key); // push to the current section
						this.Table.TextureToSectionTable[key] = section.Key;   // add to the section table map
					}
					else // else if this texture was previously encounter
					{
						if (this.Table.TextureToSectionTable[key] != kDefaultTextureSection) // if it is not in default section
						{
							// Remove shared texture from section and move it to default one
							this.Table.TexturePerSectionNew[this.Table.TextureToSectionTable[key]].Remove(key);
							this.Table.TextureToSectionTable[key] = kDefaultTextureSection;
							this.Table.TexturePerSectionNew[kDefaultTextureSection].Add(key);

							this.Table.NewSharedTextures.Add(key);
							texturesNeedRefactoring = true;
						}
					}
				}

				this.Table.TexturesNeedRefactoring.Add(section.Key, texturesNeedRefactoring);
			}
		}

		private void RebuildAllAssets()
		{
			var sharedMeshes = this.RequestNewMeshContainer();
			var sharedTextures = this.RequestNewTextureContainer();

			foreach (var section in this.Project.Sections)
			{
				if (section == kDefaultMeshSection || section == kDefaultTextureSection) continue;

				var directory = Path.Combine(this.Project.Folder, Namings.StreamFolder, section.ToString());
				var assetFile = Path.Combine(directory, Namings.Assets);

				if (!File.Exists(assetFile)) return;

				using (var sr = IOHelper.GetStreamReader(assetFile, true))
				{
					while (!sr.EndOfStream)
					{
						var path = Path.Combine(directory, sr.ReadLine());
						var fileAsset = this.LoadFileAsset(path);

						if (fileAsset is null) continue;

						foreach (var container in fileAsset.MeshContainers)
						{
							var keys = container.MeshMap.Keys.ToArray();

							foreach (var key in keys)
							{
								var belongs = this.Table.MeshToSectionTable[key];

								if (belongs == kDefaultMeshSection)
								{
									container.MeshMap.TryRemove(key, out var meshObject);

									if (this.Table.NewSharedMeshes.Contains(key))
									{
										sharedMeshes.MeshMap.TryAdd(key, meshObject);
									}
								}
								else if (belongs != section)
								{
									throw new Exception("H0W...");
								}
							}
						}

						foreach (var container in fileAsset.TextureContainers)
						{
							var keys = container.TextureMap.Keys.ToArray();

							foreach (var key in keys)
							{
								var belongs = this.Table.TextureToSectionTable[key];

								if (belongs == kDefaultTextureSection)
								{
									container.TextureMap.TryRemove(key, out var textureObject);

									if (this.Table.NewSharedTextures.Contains(key))
									{
										sharedTextures.TextureMap.TryAdd(key, textureObject);
									}

									if (container.AnimationMap.TryRemove(key, out var animobject) &&
										this.Table.NewSharedTextures.Contains(key))
									{
										sharedTextures.AnimationMap.TryAdd(key, animobject);
									}
								}
								else if (belongs != section)
								{
									throw new Exception("I'm done...");
								}
							}
						}

						this.SaveFileAsset(path, fileAsset);
					}
				}
			}
		}
	}
}
