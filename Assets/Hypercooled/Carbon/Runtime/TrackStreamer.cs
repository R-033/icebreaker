using Hypercooled.Shared.Core;
using Hypercooled.Carbon.MapStream;
using Hypercooled.Carbon.Textures;
using Hypercooled.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Namings = Hypercooled.Shared.Core.Namings;
using CollisionObject = Hypercooled.Carbon.Core.CollisionObject;
using SceneryObject = Hypercooled.Carbon.Core.SceneryObject;

namespace Hypercooled.Carbon.Runtime
{
	public class TrackStreamer : Shared.Runtime.TrackStreamer
	{
		// todo : location stuff

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

		protected override void CloseOverride()
		{
			// todo
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
			var dxShader = (Solids.DXShaderType)subMeshInfo.DXShaderType;
			var material = new Material(Core.ShaderFactory.GetDXShader(dxShader)); // todo : shader ?

			var diffuseObject = this.GetTextureObjectOrNull(subMeshInfo.DiffuseKey);
			var diffuseTexture = diffuseObject?.GetUnityTexture();

			var normalObject = this.GetTextureObjectOrNull(subMeshInfo.NormalKey);
			var normalTexture = normalObject?.GetUnityTexture();

			var heightObject = this.GetTextureObjectOrNull(subMeshInfo.HeightKey);
			var heightTexture = heightObject?.GetUnityTexture();

			var specularObject = this.GetTextureObjectOrNull(subMeshInfo.SpecularKey);
			var specularTexture = specularObject?.GetUnityTexture();

			var opacityObject = this.GetTextureObjectOrNull(subMeshInfo.OpacityKey);
			var opacityTexture = opacityObject?.GetUnityTexture();

#if DEBUG
			DebugPrintIfTextureIsNull(diffuseObject, subMeshInfo.DiffuseKey, "diffusemap");
			DebugPrintIfTextureIsNull(normalObject, subMeshInfo.NormalKey, "normalmap");
			DebugPrintIfTextureIsNull(heightObject, subMeshInfo.HeightKey, "heightmap");
			DebugPrintIfTextureIsNull(specularObject, subMeshInfo.SpecularKey, "specularmap");
			DebugPrintIfTextureIsNull(opacityObject, subMeshInfo.OpacityKey, "opacitymap");
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

			// avail : normal, height, specular, opacity maps ?

			material.mainTexture = diffuseTexture;
			return material;

#if DEBUG
			void DebugPrintIfTextureIsNull(TextureObject textureObject, uint key, string name)
			{
				if (textureObject is null)
				{
					Debug.LogWarning($"Cannot find {name} texture 0x{key:X8} - {key.BinString()}");
				}
			}
#endif
		}
	}
}
