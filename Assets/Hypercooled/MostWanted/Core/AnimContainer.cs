using CoreExtensions.IO;
using Hypercooled.Shared.Core;
using Hypercooled.Shared.Structures;
using Hypercooled.MostWanted.Textures;
using Hypercooled.Utils;
using System;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Hypercooled.MostWanted.Core
{
	public class AnimContainer
	{
		public ConcurrentDictionary<uint, AnimObject> AnimationMap { get; }

		public AnimContainer()
		{
			this.AnimationMap = new ConcurrentDictionary<uint, AnimObject>();
		}
		public AnimContainer(ConcurrentDictionary<uint, AnimObject> animationMap)
		{
			this.AnimationMap = animationMap;
		}

		public void MergeWithTextureContainer(TextureContainer container)
		{
			foreach (var animObject in this.AnimationMap)
			{
				container.AnimationMap.TryAdd(animObject.Key, animObject.Value);
			}
		}
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

		public void Load(BinaryReader br)
		{
			var offset = br.BaseStream.Position;
			var info = br.ReadUnmanaged<BinBlock.Info>();

			this.InternalLoaderAnimations(br, new BinBlock(info, offset));
		}
		public void Save(BinaryWriter bw)
		{
			// Precalculate size
			int size = 0x20;
			int adds = 0x00;
			size += TextureAnimPackHeader.SizeOf;
			size += TextureAnimEntry.SizeOf * this.AnimationMap.Count;

			foreach (var animObject in this.AnimationMap.Values)
			{
				adds += TextureAnimFrame.SizeOf * animObject.Frames.Count;
			}

			size += adds;

			bw.Write((uint)BinBlockID.TextureAnimPack);
			bw.Write(size);

			this.InternalSaverAnimations(bw, adds);
		}

		private void InternalLoaderAnimations(BinaryReader br, BinBlock block)
		{
			br.BaseStream.Position = block.Start;
			List<uint> keys = null;

			while (br.BaseStream.Position < block.End)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;

				if (id == BinBlockID.TextureAnimEntry)
				{
					keys = this.ReadTextureAnimEntries(br, size);
				}
				else if (id == BinBlockID.TextureAnimFrames)
				{
					this.ReadTextureAnimFrames(br, size, keys);
				}

				br.BaseStream.Position = offset + size;
			}
		}
		private void InternalSaverAnimations(BinaryWriter bw, int additional)
		{
			var keys = this.AnimationMap.Keys.ToArray();
			Array.Sort(keys);

			this.WriteTextureAnimPackHeader(bw);
			this.WriteTextureAnimEntries(bw, keys);
			this.WriteTextureAnimFrames(bw, keys, additional);
		}
		private List<uint> ReadTextureAnimEntries(BinaryReader br, int size)
		{
			var count = size / TextureAnimEntry.SizeOf;
			var list = new List<uint>(count);

			for (int i = 0; i < count; ++i)
			{
				var entry = br.ReadStruct<TextureAnimEntry>();

				this.AnimationMap.TryAdd(entry.Key, new AnimObject()
				{
					CollectionName = Hashing.ResolveBinEqual(entry.Key, entry.Name),
					Key = entry.Key,
					FramesPerSecond = entry.FramePerSecond,
					NumberOfFrames = entry.NumFrames,
					TimeBase = entry.TimeBase,
				});

				list.Add(entry.Key);
			}

			return list;
		}
		private void ReadTextureAnimFrames(BinaryReader br, int size, List<uint> keys)
		{
			if (keys is null) return;

			int count = size / TextureAnimFrame.SizeOf;
			int index = 0;

			foreach (var key in keys)
			{
				var entry = this.AnimationMap[key];

				for (int i = 0; i < entry.NumberOfFrames && index < count; ++i, ++index)
				{
					var frame = br.ReadUnmanaged<TextureAnimFrame>();
					entry.Frames.Add(frame.Key.BinString());
				}
			}
		}
		private void WriteTextureAnimPackHeader(BinaryWriter bw)
		{
			var header = new TextureAnimPackHeader()
			{
				IsValid = 1,
			};

			bw.Write((uint)BinBlockID.TextureAnimPackHeader);
			bw.Write(TextureAnimPackHeader.SizeOf);
			bw.WriteUnmanaged(header);
		}
		private void WriteTextureAnimEntries(BinaryWriter bw, uint[] keys)
		{
			bw.Write((uint)BinBlockID.TextureAnimEntry);
			bw.Write(TextureAnimEntry.SizeOf * keys.Length);

			foreach (var key in keys)
			{
				var animobject = this.AnimationMap[key];

				var entry = new TextureAnimEntry()
				{
					Key = animobject.Key,
					Name = animobject.CollectionName,
					FramePerSecond = animobject.FramesPerSecond,
					NumFrames = animobject.NumberOfFrames,
					TimeBase = animobject.TimeBase,
				};

				bw.WriteStruct(entry);
			}
		}
		private void WriteTextureAnimFrames(BinaryWriter bw, uint[] keys, int additional)
		{
			bw.Write((uint)BinBlockID.TextureAnimFrames);
			bw.Write(additional);

			foreach (var key in keys)
			{
				var animObject = this.AnimationMap[key];

				foreach (var frame in animObject.Frames)
				{
					var write = new TextureAnimFrame()
					{
						Key = frame.BinHash(),
					};

					bw.WriteUnmanaged(write);
				}
			}
		}
	}
}
