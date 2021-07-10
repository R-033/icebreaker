using Hypercooled.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Hypercooled.Shared.Core
{
	public abstract class TextureContainer
	{
		public Textures.TexturePackHeader Header { get; set; }
		public ConcurrentDictionary<uint, TextureObject> TextureMap { get; }
		public ConcurrentDictionary<uint, AnimObject> AnimationMap { get; }

		public TextureContainer()
		{
			this.TextureMap = new ConcurrentDictionary<uint, TextureObject>();
			this.AnimationMap = new ConcurrentDictionary<uint, AnimObject>();
		}
		public TextureContainer(ConcurrentDictionary<uint, TextureObject> textureMap)
		{
			this.TextureMap = textureMap;
			this.AnimationMap = new ConcurrentDictionary<uint, AnimObject>();
		}

		public abstract uint[] GetAllKeys(BinaryReader br);
		public abstract void Load(BinaryReader br);
		public abstract void Save(BinaryWriter bw);
		public abstract void Anim(BinaryWriter bw);

		public void MergeAnimationsIntoMap(IDictionary<uint, AnimObject> map)
		{
			if (map is null) return;

			foreach (var pair in this.AnimationMap)
			{
				if (!map.ContainsKey(pair.Key))
				{
					map.Add(pair);
				}
			}
		}

		public void MergeTexturesIntoMap(IDictionary<uint, TextureObject> map)
		{
			if (map is null) return;

			foreach (var pair in this.TextureMap)
			{
				if (!map.ContainsKey(pair.Key))
				{
					map.Add(pair);
				}
			}
		}

		public void SetAnimations(IEnumerable<AnimObject> animObjects)
		{
			foreach (var animObject in animObjects)
			{
				this.AnimationMap.TryAdd(animObject.Key, animObject);
			}
		}

		public void SetContainerName(string name)
		{
			var header = this.Header;
			header.Descriptor = name ?? String.Empty;
			header.PipelinePath = header.Descriptor + ".tpk";
			header.Key = header.PipelinePath.BinHash();
			this.Header = header;
		}

		public static TextureContainer GetTextureContainer(Managed.Game game)
		{
			switch (game)
			{
				case Managed.Game.Underground1: return null;
				case Managed.Game.Underground2: return new Underground2.Core.TextureContainer();
				case Managed.Game.MostWanted: return new MostWanted.Core.TextureContainer();
				case Managed.Game.Carbon: return new Carbon.Core.TextureContainer();
				default: return null;
			}
		}
	}
}
