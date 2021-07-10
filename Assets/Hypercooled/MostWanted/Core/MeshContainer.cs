using CoreExtensions.IO;
using Hypercooled.MostWanted.Solids;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using SubMeshInfo = Hypercooled.Shared.Core.MeshObject.SubMeshInfo;

namespace Hypercooled.MostWanted.Core
{
	public class MeshContainer : Shared.Core.MeshContainer
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

			this.LoaderSolidPack(br, new BinBlock(info, offset));
		}
		public override void Save(BinaryWriter bw)
		{
			bw.Write((uint)BinBlockID.GeometryPack);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.Write((long)0);
			this.SaverSolidPack(bw);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}

		private uint[] InternalGetAllKeys(BinaryReader br, BinBlock main)
		{
			var headerBlock = this.FindSolidPackHeader(br, main);
			if (headerBlock.Offset == -1) return null;

			br.BaseStream.Position = headerBlock.Start;

			while (br.BaseStream.Position < headerBlock.End)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;

				if (id == BinBlockID.MeshContainerKeys)
				{
					var result = new uint[size >> 3];

					for (int i = 0; i < result.Length; ++i)
					{
						result[i] = br.ReadUInt32();
						br.BaseStream.Position += 4;
					}

					return result;
				}

				br.BaseStream.Position = offset + size;
			}

			return null;
		}
		private void LoaderSolidPack(BinaryReader br, BinBlock main)
		{
			var headerBlock = BinBlock.Null;
			var offsetBlock = BinBlock.Null;

			headerBlock = this.FindSolidPackHeader(br, main);
			if (headerBlock.Offset == -1) return;

			br.BaseStream.Position = headerBlock.Start;
			
			while (br.BaseStream.Position < headerBlock.End)
			{
				var offset = br.BaseStream.Position;
				var block = br.ReadUnmanaged<BinBlock.Info>();

				switch ((BinBlockID)block.ID)
				{
					case BinBlockID.MeshContainerHeader:
						this.Header = br.ReadStruct<Shared.Solids.MeshContainerHeader>();
						break;

					case BinBlockID.MeshContainerKeys:
						var count = block.Size >> 3;
						if (count != this.Header.NumberOfMeshes) Debug.LogWarning($"MeshContainer has inconsistent amount of solids!");
						break;

					case BinBlockID.MeshContainerOffsets:
						offsetBlock = new BinBlock(block, offset);
						break;

					case BinBlockID.MeshContainerEmpty:
						if (block.Size != 0) Debug.LogWarning($"MeshContainerEmpty is not empty!?");
						break;
				}

				br.BaseStream.Position = offset + block.Size + 8;
			}

			if (offsetBlock.Offset == -1)
			{
				this.InternalLoaderRawSolids(br, main);
			}
			else
			{
				this.InternalLoaderStreamingSolids(br, offsetBlock);
			}

			this.InternalFixupSubMeshInfos();
		}
		private BinBlock FindSolidPackHeader(BinaryReader br, BinBlock main)
		{
			br.BaseStream.Position = main.Start;

			while (br.BaseStream.Position < main.End)
			{
				var offset = br.BaseStream.Position;
				var block = br.ReadUnmanaged<BinBlock.Info>();

				if ((BinBlockID)block.ID == BinBlockID.MeshContainerInfo)
				{
					return new BinBlock(block, offset);
				}

				br.BaseStream.Position += block.Size;
			}

			return BinBlock.Null;
		}
		private void InternalLoaderRawSolids(BinaryReader br, BinBlock main)
		{
			br.BaseStream.Position = main.Start;

			while (br.BaseStream.Position < main.End)
			{
				var block = br.ReadUnmanaged<BinBlock.Info>();
				var offset = br.BaseStream.Position;

				if ((BinBlockID)block.ID == BinBlockID.SolidPack)
				{
					var mesh = new MeshObject();

					while (br.BaseStream.Position < offset + block.Size)
					{
						this.ReadSingleMeshPartition(br, mesh);
					}

					this.MeshMap.TryAdd(mesh.Key, mesh);
				}

				br.BaseStream.Position = offset + block.Size;
			}
		}
		private void InternalLoaderStreamingSolids(BinaryReader br, BinBlock offsets)
		{
			for (int i = 0; i < offsets.Size / OffsetEntry.SizeOf; ++i)
			{
				br.BaseStream.Position = offsets.Start + i * OffsetEntry.SizeOf;
				var entry = br.ReadUnmanaged<OffsetEntry>();

				if (entry.CompressionFlags != 0)
				{
					Debug.LogWarning($"Warning! Mesh data at address is 0x{entry.AbsoluteOffset:X8} is compressed!");
					continue;
				}

				br.BaseStream.Position = entry.AbsoluteOffset;
				var block = br.ReadUnmanaged<BinBlock.Info>();
				var offset = br.BaseStream.Position;

				if ((BinBlockID)block.ID == BinBlockID.SolidPack)
				{
					var mesh = new MeshObject();

					while (br.BaseStream.Position < offset + block.Size)
					{
						this.ReadSingleMeshPartition(br, mesh);
					}

					this.MeshMap.TryAdd(mesh.Key, mesh);
				}
			}
		}
		private void ReadSingleMeshPartition(BinaryReader br, MeshObject mesh)
		{
			var id = (BinBlockID)br.ReadUInt32();
			var size = br.ReadInt32();
			var offset = br.BaseStream.Position;

			switch (id)
			{
				case BinBlockID.Empty: break;
				case BinBlockID.SolidInfo: this.ReadSolidInfo(br, size, mesh); break;
				case BinBlockID.SolidTextures: this.ReadSolidTextures(br, size, mesh); break;
				case BinBlockID.SolidShaders: this.ReadSolidMaterials(br, size, mesh); break;
				case BinBlockID.MeshNormalSmoother: break;
				case BinBlockID.MeshSmoothVertices: break;
				case BinBlockID.MeshSmoothVertexPlats: break;
				case BinBlockID.SolidMarkers: this.ReadSolidMarkers(br, size, mesh); break;
				case BinBlockID.MeshInfoContainer: this.ReadMeshInfoContainer(br, size, mesh); break;
				default: Debug.LogWarning($"Unknown block 0x{(int)id:X8} at offset 0x{offset - 8:X8}"); break;
			}

			br.BaseStream.Position = offset + size;
		}
		private void ReadSolidInfo(BinaryReader br, int size, MeshObject mesh)
		{
			br.AlignReaderPow2(0x10);
			var info = br.ReadUnmanaged<SolidInfo>();
			var name = br.ReadNullTermUTF8(size - SolidInfo.SizeOf);

			mesh.CollectionName = Hashing.ResolveBinEqual(info.Key, name);
			info.AABBMin = new Vector3(info.AABBMin.x, info.AABBMin.z, info.AABBMin.y);
			info.AABBMax = new Vector3(info.AABBMax.x, info.AABBMax.z, info.AABBMax.y);
			mesh.SolidInformation = info;
		}
		private void ReadSolidTextures(BinaryReader br, int size, MeshObject mesh)
		{
			int count = size / Shared.Solids.SolidTexture.SizeOf;
			mesh.Textures = new Shared.Solids.SolidTexture[count];

			for (int i = 0; i < count; ++i)
			{
				mesh.Textures[i] = br.ReadUnmanaged<Shared.Solids.SolidTexture>();
			}
		}
		private void ReadSolidMaterials(BinaryReader br, int size, MeshObject mesh)
		{
			int count = size / Shared.Solids.SolidMaterial.SizeOf;
			mesh.Materials = new Shared.Solids.SolidMaterial[count];

			for (int i = 0; i < count; ++i)
			{
				mesh.Materials[i] = br.ReadUnmanaged<Shared.Solids.SolidMaterial>();
			}
		}
		private void ReadSolidMarkers(BinaryReader br, int size, MeshObject mesh)
		{
			br.AlignReaderPow2(0x10);
			int count = size / Shared.Solids.SolidMarker.SizeOf;
			mesh.Markers = new Shared.Solids.SolidMarker[count];

			for (int i = 0; i < count; ++i)
			{
				mesh.Markers[i] = br.ReadUnmanaged<Shared.Solids.SolidMarker>();
			}
		}
		private void ReadMeshInfoContainer(BinaryReader br, int size, MeshObject mesh)
		{
			var offset = br.BaseStream.Position;

			while (br.BaseStream.Position < offset + size)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var len = br.ReadInt32();
				var off = br.BaseStream.Position;

				switch (id)
				{
					case BinBlockID.Empty: break;
					case BinBlockID.MeshInfoHeader: this.ReadMeshInfoHeader(br, mesh); break;
					case BinBlockID.MeshVertexBuffer: this.ReadVertexBuffer(br, len, mesh); break;
					case BinBlockID.MeshShaderInfos: this.ReadMeshShaderInfos(br, len, mesh); break;
					case BinBlockID.MeshPolygons: this.ReadMeshPolygons(br, len, mesh); break;
					case BinBlockID.MeshVltMaterials: this.ReadVltMaterial(br, len, mesh); break;
					default: Debug.Log($"Unknown block 0x{(int)id:X8} at offset 0x{offset - 8:X8}"); break;
				}

				br.BaseStream.Position = off + len;
			}
		}
		private void ReadMeshInfoHeader(BinaryReader br, MeshObject mesh)
		{
			br.AlignReaderPow2(0x10);
			mesh.MeshInformation = br.ReadUnmanaged<MeshInfoHeader>();
			mesh.Buffer = new float[mesh.MeshInformation.NumVertexBuffers][];
		}
		private void ReadMeshShaderInfos(BinaryReader br, int size, MeshObject mesh)
		{
			var current = br.BaseStream.Position;
			br.AlignReaderPow2(0x10);
			size -= (int)(br.BaseStream.Position - current);

			var count = size / MeshShaderInfo.SizeOf;
			mesh.SubMeshes = new MeshShaderInfo[count];

			for (int i = 0; i < count; ++i)
			{
				var submesh = br.ReadUnmanaged<MeshShaderInfo>();

				submesh.BoundsMin = new Vector3(submesh.BoundsMin.x, submesh.BoundsMin.z, submesh.BoundsMin.y);
				submesh.BoundsMax = new Vector3(submesh.BoundsMax.x, submesh.BoundsMax.z, submesh.BoundsMax.y);

				mesh.SubMeshes[i] = submesh;
			}
		}
		private void ReadMeshPolygons(BinaryReader br, int size, MeshObject mesh)
		{
			var current = br.BaseStream.Position;
			br.AlignReaderPow2(0x10);
			size -= (int)(br.BaseStream.Position - current);

			var count = size >> 1;
			var polygons = new int[count];

			for (int i = 0; i < count; ++i) polygons[i] = br.ReadUInt16();

			for (int i = 0; i < polygons.Length - 1; i += 3)
			{
				int temp = polygons[i + 1];
				polygons[i + 1] = polygons[i + 2];
				polygons[i + 2] = temp;
			}

			mesh.Triangles = polygons;
		}
		private void ReadVertexBuffer(BinaryReader br, int size, MeshObject mesh)
		{
			var current = br.BaseStream.Position;
			br.AlignReader(this.Header.MeshAlignment);
			size -= (int)(br.BaseStream.Position - current);

			var count = size >> 2;
			var buffer = new float[count];

			unsafe
			{
				fixed (float* ptr = buffer)
				{
					for (int i = 0; i < count; ++i)
					{
						ptr[i] = br.ReadSingle();
					}
				}
			}

			var info = mesh.MeshInformation;
			mesh.SetVertexBuffer(info.ChunksLoaded++, buffer);
			mesh.MeshInformation = info;
		}
		private void ReadVltMaterial(BinaryReader br, int size, MeshObject mesh)
		{
			var name = br.ReadNullTermUTF8(size);
			var mats = mesh.VltMaterials;

			var count = mats.Length;
			Array.Resize(ref mats, count + 1);

			mats[count] = name;
		}
		private void InternalFixupSubMeshInfos()
		{
			foreach (MeshObject meshObject in this.MeshMap.Values)
			{
				var subMeshInfos = new SubMeshInfo[meshObject.SubMeshCount];

				for (int i = 0; i < subMeshInfos.Length; ++i)
				{
					var subMesh = meshObject.SubMeshes[i];

					var diffuse = subMesh.TextureDiffuseIndex == Byte.MaxValue
						? UInt32.MaxValue
						: meshObject.Textures[subMesh.TextureDiffuseIndex].Key;

					var normal = subMesh.TextureNormalIndex == Byte.MaxValue
						? UInt32.MaxValue
						: meshObject.Textures[subMesh.TextureNormalIndex].Key;

					var height = subMesh.TextureHeightIndex == Byte.MaxValue
						? UInt32.MaxValue
						: meshObject.Textures[subMesh.TextureHeightIndex].Key;

					var specular = subMesh.TextureSpecularIndex == Byte.MaxValue
						? UInt32.MaxValue
						: meshObject.Textures[subMesh.TextureSpecularIndex].Key;

					var opacity = subMesh.TextureOpacityIndex == Byte.MaxValue
						? UInt32.MaxValue
						: meshObject.Textures[subMesh.TextureOpacityIndex].Key;

					var light = subMesh.ShaderIndex == Byte.MaxValue
						? UInt32.MaxValue
						: meshObject.Materials[subMesh.ShaderIndex].Key;

					var dxShader = (uint)subMesh.DXShader;

					subMeshInfos[i] = new SubMeshInfo(diffuse, normal, height, specular, opacity, light, dxShader);
				}

				meshObject.SubMeshInfos = subMeshInfos;

				var info = meshObject.MeshInformation;
				info.ChunksLoaded = 0;
				meshObject.MeshInformation = info;
			}
		}

		private void SaverSolidPack(BinaryWriter bw)
		{
			var keys = this.MeshMap.Keys.ToArray();
			Array.Sort(keys);

			this.InternalSaverSolidData(bw, keys);
			this.InternalSaverRawSolids(bw, keys);
		}
		private void InternalSaverSolidData(BinaryWriter bw, uint[] keys)
		{
			bw.Write((uint)BinBlockID.MeshContainerInfo);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			this.WriteMeshContainerHeader(bw);
			this.WriteMeshContainerKeys(bw, keys);
			this.WriteMeshContainerEmpty(bw);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void InternalSaverRawSolids(BinaryWriter bw, uint[] keys)
		{
			foreach (var key in keys)
			{
				var mesh = this.MeshMap[key] as MeshObject;
				this.WriteSingleRawSolid(bw, mesh);
			}
		}
		private void WriteMeshContainerHeader(BinaryWriter bw)
		{
			bw.Write((uint)BinBlockID.MeshContainerHeader);
			bw.Write(Shared.Solids.MeshContainerHeader.SizeOf);

			bw.WriteStruct(this.Header);
		}
		private void WriteMeshContainerKeys(BinaryWriter bw, uint[] keys)
		{
			bw.Write((uint)BinBlockID.MeshContainerKeys);
			bw.Write(keys.Length << 3);

			foreach (var key in keys) bw.Write((ulong)key);
		}
		private void WriteMeshContainerEmpty(BinaryWriter bw)
		{
			bw.Write((uint)BinBlockID.MeshContainerEmpty);
			bw.Write(0);
		}
		private void WriteSingleRawSolid(BinaryWriter bw, MeshObject mesh)
		{
			bw.Write((uint)BinBlockID.SolidPack);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			this.WriteSolidInfo(bw, mesh);
			this.WriteSolidTextures(bw, mesh);
			this.WriteSolidMaterials(bw, mesh);
			this.WriteSolidMarkers(bw, mesh);
			this.WriteMeshInfoContainer(bw, mesh);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteSolidInfo(BinaryWriter bw, MeshObject mesh)
		{
			bw.Write((uint)BinBlockID.SolidInfo);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.AlignWriterPow2(0x10);
			bw.WriteUnmanaged(mesh.SolidInformation);
			bw.WriteNullTermUTF8(mesh.CollectionName);
			bw.FillBufferPow2(0x04);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteSolidTextures(BinaryWriter bw, MeshObject mesh)
		{
			if (mesh.Textures.Length == 0) return;

			bw.Write((uint)BinBlockID.SolidTextures);
			bw.Write(Shared.Solids.SolidTexture.SizeOf * mesh.Textures.Length);

			foreach (var texture in mesh.Textures)
			{
				bw.WriteUnmanaged(texture);
			}
		}
		private void WriteSolidMaterials(BinaryWriter bw, MeshObject mesh)
		{
			if (mesh.Materials.Length == 0) return;

			bw.Write((uint)BinBlockID.SolidShaders);
			bw.Write(Shared.Solids.SolidMaterial.SizeOf * mesh.Materials.Length);

			foreach (var material in mesh.Materials)
			{
				bw.WriteUnmanaged(material);
			}
		}
		private void WriteSolidMarkers(BinaryWriter bw, MeshObject mesh)
		{
			if (mesh.Markers.Length == 0) return;

			bw.Write((uint)BinBlockID.SolidMarkers);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.AlignWriterPow2(0x10);

			foreach (var marker in mesh.Markers)
			{
				bw.WriteUnmanaged(marker);
			}

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.WriteUnmanaged((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteMeshInfoContainer(BinaryWriter bw, MeshObject mesh)
		{
			bw.Write((uint)BinBlockID.MeshInfoContainer);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			this.WriteMeshInfoHeader(bw, mesh);
			this.WriteMeshShaderInfos(bw, mesh);
			this.WriteMeshPolygons(bw, mesh);
			this.WriteVertexBuffer(bw, mesh);
			this.WriteVltMaterials(bw, mesh);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteMeshInfoHeader(BinaryWriter bw, MeshObject mesh)
		{
			bw.Write((uint)BinBlockID.MeshInfoHeader);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.AlignWriterPow2(0x10);
			bw.WriteUnmanaged(mesh.MeshInformation);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteMeshShaderInfos(BinaryWriter bw, MeshObject mesh)
		{
			bw.Write((uint)BinBlockID.MeshShaderInfos);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.AlignWriterPow2(0x10);

			for (int i = 0; i < mesh.SubMeshes.Length; ++i)
			{
				bw.WriteUnmanaged(mesh.SubMeshes[i]);
			}

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteMeshPolygons(BinaryWriter bw, MeshObject mesh)
		{
			bw.Write((uint)BinBlockID.MeshPolygons);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.AlignWriterPow2(0x10);
			this.InternalSwapPolygonIndices(mesh.Triangles);

			for (int i = 0; i < mesh.Triangles.Length; ++i)
			{
				bw.Write((ushort)mesh.Triangles[i]);
			}

			this.InternalSwapPolygonIndices(mesh.Triangles);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteVertexBuffer(BinaryWriter bw, MeshObject mesh)
		{
			for (int i = 0; i < mesh.MeshInformation.NumVertexBuffers; ++i)
			{
				bw.Write((uint)BinBlockID.MeshVertexBuffer);
				bw.Write(-1);

				var start = bw.BaseStream.Position;

				bw.AlignWriterPow2(this.Header.MeshAlignment);
				var buffer = mesh.GetVertexBuffer(i);

				unsafe
				{
					fixed (float* ptr = buffer)
					{
						for (int k = 0; k < buffer.Length; ++k)
						{
							bw.Write(ptr[k]);
						}
					}
				}

				var end = bw.BaseStream.Position;

				bw.BaseStream.Position = start - 4;
				bw.Write((uint)(end - start));
				bw.BaseStream.Position = end;
			}
		}
		private void WriteVltMaterials(BinaryWriter bw, MeshObject mesh)
		{
			foreach (var vltMaterial in mesh.VltMaterials)
			{
				bw.Write((uint)BinBlockID.MeshVltMaterials);
				bw.Write(-1);

				var start = bw.BaseStream.Position;

				bw.WriteNullTermUTF8(vltMaterial);
				bw.FillBufferPow2(0x04);

				var end = bw.BaseStream.Position;

				bw.BaseStream.Position = start - 4;
				bw.Write((uint)(end - start));
				bw.BaseStream.Position = end;
			}
		}
	}
}
