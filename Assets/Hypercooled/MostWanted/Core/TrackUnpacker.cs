using Hypercooled.Managed;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Namings = Hypercooled.Shared.Core.Namings;

namespace Hypercooled.MostWanted.Core
{
	public class TrackUnpacker : Shared.Core.TrackUnpacker
	{
		private List<MapStream.SceneryBarrierGroup> m_sceneryBarrierGroups;
		private List<MapStream.SceneryOverrideInfo> m_sceneryOverrides;
		private List<Shared.MapStream.ModelHierarchy> m_modelHierarchies;
		private readonly List<Parsers.SmokeableSpawnerParser> m_smokeables;
		private readonly List<TopologyManager> m_topologies;

		public TrackUnpacker(string folder, string globalB, string lxry) : base(Game.MostWanted, folder, globalB, lxry)
		{
			this.m_modelHierarchies = new List<Shared.MapStream.ModelHierarchy>();
			this.m_smokeables = new List<Parsers.SmokeableSpawnerParser>();
			this.m_topologies = new List<TopologyManager>();
		}

		protected override void InternalUnpackFixup()
		{
			this.FixupSceneryGroups();
			this.FixupSmokeableSpawners();
			this.FixupWCollisionAssets();
			this.FixupModelHierarchies();
		}
		protected override void RegisterLocationParsers(BlockReader reader)
		{
			reader.RegisterParser((uint)BinBlockID.TrackStreamingBarriers, () => new Shared.Parsers.TrackStreamingBarrierParser());
			reader.RegisterParser((uint)BinBlockID.TrackObjectBounds, () => new Shared.Parsers.TrackObjectBoundsParser());
			reader.RegisterParser((uint)BinBlockID.TrackPositionMarkers, () => new Shared.Parsers.TrackPositionMarkerParser());
			//reader.RegisterParser((uint)BinBlockID.TrackPathManager, () => new Parsers.TrackPathManagerParser());
			reader.RegisterParser((uint)BinBlockID.TrackPathBarriers, () => new Parsers.TrackPathBarrierParser());
			reader.RegisterParser((uint)BinBlockID.VisibleSectionManager, () => new Parsers.VisibleManagerParser());
			reader.RegisterParser((uint)BinBlockID.TrackStreamingSections, () => new Parsers.TrackStreamingSectionParser());
			reader.RegisterParser((uint)BinBlockID.TrackStreamingInfos, () => new Shared.Parsers.TrackStreamingInfoParser());
			reader.RegisterParser((uint)BinBlockID.SceneryOverrideInfos, () => new Parsers.SceneryOverrideInfoParser());
			reader.RegisterParser((uint)BinBlockID.SceneryBarrierGroups, () => new Parsers.SceneryBarrierGroupParser());
			reader.RegisterParser((uint)BinBlockID.CollisionVolumes, () => new Parsers.CollisionVolumesParser());
			reader.RegisterParser((uint)BinBlockID.ModelHierarchyTree, () => new Shared.Parsers.ModelHierarchyTreeParser());
			//reader.RegisterParser((uint)BinBlockID.UppleUWorld, () => new Parsers.UppleUWorldParser());

			//reader.RegisterParser((uint)BinBlockID.WorldAnimDirectories, () => new Parsers.WorldAnimDirectoryParser());
			//reader.RegisterParser((uint)BinBlockID.WorldAnimEntities, () => new Parsers.WorldAnimEntityParser());
			//reader.RegisterParser((uint)BinBlockID.WorldAnimEventDirs, () => new Parsers.WorldAnimEventDirParser());
			//reader.RegisterParser((uint)BinBlockID.WorldAnimInstances, () => new Parsers.WorldAnimInstanceParser());
			//reader.RegisterParser((uint)BinBlockID.WorldAnimTreeMarkers, () => new Parsers.WorldAnimTreeMarkerParser());
			//reader.RegisterParser((uint)BinBlockID.EAGLAnimations, () => new Shared.Parsers.GenericParser());

			reader.RegisterParser((uint)BinBlockID.ParameterMaps, () => new Shared.Parsers.GenericParser());
			reader.RegisterParser((uint)BinBlockID.Weatherman, () => new Shared.Parsers.GenericParser());
			reader.RegisterParser((uint)BinBlockID.EventSystem, () => new Shared.Parsers.GenericParser());
			reader.RegisterParser((uint)BinBlockID.Quicksplines, () => new Shared.Parsers.GenericParser());
			reader.RegisterParser((uint)BinBlockID.VisibleSectionOverlays, () => new Shared.Parsers.GenericParser());
			reader.RegisterParser((uint)BinBlockID.TrackStreamingDiscBundle, () => new Shared.Parsers.GenericParser());
		}
		protected override void RegisterStreamParsers(BlockReader reader)
		{
			reader.RegisterParser((uint)BinBlockID.GeometryPack, () => new Parsers.GeometryPackParser());
			reader.RegisterParser((uint)BinBlockID.ScenerySection, () => new Parsers.ScenerySectionParser());
			reader.RegisterParser((uint)BinBlockID.TexturePack, () => new Parsers.TexturePackParser());
			reader.RegisterParser((uint)BinBlockID.TextureAnimPack, () => new Parsers.TextureAnimPackParser());
			reader.RegisterParser((uint)BinBlockID.LightSourcesPack, () => new Shared.Parsers.LightSourcePackParser());
			reader.RegisterParser((uint)BinBlockID.LightFlaresPack, () => new Shared.Parsers.LightFlarePackParser());
			reader.RegisterParser((uint)BinBlockID.EventTriggerPack, () => new Shared.Parsers.EventTriggerPackParser());
			reader.RegisterParser((uint)BinBlockID.SmokeableSpawners, () => new Parsers.SmokeableSpawnerParser());
			reader.RegisterParser((uint)BinBlockID.HeliSheetManager, () => new Shared.Parsers.HeliSectionManagerParser());
			reader.RegisterParser((uint)BinBlockID.WCollisionAssets, () => new Parsers.WCollisionAssetParser());
			//reader.RegisterParser((uint)BinBlockID.WorldGridMaker, () => new Parsers.WorldGridMakerParser());
			reader.RegisterParser((uint)BinBlockID.EmitterSystem, () => new Parsers.EmitterSystemParser());
		}
		protected override void ResolveLocationParser(IParser iparser)
		{
			switch ((BinBlockID)iparser.BlockID)
			{
				case BinBlockID.TrackStreamingBarriers: this.UnfoldTrackStreamingBarriers(iparser); break;
				case BinBlockID.TrackObjectBounds: this.UnfoldTrackObjectBounds(iparser); break;
				case BinBlockID.TrackPositionMarkers: this.UnfoldTrackPositionMarkers(iparser); break;
				case BinBlockID.TrackPathManager: this.UnfoldTrackPathManager(iparser); break;
				case BinBlockID.TrackPathBarriers: this.UnfoldTrackPathBarriers(iparser); break;
				case BinBlockID.VisibleSectionManager: this.UnfoldVisibleSectionManager(iparser); break;
				case BinBlockID.TrackStreamingSections: this.UnfoldTrackStreamingSections(iparser); break;
				case BinBlockID.TrackStreamingInfos: this.UnfoldTrackStreamingInfos(iparser); break;
				case BinBlockID.SceneryOverrideInfos: this.UnfoldSceneryOverrideInfos(iparser); break;
				case BinBlockID.SceneryBarrierGroups: this.UnfoldSceneryBarrierGroups(iparser); break;
				case BinBlockID.CollisionVolumes: this.UnfoldCollisionVolumes(iparser); break;
				case BinBlockID.ModelHierarchyTree: this.UnfoldModelHierarchyTree(iparser); break;
				case BinBlockID.UppleUWorld: this.UnfoldUppleUWorld(iparser); break;

				// WorldAnim stuff

				case BinBlockID.ParameterMaps: this.UnfoldGenericDataStruct(iparser); break;
				case BinBlockID.Weatherman: this.UnfoldGenericDataStruct(iparser); break;
				case BinBlockID.EventSystem: this.UnfoldGenericDataStruct(iparser); break;
				case BinBlockID.Quicksplines: this.UnfoldGenericDataStruct(iparser); break;
				case BinBlockID.VisibleSectionOverlays: this.UnfoldGenericDataStruct(iparser); break;
				case BinBlockID.TrackStreamingDiscBundle: this.UnfoldGenericDataStruct(iparser); break;
				default: break;
			}
		}
		protected override void ResolveStreamParsers(List<IParser> iparsers, ushort sectionNumber)
		{
			int geometryIndex = 0;
			int textureIndex = 0;

			foreach (var iparser in iparsers)
			{
				switch ((BinBlockID)iparser.BlockID)
				{
					case BinBlockID.GeometryPack: this.UnfoldGeometryPack(iparser, sectionNumber, geometryIndex++); break;
					case BinBlockID.TexturePack: this.UnfoldTexturePack(iparser, sectionNumber, textureIndex++); break;
					case BinBlockID.ScenerySection: this.UnfoldScenerySection(iparser); break;
					case BinBlockID.LightSourcesPack: this.UnfoldLightSourcePack(iparser); break;
					case BinBlockID.LightFlaresPack: this.UnfoldLightFlarePack(iparser); break;
					case BinBlockID.EventTriggerPack: this.UnfoldEventTriggerPack(iparser); break;
					case BinBlockID.SmokeableSpawners: this.UnfoldSmokeableSpawners(iparser); break;
					case BinBlockID.HeliSheetManager: this.UnfoldHeliSheetManager(iparser); break;
					case BinBlockID.WCollisionAssets: this.UnfoldWCollisionAssets(iparser); break;
					case BinBlockID.EmitterSystem: this.UnfoldEmitterSystem(iparser); break;
					default: break;
				}
			}
		}

		private void UnfoldTrackStreamingBarriers(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.TrackStreamingBarrierParser;

			var name = BinBlockID.TrackStreamingBarriers.ToString() + Namings.HyperExt;
			var path = Path.Combine(this.m_pathLocation, name);
			var json = JsonConvert.SerializeObject(parser.TrackStreamingBarrierMap, Formatting.Indented);

			File.WriteAllText(path, json);
			this.m_header.Extras.Add(name);
		}
		private void UnfoldTrackObjectBounds(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.TrackObjectBoundsParser;

			var name = BinBlockID.TrackObjectBounds.ToString() + Namings.HyperExt;
			var path = Path.Combine(this.m_pathLocation, name);
			var json = JsonConvert.SerializeObject(parser.TrackOOBs, Formatting.Indented);

			File.WriteAllText(path, json);
			this.m_header.Extras.Add(name);
		}
		private void UnfoldTrackPositionMarkers(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.TrackPositionMarkerParser;

			var name = BinBlockID.TrackPositionMarkers.ToString() + Namings.HyperExt;
			var path = Path.Combine(this.m_pathLocation, name);
			var json = JsonConvert.SerializeObject(parser.TrackPositionMarkers, Formatting.Indented);

			File.WriteAllText(path, json);
			this.m_header.Extras.Add(name);
		}
		private void UnfoldTrackPathManager(IParser iparser)
		{
			// todo
		}
		private void UnfoldTrackPathBarriers(IParser iparser)
		{
			var parser = iparser as Parsers.TrackPathBarrierParser;

			var name = BinBlockID.TrackPathBarriers.ToString() + Namings.HyperExt;
			var path = Path.Combine(this.m_pathLocation, name);
			var json = JsonConvert.SerializeObject(parser.TrackPathBarriers, Formatting.Indented);

			File.WriteAllText(path, json);
			this.m_header.Extras.Add(name);
		}
		private void UnfoldVisibleSectionManager(IParser iparser)
		{
			var parser = iparser as Parsers.VisibleManagerParser;
			var manager = MapStream.VisibleManager.ToVisibleManager(parser.SectionManager);

			var name = BinBlockID.VisibleSectionManager.ToString() + Namings.HyperExt;
			var path = Path.Combine(this.m_pathLocation, name);
			var json = JsonConvert.SerializeObject(manager, Formatting.Indented);

			File.WriteAllText(path, json);
			this.m_header.Extras.Add(name);
		}
		private void UnfoldTrackStreamingSections(IParser iparser)
		{
			var parser = iparser as Parsers.TrackStreamingSectionParser;

			foreach (var section in parser.TrackStreamingSectionMap)
			{
				var streaming = new StreamingSection(section.SectionNumber, section.FileOffset, section.Size);
				this.m_streamingSections.Add(streaming);
			}
		}
		private void UnfoldTrackStreamingInfos(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.TrackStreamingInfoParser;

			var name = BinBlockID.TrackStreamingInfos.ToString() + Namings.HyperExt;
			var path = Path.Combine(this.m_pathLocation, name);
			var json = JsonConvert.SerializeObject(parser.TrackStreamingInfos, Formatting.Indented);

			File.WriteAllText(path, json);
			this.m_header.Extras.Add(name);
		}
		private void UnfoldSceneryOverrideInfos(IParser iparser)
		{
			var parser = iparser as Parsers.SceneryOverrideInfoParser;
			this.m_sceneryOverrides = parser.SceneryOverrideInfoMap;
		}
		private void UnfoldSceneryBarrierGroups(IParser iparser)
		{
			var parser = iparser as Parsers.SceneryBarrierGroupParser;
			this.m_sceneryBarrierGroups = parser.SceneryBarrierGroups;
		}
		private void UnfoldCollisionVolumes(IParser iparser)
		{
			var parser = iparser as Parsers.CollisionVolumesParser;

			var collisionVolumes = BinBlockID.CollisionVolumes.ToString();
			var path = Path.Combine(this.m_pathLocation, collisionVolumes);
			var main = Path.Combine(path, Namings.Assets + Namings.HyperExt);

			Directory.CreateDirectory(path);
			var files = new List<string>();

			foreach (var cv in parser.CollisionObjects)
			{
				var name = cv.CollectionName + Namings.BinExt;
				var file = Path.Combine(path, name);

				using (var bw = IOHelper.GetBinaryWriter(file, false))
				{
					cv.Serialize(bw);
				}

				files.Add(name);
			}

			File.WriteAllLines(main, files);
			this.m_header.Extras.Add(collisionVolumes);
		}
		private void UnfoldModelHierarchyTree(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.ModelHierarchyTreeParser;
			this.m_modelHierarchies = parser.Hierarchies; // later b/c hashes
		}
		private void UnfoldUppleUWorld(IParser iparser)
		{
			// todo
		}
		// todo: world anim and eaglanim
		private void UnfoldGenericDataStruct(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.GenericParser;

			var name = ((BinBlockID)parser.BlockID).ToString() + Namings.BinExt;
			var path = Path.Combine(this.m_pathLocation, name);

			File.WriteAllBytes(path, parser.Data);
			this.m_header.Extras.Add(name);
		}

		private void UnfoldTexturePack(IParser iparser, ushort sectionNumber, int index)
		{
			var parser = iparser as Parsers.TexturePackParser;

			var name = Namings.TexturesPack + index.ToString() + Namings.BinExt;
			var path = Path.Combine(this.m_pathStream, sectionNumber.ToString(), name);

			this.AddAssetName(sectionNumber, name);
			parser.Container.SetContainerName($"{sectionNumber}_{index}");

			using (var bw = IOHelper.GetBinaryWriter(path, false))
			{
				parser.Container.Save(bw);
			}
		}
		private void UnfoldGeometryPack(IParser iparser, ushort sectionNumber, int index)
		{
			var parser = iparser as Parsers.GeometryPackParser;

			var name = Namings.GeometryPack + index.ToString() + Namings.BinExt;
			var path = Path.Combine(this.m_pathStream, sectionNumber.ToString(), name);

			this.AddAssetName(sectionNumber, name);
			parser.Container.SetContainerName($"{sectionNumber}_{index}");

			using (var bw = IOHelper.GetBinaryWriter(path, false))
			{
				parser.Container.Save(bw);
			}
		}
		private void UnfoldScenerySection(IParser iparser)
		{
			var parser = iparser as Parsers.ScenerySectionParser;
			var scenery = parser.Scenery;

			if (!this.m_userInfos.TryGetValue(scenery.SectionNumber, out var userInfo))
			{
				userInfo = this.CreateNewUserInfo(Game.MostWanted, scenery.SectionNumber);
			}

			foreach (var sceneryInstance in scenery.SceneryInstances)
			{
				var sceneryInfo = scenery.SceneryInfos[sceneryInstance.SceneryInfoNumber];
				var obj = new SceneryObject(scenery.SectionNumber, sceneryInstance, sceneryInfo);
				userInfo.SceneryObjects.Add(obj);
			}
		}
		private void UnfoldLightSourcePack(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.LightSourcePackParser;

			if (!this.m_userInfos.TryGetValue(parser.SectionNumber, out var userInfo))
			{
				userInfo = this.CreateNewUserInfo(Game.MostWanted, parser.SectionNumber);
			}

			foreach (var lightSource in parser.LightSources)
			{
				userInfo.LightObjects.Add(new Shared.Core.LightObject(lightSource));
			}
		}
		private void UnfoldLightFlarePack(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.LightFlarePackParser;

			if (!this.m_userInfos.TryGetValue(parser.SectionNumber, out var userInfo))
			{
				userInfo = this.CreateNewUserInfo(Game.MostWanted, parser.SectionNumber);
			}

			foreach (var lightFlare in parser.LightFlares)
			{
				userInfo.FlareObjects.Add(new Shared.Core.FlareObject(lightFlare));
			}
		}
		private void UnfoldEventTriggerPack(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.EventTriggerPackParser;

			if (!this.m_userInfos.TryGetValue(parser.SectionNumber, out var userInfo))
			{
				userInfo = this.CreateNewUserInfo(Game.MostWanted, parser.SectionNumber);
			}

			foreach (var eventTrigger in parser.EventTriggers)
			{
				userInfo.TriggerObjects.Add(new Shared.Core.TriggerObject(parser.SectionNumber, eventTrigger));
			}
		}
		private void UnfoldSmokeableSpawners(IParser iparser)
		{
			var parser = iparser as Parsers.SmokeableSpawnerParser;
			this.m_smokeables.Add(parser);
		}
		private void UnfoldHeliSheetManager(IParser iparser)
		{
			var parser = iparser as Shared.Parsers.HeliSectionManagerParser;

			var name = BinBlockID.HeliSheetManager.ToString() + Namings.BinExt;
			var path = Path.Combine(this.m_pathStream, parser.HeliManager.SectionNumber.ToString(), name);

			this.AddAssetName(parser.HeliManager.SectionNumber, name);

			using (var bw = IOHelper.GetBinaryWriter(path, false))
			{
				parser.HeliManager.Save(bw);
			}
		}
		private void UnfoldWCollisionAssets(IParser iparser)
		{
			var parser = iparser as Parsers.WCollisionAssetParser;
			this.m_topologies.Add(parser.Topology);
		}
		private void UnfoldEmitterSystem(IParser iparser)
		{
			var parser = iparser as Parsers.EmitterSystemParser;

			if (!this.m_userInfos.TryGetValue(parser.SectionNumber, out var userInfo))
			{
				userInfo = this.CreateNewUserInfo(Game.MostWanted, parser.SectionNumber);
			}

			foreach (var effect in parser.AcidEffects)
			{
				userInfo.EmitterObjects.Add(new Shared.Core.EmitterObject(effect));
			}
		}

		private void FixupSceneryGroups()
		{
			if (this.m_sceneryBarrierGroups is null) return;
			if (this.m_sceneryOverrides is null) return;

			this.m_sceneryBarrierGroups.Sort((x, y) => x.Index.CompareTo(y.Index));

			foreach (var group in this.m_sceneryBarrierGroups)
			{
				for (int i = 0; i < group.Overrides.Length; ++i)
				{
					var overrideInfo = this.m_sceneryOverrides[group.Overrides[i]];

					if (this.m_userInfos.TryGetValue(overrideInfo.SectionNumber, out var userInfo))
					{
						var instance = userInfo.SceneryObjects[overrideInfo.InstanceIndex];
						instance.HasOverride = true;
						instance.ExcludeFlags = overrideInfo.ExcludeFlags;
						instance.SceneryGroups.Add(group.Key);
					}
				}
			}
		}
		private void FixupSmokeableSpawners()
		{
			foreach (var parser in this.m_smokeables)
			{
				foreach (var smokeable in parser.SmokeableSpawners)
				{
					var overrideInfo = this.m_sceneryOverrides[smokeable.SceneryOverrideInfoNumber];

					if (this.m_userInfos.TryGetValue(overrideInfo.SectionNumber, out var userInfo))
					{
						var instance = userInfo.SceneryObjects[overrideInfo.InstanceIndex] as SceneryObject;

						instance.IsSmackable = true;

						var rotation = smokeable.Rotation.eulerAngles;
						rotation = new Vector3(rotation.x, rotation.z, rotation.y);
						instance.SpawnerRotation = Quaternion.Euler(rotation);
						instance.SpawnerPosition = new Vector3(smokeable.Position.x, smokeable.Position.z, smokeable.Position.y);
						instance.SpawnerScale = new Vector3(instance.Transform.m00, instance.Transform.m11, instance.Transform.m22);

						instance.SpawnerReplacementKey = smokeable.Model;
						instance.SpawnerCollisionKey = smokeable.CollisionKey;
						instance.SpawnerAttributes = smokeable.Attributes;
						instance.SpawnerExcludeFlags = smokeable.ExcludeFlags;
						instance.SpawnerSpawnFlags = smokeable.SpawnFlags;
					}
				}
			}
		}
		private void FixupWCollisionAssets()
		{
			bool areSceneriesNulled = this.m_sceneryBarrierGroups is null;

			var sceneryNames = new List<string>(this.m_sceneryBarrierGroups.Select((group) => group.Name));

			foreach (var tree in this.m_topologies)
			{
				if (!areSceneriesNulled)
				{
					tree.FromGroupNames(sceneryNames);
				}

				var name = Namings.Topology + Namings.BinExt;
				var path = Path.Combine(this.m_pathStream, tree.SectionNumber.ToString(), name);

				this.AddAssetName(tree.SectionNumber, name);

				using (var bw = IOHelper.GetBinaryWriter(path, false))
				{
					tree.Save(bw);
				}
			}
		}
		private void FixupModelHierarchies()
		{
			var name = BinBlockID.ModelHierarchyTree.ToString() + Namings.HyperExt;
			var path = Path.Combine(this.m_pathLocation, name);
			var json = JsonConvert.SerializeObject(this.m_modelHierarchies, Formatting.Indented);

			File.WriteAllText(path, json);
			this.m_header.Extras.Add(name);
		}
	}
}
