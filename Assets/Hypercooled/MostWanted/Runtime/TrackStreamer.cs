using Hypercooled.MostWanted.MapStream;
using Hypercooled.MostWanted.Textures;
using Hypercooled.Shared.Core;
using Hypercooled.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Namings = Hypercooled.Shared.Core.Namings;
using CollisionObject = Hypercooled.MostWanted.Core.CollisionObject;
using SceneryObject = Hypercooled.MostWanted.Core.SceneryObject;

namespace Hypercooled.MostWanted.Runtime
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
			var material = new Material(Core.ShaderFactory.GetDXShader(dxShader));

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

			material.SetFloat("cullMode", (float)UnityEngine.Rendering.CullMode.Off);
			material.enableInstancing = true;

			// do some lightMat manipulations with colors
			var light = this.GetMaterialObjectOrNull(subMeshInfo.LightKey); // can be null

			material.mainTexture = diffuseTexture;
			try
			{
				if (normalTexture)
					material.SetTexture("_BumpMap",  normalTexture);
				if (specularTexture)
					material.SetTexture("SPECULARMAP_SAMPLER",  specularTexture);
				if (opacityTexture)
					material.SetTexture("OPACITYMAP_SAMPLER",  opacityTexture);
				//material.SetTexture("MISCMAP1_SAMPLER",  ???);
			} catch
			{
				
			}

			if (diffuseObject != null)
			{
				var textureInfo = ((MostWanted.Core.TextureObject)diffuseObject).Info;

				// textureInfo.ApplyAlphaSorting ?

				material.SetFloat("IsTransparent", (float)textureInfo.AlphaBlendType);
				
				// todo better blending options
				switch (textureInfo.AlphaBlendType)
				{
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_DEFAULT:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Add);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
						material.SetFloat("b_ZWrite", 1f);
						material.SetFloat("Cutout", 0.5f);
						break;
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_SRCCOPY:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Add);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
						material.SetFloat("b_ZWrite", 1f);
						material.SetFloat("Cutout", 0.5f);
						break;
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_BLEND:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Add);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
						material.SetFloat("b_ZWrite", 0f);
						material.SetFloat("Cutout", 0f);
						break;
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_ADDATIVE:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcColor);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcColor);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Add);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
						material.SetFloat("b_ZWrite", 0f);
						material.SetFloat("Cutout", 0f);
						break;
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_SUBTRACTIVE:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Subtract);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
						material.SetFloat("b_ZWrite", 0f);
						material.SetFloat("Cutout", 0f);
						break;
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_OVERBRIGHT:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Add);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
						material.SetFloat("b_ZWrite", 0f);
						material.SetFloat("Cutout", 0f);
						break;
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_DEST_BLEND:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Add);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
						material.SetFloat("b_ZWrite", 0f);
						material.SetFloat("Cutout", 0f);
						break;
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_DEST_ADDATIVE:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Add);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
						material.SetFloat("b_ZWrite", 0f);
						material.SetFloat("Cutout", 0f);
						break;
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_DEST_SUBTRACTIVE:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Subtract);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
						material.SetFloat("b_ZWrite", 0f);
						material.SetFloat("Cutout", 0f);
						break;
					case Shared.Textures.TextureAlphaBlendType.TEXBLEND_DEST_OVERBRIGHT:
						material.SetFloat("SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
						material.SetFloat("DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
						material.SetFloat("BlendOperation", (float)UnityEngine.Rendering.BlendOp.Add);
						material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
						material.SetFloat("b_ZWrite", 0f);
						material.SetFloat("Cutout", 0f);
						break;
				}
			}

			if (light != null)
			{
				material.SetColor("DiffuseMin", light.DiffuseMinColor);
				material.SetColor("DiffuseRange", light.DiffuseMaxColor);
				material.SetFloat("SpecularPower", light.SpecularPower);
				material.SetColor("SpecularMin", light.SpecularMinColor);
				material.SetColor("SpecularRange", light.SpecularMaxColor);
				material.SetFloat("EnvmapPower", light.EnvmapPower);
				material.SetColor("EnvmapMin", new Color(light.EnvmapMinColor.r, light.EnvmapMinColor.g, light.EnvmapMinColor.b, 1f));
				material.SetColor("EnvmapRange", new Color(light.EnvmapMaxColor.r, light.EnvmapMaxColor.g, light.EnvmapMaxColor.b, 1f));
				material.SetFloat("MetallicScale", 0f);
				material.SetFloat("SpecularHotSpot", 0f);
			}

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
