using CoreExtensions.IO;
using Hypercooled.Shared.Core;
using Hypercooled.Shared.Structures;
using Hypercooled.Carbon.Textures;
using Hypercooled.Utils;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Hypercooled.Carbon.Core
{
	public class TextureContainer : Shared.Core.TextureContainer
	{
		public override uint[] GetAllKeys(BinaryReader br)
		{
			var offset = br.BaseStream.Position;
			var info = br.ReadUnmanaged<BinBlock.Info>();

			return this.InternalGetAllKeys(br, new BinBlock(info, offset));
		}
		public override void Load(BinaryReader br)
		{
			var offset = br.BaseStream.Position;
			var info = br.ReadUnmanaged<BinBlock.Info>();

			this.LoaderTexturePack(br, new BinBlock(info, offset));
		}
		public override void Save(BinaryWriter bw)
		{
			bw.Write((uint)BinBlockID.TexturePack);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			this.SaverTexturePack(bw);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		public override void Anim(BinaryWriter bw)
		{
			// empty
		}

		private uint[] InternalGetAllKeys(BinaryReader br, BinBlock main)
		{
			var offsets = this.FindOffsets(br, main);
			if (offsets[1] == -1) return null;

			br.BaseStream.Position = offsets[1];
			var size = br.ReadInt32() >> 3;
			var result = new uint[size];

			for (int i = 0; i < size; ++i)
			{
				result[i] = br.ReadUInt32();
				br.BaseStream.Position += 4;
			}

			return result;
		}
		private void LoaderTexturePack(BinaryReader br, BinBlock main)
		{
			var offsets = this.FindOffsets(br, main);

			if (offsets[2] != -1)
			{
				Debug.LogWarning("Streaming texture packs are not allowed in stream files!");
				return;
			}

			this.ReadHeaderInfo(br, offsets[0]);
			int count = this.ReadTextureCount(br, offsets[1]);

			this.ReadTextureInfos(br, count, offsets[3], offsets[4]);
			this.ReadTextureData(br, offsets[6]);

			this.ReadTextureAnims(br, offsets[8]);
		}
		private long[] FindOffsets(BinaryReader br, BinBlock main)
		{
			var offsets = new long[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
			br.BaseStream.Position = main.Start;

			while (br.BaseStream.Position < main.End)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;

				if (id == BinBlockID.TexturePackInfo || id == BinBlockID.TexturePackData)
				{
					while (br.BaseStream.Position < offset + size)
					{
						var subid = (BinBlockID)br.ReadUInt32();
						var suboffset = br.BaseStream.Position;
						var subsize = br.ReadInt32();

						switch (subid)
						{
							case BinBlockID.TexturePackInfoHeader: offsets[0] = suboffset; break;
							case BinBlockID.TexturePackInfoKeys: offsets[1] = suboffset; break;
							case BinBlockID.TexturePackInfoEntries: offsets[2] = suboffset; break;
							case BinBlockID.TexturePackInfoTextures: offsets[3] = suboffset; break;
							case BinBlockID.TexturePackInfoComps: offsets[4] = suboffset; break;
							case BinBlockID.TexturePackDataHeader: offsets[5] = suboffset; break;
							case BinBlockID.TexturePackDataArray: offsets[6] = suboffset; break;
							case BinBlockID.TexturePackDataUnknown: offsets[7] = suboffset; break;
							case BinBlockID.TexturePackBinary: offsets[8] = suboffset; break;
							default: break;
						}

						br.BaseStream.Position = suboffset + subsize + 4;
					}
				}

				br.BaseStream.Position = offset + size;
			}

			return offsets;
		}
		private void ReadHeaderInfo(BinaryReader br, long dataOffset)
		{
			if (dataOffset == -1) return;

			br.BaseStream.Position = dataOffset;
			if (br.ReadInt32() != 0x7C) return;

			this.Header = br.ReadStruct<Shared.Textures.TexturePackHeader>();

			if (this.Header.Version != Shared.Textures.TexturePackVersion.Carbon)
			{
				Debug.LogWarning($"Texture pack has an invalid version! Got {this.Header.Version}, needs {(int)Shared.Textures.TexturePackVersion.Carbon}");
			}
		}
		private int ReadTextureCount(BinaryReader br, long dataOffset)
		{
			if (dataOffset == -1) return 0;
			br.BaseStream.Position = dataOffset;
			return br.ReadInt32() >> 3;
		}
		private void ReadTextureInfos(BinaryReader br, int textureCount, long texOffset, long compOffset)
		{
			if (texOffset == -1 || compOffset == -1) return;

			var lastTexOffset = texOffset + 4;
			var lastCompOffset = compOffset + 4;

			for (int i = 0; i < textureCount; ++i)
			{
				var texture = new TextureObject();

				br.BaseStream.Position = lastTexOffset;
				texture.Info = br.ReadStruct<TextureInfo>();

				var name = br.ReadNullTermUTF8(br.ReadByte());
				texture.CollectionName = Hashing.ResolveBinEqual(texture.Key, name);
				lastTexOffset = br.BaseStream.Position;

				br.BaseStream.Position = lastCompOffset;
				texture.CompSlot = br.ReadUnmanaged<CompressionSlot>();
				lastCompOffset = br.BaseStream.Position;

				this.TextureMap.TryAdd(texture.Key, texture);
			}
		}
		private void ReadTextureData(BinaryReader br, long dataOffset)
		{
			if (dataOffset == -1) return;

			br.BaseStream.Position = dataOffset;
			br.AlignReaderPow2(0x80);
			dataOffset = br.BaseStream.Position;

			foreach (TextureObject texture in this.TextureMap.Values)
			{
				var info = texture.Info;
				var texData = new byte[info.Size + info.PaletteSize];

				if (info.PaletteSize > 0)
				{
					br.BaseStream.Position = dataOffset + info.PaletteOffset;
					var array = br.ReadBytes(info.PaletteSize);
					Array.Copy(array, 0, texData, 0, array.Length);
				}
				if (info.Size > 0)
				{
					br.BaseStream.Position = dataOffset + info.Offset;
					var array = br.ReadBytes(info.Size);
					Array.Copy(array, 0, texData, info.PaletteSize, array.Length);
				}

				texture.Data = texData;
			}
		}
		private void ReadTextureAnims(BinaryReader br, long dataOffset)
		{
			if (dataOffset == -1) return;

			br.BaseStream.Position = dataOffset;
			int size = br.ReadInt32();
			var offset = br.BaseStream.Position;

			while (br.BaseStream.Position < offset + size)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var len = br.ReadInt32();
				var off = br.BaseStream.Position;

				if (id == BinBlockID.TexturePackAnim)
				{
					while (br.BaseStream.Position < off + len)
					{
						var subid = (BinBlockID)br.ReadUInt32();
						var subsize = br.ReadInt32();
						var suboffset = br.BaseStream.Position;

						var animation = new AnimObject();

						if (subid == BinBlockID.TexturePackAnimNames)
						{
							var entry = br.ReadStruct<TextureAnimEntry>();
							animation.CollectionName = Hashing.ResolveBinEqual(entry.Key, entry.Name);
							animation.Key = entry.Key;
							animation.FramesPerSecond = entry.FramesPerSecond;
							animation.NumberOfFrames = entry.NumFrames;
							animation.TimeBase = entry.TimeBase;
						}
						if (subid == BinBlockID.TexturePackAnimFrames)
						{
							for (int i = 0; i < animation.NumberOfFrames; ++i)
							{
								var frame = br.ReadUnmanaged<TextureAnimFrame>();
								animation.Frames.Add(frame.Key.BinString());
							}
						}

						this.AnimationMap.TryAdd(animation.Key, animation);
						br.BaseStream.Position = suboffset + subsize;
					}
				}

				br.BaseStream.Position = off + len;
			}
		}

		private void SaverTexturePack(BinaryWriter bw)
		{
			bw.Write(0);
			bw.Write(0x30);
			bw.WriteBytes(0, 0x30);

			var array = this.TextureMap.Keys.ToArray();
			Array.Sort(array);

			this.InternalSaverTexturePackInfo(bw, array);
			this.InternalSaverTexturePackData(bw, array);
		}
		private void InternalSaverTexturePackInfo(BinaryWriter bw, uint[] keys)
		{
			bw.Write((uint)BinBlockID.TexturePackInfo);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			this.WriteHeaderInfo(bw);
			this.WriteTextureKeys(bw, keys);
			this.WriteTextureInfos(bw, keys);
			this.WriteTextureComps(bw, keys);
			this.WriteTextureAnims(bw);
			bw.GeneratePaddingPow2(0x80);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void InternalSaverTexturePackData(BinaryWriter bw, uint[] keys)
		{
			bw.Write((uint)BinBlockID.TexturePackData);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			this.WriteBufferInfo(bw);
			this.WriteTextureDatas(bw, keys);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteHeaderInfo(BinaryWriter bw)
		{
			bw.Write((uint)BinBlockID.TexturePackInfoHeader);
			bw.Write(Shared.Textures.TexturePackHeader.SizeOf);

			bw.WriteStruct(this.Header);
		}
		private void WriteTextureKeys(BinaryWriter bw, uint[] keys)
		{
			bw.Write((uint)BinBlockID.TexturePackInfoKeys);
			bw.Write(this.TextureMap.Count << 3);

			foreach (var key in keys) bw.Write((ulong)key);
		}
		private void WriteTextureInfos(BinaryWriter bw, uint[] keys)
		{
			bw.Write((uint)BinBlockID.TexturePackInfoTextures);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			uint length = 0;

			foreach (var key in keys)
			{
				var texture = this.TextureMap[key] as TextureObject;
				var info = texture.Info;

				info.PaletteOffset = length;
				info.Offset = length + (uint)info.PaletteSize;

				texture.Info = info;
				this.WriteTextureInfo(bw, texture);

				length += (uint)info.PaletteSize;
				length += (uint)info.Size;

				var pad = 0x80 - (length & 0x7F);
				if (pad != 0x80) length += pad;
			}

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteTextureComps(BinaryWriter bw, uint[] keys)
		{
			bw.Write((uint)BinBlockID.TexturePackInfoComps);
			bw.Write(keys.Length * CompressionSlot.SizeOf);

			foreach (var key in keys)
			{
				var texture = this.TextureMap[key] as TextureObject;
				bw.WriteUnmanaged(texture.CompSlot);
			}
		}
		private void WriteBufferInfo(BinaryWriter bw)
		{
			bw.Write((uint)BinBlockID.TexturePackDataHeader);
			bw.Write(Shared.Textures.TexturePackBuffer.SizeOf);

			bw.WriteUnmanaged(new Shared.Textures.TexturePackBuffer()
			{
				IsBuffer = 1,
				Key = this.Header.Key,
			});

			bw.Write(0);
			bw.Write(0x50);
			bw.WriteNullTermUTF8(Shared.Core.Namings.Watermark, 0x20);
			bw.WriteBytes(0, 0x30);
		}
		private void WriteTextureDatas(BinaryWriter bw, uint[] keys)
		{
			bw.Write((uint)BinBlockID.TexturePackDataArray);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.AlignWriterPow2(0x80);

			foreach (var key in keys)
			{
				var texture = this.TextureMap[key];
				bw.Write(texture.Data);
				bw.FillBufferPow2(0x80);
			}

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteTextureAnims(BinaryWriter bw)
		{
			if (this.AnimationMap.Count == 0) return;

			bw.Write((uint)BinBlockID.TexturePackBinary);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			var keys = this.AnimationMap.Keys.ToArray();
			Array.Sort(keys);

			foreach (var key in keys)
			{
				var animation = this.AnimationMap[key];

				var entrySize = TextureAnimEntry.SizeOf;
				var frameSize = animation.Frames.Count * TextureAnimFrame.SizeOf;
				var totalSize = 0x10 + entrySize + frameSize;

				bw.Write((uint)BinBlockID.TexturePackAnim);
				bw.Write(totalSize);

				bw.Write((uint)BinBlockID.TexturePackAnimNames);
				bw.Write(entrySize);

				var entry = new TextureAnimEntry()
				{
					Name = animation.CollectionName,
					Key = animation.Key,
					FramesPerSecond = (byte)animation.FramesPerSecond,
					NumFrames = (byte)animation.NumberOfFrames,
					TimeBase = (byte)animation.TimeBase,
				};

				bw.WriteStruct(entry);

				bw.Write((uint)BinBlockID.TexturePackAnimFrames);
				bw.Write(frameSize);

				for (int i = 0; i < animation.Frames.Count; ++i)
				{
					bw.Write(animation.Frames[i].BinHash());
					bw.Write((long)0);
				}
			}

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteTextureInfo(BinaryWriter bw, TextureObject texture)
		{
			const int maxNameSize = 0x22;
			const byte minTexSize = 0x59;

			int a1 = (texture.CollectionName.Length > maxNameSize)
				? maxNameSize
				: texture.CollectionName.Length;

			int a2 = 0x5D + a1 - ((1 + a1) & 0x03); // size of the texture header

			bw.WriteUnmanaged(texture.Info);
			bw.Write((byte)(a2 - minTexSize));

			bw.WriteNullTermUTF8(texture.CollectionName, a1 + 1);
			bw.FillBufferPow2(0x04);
		}
	}
}
