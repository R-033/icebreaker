using Hypercooled.Shared.Core;
using Hypercooled.Underground2.MapStream;
using Hypercooled.Underground2.Textures;
using Hypercooled.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Namings = Hypercooled.Shared.Core.Namings;
using CollisionObject = Hypercooled.Underground2.Core.CollisionObject;
using SceneryObject = Hypercooled.Underground2.Core.SceneryObject;

namespace Hypercooled.Underground2.Runtime
{
	public class TrackStreamer : Shared.Runtime.TrackStreamer
	{
		private readonly List<string> m_topologyBarrierGroups = new List<string>();
		private readonly ConcurrentDictionary<uint, AcidEmitter> m_acidEmitters = new ConcurrentDictionary<uint, AcidEmitter>();
		private readonly ConcurrentDictionary<uint, SmokeableInfo> m_smokeableInfos = new ConcurrentDictionary<uint, SmokeableInfo>();

		private void LoaderCollisionVolumes(string folder)
		{
			var assetMap = Path.Combine(folder, Namings.Assets + Namings.HyperExt);
			if (!File.Exists(assetMap)) return;

			using (var sr = new StreamReader(assetMap))
			{
				while (!sr.EndOfStream)
				{
					var file = sr.ReadLine();
					if (String.IsNullOrWhiteSpace(file)) continue;

					var path = Path.Combine(folder, file);
					if (!File.Exists(path)) continue;

					using (var br = IOHelper.GetBinaryReader(path, false))
					{
						var collision = new CollisionObject();
						collision.Deserialize(br);
						this.GlobalCollisions.TryAdd(collision.Key, collision);
					}
				}
			}
		}
		private void LoaderTopologyBarrierGroups(string file)
		{
			if (!File.Exists(file)) return;

			using (var sr = new StreamReader(file))
			{
				while (!sr.EndOfStream)
				{
					var line = sr.ReadLine();

					if (!String.IsNullOrWhiteSpace(line))
					{
						this.m_topologyBarrierGroups.Add(line);
					}
				}
			}
		}
		private void LoaderAcidEmitters(string folder)
		{
			var assetMap = Path.Combine(folder, Namings.Assets + Namings.HyperExt);
			if (!File.Exists(assetMap)) return;

			using (var sr = new StreamReader(assetMap))
			{
				while (!sr.EndOfStream)
				{
					var file = sr.ReadLine();
					if (String.IsNullOrWhiteSpace(file)) continue;

					var path = Path.Combine(folder, file);
					if (!File.Exists(path)) continue;

					var text = File.ReadAllText(path);
					var emitter = JsonConvert.DeserializeObject<AcidEmitter>(text);

					this.m_acidEmitters.TryAdd(emitter.NameHash, emitter);
				}
			}
		}
		private void LoaderSmokeableInfos(string folder)
		{
			var assetMap = Path.Combine(folder, Namings.Assets + Namings.HyperExt);
			if (!File.Exists(assetMap)) return;

			using (var sr = new StreamReader(assetMap))
			{
				while (!sr.EndOfStream)
				{
					var file = sr.ReadLine();
					if (String.IsNullOrWhiteSpace(file)) continue;

					var path = Path.Combine(folder, file);
					if (!File.Exists(path)) continue;

					var text = File.ReadAllText(path);
					var smokeable = JsonConvert.DeserializeObject<SmokeableInfo>(text);

					this.m_smokeableInfos.TryAdd(smokeable.Key, smokeable);
				}
			}
		}

		protected override void CloseOverride()
		{
			this.m_topologyBarrierGroups.Clear();
			this.m_acidEmitters.Clear();
			this.m_smokeableInfos.Clear();
		}
		protected override Action<string> GetAssetParser(string assetName)
		{
			var name = Path.GetFileNameWithoutExtension(assetName);
			if (!Enum.TryParse(name, out BinBlockID type)) return null;

			switch (type)
			{
				case BinBlockID.VisibleSectionManager: return this.LoaderVisibleSectionManager;
				//case BinBlockID.TrackStreamingBarriers: return this.LoaderTrackStreamingBarriers;
				case BinBlockID.CollisionVolumes: return this.LoaderCollisionVolumes;
				case BinBlockID.TopologyBarrierGroups: return this.LoaderTopologyBarrierGroups;
				case BinBlockID.AcidEmitters: return this.LoaderAcidEmitters;
				case BinBlockID.SmokeableInfos: return this.LoaderSmokeableInfos;
				default: return null;
			}
		}
		protected override Shared.Runtime.TrackSection CreateNewSection(ushort sectionNumber)
		{
			return new TrackSection(sectionNumber, this);
		}
		public override void EnableSceneryGroup(string group)
		{
			
		}
		public override void DisableSceneryGroup(string group)
		{

		}
		public override Material PleaseGiveMeMaterial(MeshObject.SubMeshInfo subMeshInfo)
		{
			var material = new Material(Core.ShaderFactory.DefaultShader); // todo : shader ?

			var textureObject = this.GetTextureObjectOrNull(subMeshInfo.DiffuseKey);
			var texture = textureObject?.GetUnityTexture();

#if DEBUG
			if (textureObject is null)
			{
				// todo : default texture
				Debug.LogWarning($"Cannot find texture 0x{subMeshInfo.DiffuseKey:X8} - {subMeshInfo.DiffuseKey.BinString()}");
			}
#endif

			// avail : do material magic here

			material.SetFloat("_Cull", (float)UnityEngine.Rendering.CullMode.Off);
			material.enableInstancing = true;

			// do some lightMat manipulations with colors
			var light = this.GetMaterialObjectOrNull(subMeshInfo.LightKey); // can be null

			// else we can use some texture properties, like alpha and stuff
			// var alphaBlendType = texture.Info.AlphaBlendType;
			// var alphaUsageType = texture.Info.AlphaUsageType;
			// var applyAlphaSorting = texture.Info.ApplyAlphaSorting;
			// var tileableUV = texture.Info.TileableUV;
			// etc and do some material manipulation

			material.mainTexture = texture;
			return material;
		}
	}
}
