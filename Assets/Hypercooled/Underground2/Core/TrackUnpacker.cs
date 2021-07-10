using Hypercooled.Managed;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Namings = Hypercooled.Shared.Core.Namings;

namespace Hypercooled.Underground2.Core
{
	public class TrackUnpacker : Shared.Core.TrackUnpacker
	{
		private List<MapStream.SceneryBarrierGroup> m_sceneryBarrierGroups;
		private List<MapStream.SceneryOverrideInfo> m_sceneryOverrides;
		private List<MapStream.TopologyBarrierGroup> m_topologyBarrierGroups;
		private readonly List<Parsers.SmokeableSpawnerParser> m_smokeables;
		private readonly List<TopologyManager> m_topologies;

		public TrackUnpacker(string folder, string globalB, string lxry) : base(Game.Underground2, folder, globalB, lxry)
		{
			this.m_smokeables = new List<Parsers.SmokeableSpawnerParser>();
			this.m_topologies = new List<TopologyManager>();
		}

		protected override void InternalUnpackFixup()
		{
			this.FixupSceneryGroups();
			this.FixupSmokeableSpawners();
			this.FixupTopologyTrees();
		}
		protected override void RegisterLocationParsers(BlockReader reader)
		{
			reader.RegisterParser((uint)BinBlockID.TrackStreamingBarriers, () => new Shared.Parsers.TrackStreamingBarrierParser());
			reader.RegisterParser((uint)BinBlockID.VisibleSectionManager, () => new Parsers.VisibleManagerParser());
			reader.RegisterParser((uint)BinBlockID.TrackStreamingSections, () => new Parsers.TrackStreamingSectionParser());
			reader.RegisterParser((uint)BinBlockID.TrackStreamingInfos, () => new Shared.Parsers.TrackStreamingInfoParser());
			reader.RegisterParser((uint)BinBlockID.SceneryOverrideInfos, () => new Parsers.SceneryOverrideInfoParser());
			reader.RegisterParser((uint)BinBlockID.SceneryBarrierGroups, () => new Parsers.SceneryBarrierGroupParser());
			reader.RegisterParser((uint)BinBlockID.CollisionVolumes, () => new Parsers.CollisionVolumesParser());
			reader.RegisterParser((uint)BinBlockID.TopologyBarrierGroups, () => new Parsers.TopologyBarrierGroupParser());
			reader.RegisterParser((uint)BinBlockID.AcidEmitters, () => new Parsers.AcidEmitterParser());
			reader.RegisterParser((uint)BinBlockID.SmokeableInfos, () => new Parsers.SmokeableInfoParser());

			//reader.RegisterParser((uint)BinBlockID.WorldAnimDirectories, () => new Parsers.WorldAnimDirectoryParser());
			//reader.RegisterParser((uint)BinBlockID.WorldAnimEntities, () => new Parsers.WorldAnimEntityParser());
			//reader.RegisterParser((uint)BinBlockID.WorldAnimEventDirs, () => new Parsers.WorldAnimEventDirParser());
			//reader.RegisterParser((uint)BinBlockID.WorldAnimInstances, () => new Parsers.WorldAnimInstanceParser());
			//reader.RegisterParser((uint)BinBlockID.WorldAnimTreeMarkers, () => new Parsers.WorldAnimTreeMarkerParser());
			//reader.RegisterParser((uint)BinBlockID.EAGLAnimations, () => new Shared.Parsers.GenericParser());

			reader.RegisterParser((uint)BinBlockID.ParameterMaps, () => new Shared.Parsers.GenericParser());
			reader.RegisterParser((uint)BinBlockID.DragCameras, () => new Shared.Parsers.GenericParser());
			reader.RegisterParser((uint)BinBlockID.Weatherman, () => new Shared.Parsers.GenericParser());
			reader.RegisterParser((uint)BinBlockID.Quicksplines, () => new Shared.Parsers.GenericParser());
			reader.RegisterParser((uint)BinBlockID.TrackStreamingDiscBundle, () => new Shared.Parsers.GenericParser());
		}
		protected override void RegisterStreamParsers(BlockReader reader)
		{
			reader.RegisterParser((uint)BinBlockID.GeometryPack, () => new Parsers.GeometryPackParser());
			reader.RegisterParser((uint)BinBlockID.ScenerySection, () => new Parsers.ScenerySectionParser());
			reader.RegisterParser((uint)BinBlockID.TexturePack, () => new Parsers.TexturePackParser());
			reader.RegisterParser((uint)BinBlockID.TextureAnimPack, () => new Parsers.TextureAnimPackParser());
			reader.RegisterParser((uint)BinBlockID.TopologyTree, () => new Parsers.TopologyTreeParser());
			reader.RegisterParser((uint)BinBlockID.LightSourcesPack, () => new Shared.Parsers.LightSourcePackParser());
			reader.RegisterParser((uint)BinBlockID.LightFlaresPack, () => new Shared.Parsers.LightFlarePackParser());
			reader.RegisterParser((uint)BinBlockID.EventTriggerPack, () => new Shared.Parsers.EventTriggerPackParser());
			reader.RegisterParser((uint)BinBlockID.SmokeableSpawners, () => new Parsers.SmokeableSpawnerParser());
			reader.RegisterParser((uint)BinBlockID.AcidEffects, () => new Parsers.AcidEffectParser());
		}
		protected override void ResolveLocationParser(IParser iparser)
		{
			switch ((BinBlockID)iparser.BlockID)
			{
				case BinBlockID.TrackStreamingBarriers: this.UnfoldTrackStreamingBarriers(iparser); break;
				case BinBlockID.VisibleSectionManager: this.UnfoldVisibleSectionManager(iparser); break;
				case BinBlockID.TrackStreamingSections: this.UnfoldTrackStreamingSections(iparser); break;
				case BinBlockID.TrackStreamingInfos: this.UnfoldTrackStreamingInfos(iparser); break;
				case BinBlockID.SceneryOverrideInfos: this.UnfoldSceneryOverrideInfos(iparser); break;
				case BinBlockID.SceneryBarrierGroups: this.UnfoldSceneryBarrierGroups(iparser); break;
				case BinBlockID.CollisionVolumes: this.UnfoldCollisionVolumes(iparser); break;
				case BinBlockID.TopologyBarrierGroups: this.UnfoldTopologyBarrierGroups(iparser); break;
				case BinBlockID.AcidEmitters: this.UnfoldAcidEmitters(iparser); break;
				case BinBlockID.SmokeableInfos: this.UnfoldSmokeableInfos(iparser); break;

				// WorldAnim stuff

				case BinBlockID.ParameterMaps: this.UnfoldGenericDataStruct(iparser); break;
				case BinBlockID.DragCameras: this.UnfoldGenericDataStruct(iparser); break;
				case BinBlockID.Weatherman: this.UnfoldGenericDataStruct(iparser); break;
				case BinBlockID.Quicksplines: this.UnfoldGenericDataStruct(iparser); break;
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
					case BinBlockID.TextureAnimPack: this.UnfoldTextureAnimPack(iparser, sectionNumber, textureIndex - 1); break;
					case BinBlockID.TopologyTree: this.UnfoldTopologyTree(iparser); break;
					case BinBlockID.ScenerySection: this.UnfoldScenerySection(iparser); break;
					case BinBlockID.LightSourcesPack: this.UnfoldLightSourcePack(iparser); break;
					case BinBlockID.LightFlaresPack: this.UnfoldLightFlarePack(iparser); break;
					case BinBlockID.EventTriggerPack: this.UnfoldEventTriggerPack(iparser); break;
					case BinBlockID.SmokeableSpawners: this.UnfoldSmokeableSpawners(iparser); break;
					case BinBlockID.AcidEffects: this.UnfoldAcidEffects(iparser); break;
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
		private void UnfoldTopologyBarrierGroups(IParser iparser)
		{
			var parser = iparser as Parsers.TopologyBarrierGroupParser;
			this.m_topologyBarrierGroups = parser.BarrierGroups;
		}
		private void UnfoldAcidEmitters(IParser iparser)
		{
			var parser = iparser as Parsers.AcidEmitterParser;

			var acidEmitters = BinBlockID.AcidEmitters.ToString();
			var path = Path.Combine(this.m_pathLocation, acidEmitters);
			var main = Path.Combine(path, Namings.Assets + Namings.HyperExt);

			Directory.CreateDirectory(path);
			var files = new List<string>();

			foreach (var emitter in parser.AcidEmitters)
			{
				var name = emitter.Name + Namings.HyperExt;
				var file = Path.Combine(path, name);

				var json = JsonConvert.SerializeObject(emitter, Formatting.Indented);
				File.WriteAllText(file, json);

				files.Add(name);
			}

			File.WriteAllLines(main, files);
			this.m_header.Extras.Add(acidEmitters);
		}
		private void UnfoldSmokeableInfos(IParser iparser)
		{
			var parser = iparser as Parsers.SmokeableInfoParser;

			var smokeableInfos = BinBlockID.SmokeableInfos.ToString();
			var path = Path.Combine(this.m_pathLocation, smokeableInfos);
			var main = Path.Combine(path, Namings.Assets + Namings.HyperExt);

			Directory.CreateDirectory(path);
			var files = new List<string>();

			foreach (var smokeable in parser.SmokeableInfos)
			{
				var name = smokeable.Name + Namings.HyperExt;
				var file = Path.Combine(path, name);

				var json = JsonConvert.SerializeObject(smokeable, Formatting.Indented);
				File.WriteAllText(file, json);

				files.Add(name);
			}

			File.WriteAllLines(main, files);
			this.m_header.Extras.Add(smokeableInfos);
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
		private void UnfoldTextureAnimPack(IParser iparser, ushort sectionNumber, int index)
		{
			if (index < 0) return; // bruhmento

			var parser = iparser as Parsers.TextureAnimPackParser;

			var name = Namings.TexturesPack + index.ToString() + Namings.BinExt;
			var path = Path.Combine(this.m_pathStream, sectionNumber.ToString(), name);

			using (var bw = new BinaryWriter(File.Open(path, FileMode.Append, FileAccess.Write)))
			{
				parser.Container.Save(bw);
			}
		}
		private void UnfoldTopologyTree(IParser iparser)
		{
			var parser = iparser as Parsers.TopologyTreeParser;
			this.m_topologies.Add(parser.Topology);
		}
		private void UnfoldScenerySection(IParser iparser)
		{
			var parser = iparser as Parsers.ScenerySectionParser;
			var scenery = parser.Scenery;

			if (!this.m_userInfos.TryGetValue(scenery.SectionNumber, out var userInfo))
			{
				userInfo = this.CreateNewUserInfo(Game.Underground2, scenery.SectionNumber);
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
				userInfo = this.CreateNewUserInfo(Game.Underground2, parser.SectionNumber);
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
				userInfo = this.CreateNewUserInfo(Game.Underground2, parser.SectionNumber);
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
				userInfo = this.CreateNewUserInfo(Game.Underground2, parser.SectionNumber);
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
		private void UnfoldAcidEffects(IParser iparser)
		{
			var parser = iparser as Parsers.AcidEffectParser;

			if (!this.m_userInfos.TryGetValue(parser.SectionNumber, out var userInfo))
			{
				userInfo = this.CreateNewUserInfo(Game.Underground2, parser.SectionNumber);
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
					var overrideInfo = this.m_sceneryOverrides[smokeable.SceneryOverrideIndex];

					if (this.m_userInfos.TryGetValue(overrideInfo.SectionNumber, out var userInfo))
					{
						var instance = userInfo.SceneryObjects[overrideInfo.InstanceIndex] as SceneryObject;
						var transform = MathExtensions.TransformToPlain(smokeable.Transform);

						instance.IsSmackable = true;
						instance.ReplacementMesh = smokeable.SolidMeshKey.BinString();
						instance.SmokeableEffect = smokeable.SmokeableInfoKey.BinString();
						instance.SpawnerFlags = overrideInfo.SecondaryFlags;
						instance.SpawnerPosition = transform.Position;
						instance.SpawnerScale = transform.Scale;
						instance.SpawnerRotation = transform.Rotation;
					}
				}
			}
		}
		private void FixupTopologyTrees()
		{
			bool areTopologyBarriersNulled = this.m_topologyBarrierGroups is null;

			var topologyNames = new List<string>(this.m_topologyBarrierGroups.Select((group) => group.Name));

			foreach (var tree in this.m_topologies)
			{
				if (!areTopologyBarriersNulled)
				{
					tree.FromGroupNames(topologyNames);
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
	}
}
