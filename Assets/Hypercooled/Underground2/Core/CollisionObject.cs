using CoreExtensions.IO;
using Hypercooled.Underground2.Solids;
using Hypercooled.Utils;
using System;
using System.IO;

namespace Hypercooled.Underground2.Core
{
	public class CollisionObject : Shared.Core.CollisionObject
	{
		private string m_collectionName;
		private uint m_key;

		private CollisionVertex[] m_vertices;
		private ushort[] m_pointers;

		public override string CollectionName
		{
			get
			{
				return this.m_collectionName;
			}
			set
			{
				this.m_collectionName = value ?? String.Empty;
				this.m_key = value.BinHash();
			}
		}
		public override uint Key => this.m_key;

		public CollisionVertex[] Vertices
		{
			get => this.m_vertices;
			set => this.m_vertices = value ?? new CollisionVertex[0];
		}
		public ushort[] Pointers
		{
			get => this.m_pointers;
			set => this.m_pointers = value ?? new ushort[0];
		}

		public CollisionObject() : base()
		{
			this.m_collectionName = String.Empty;
			this.m_vertices = new CollisionVertex[0];
			this.m_pointers = new ushort[0];
		}

		public override void Serialize(BinaryWriter bw)
		{
			long start = 0;
			long end = 0;

			bw.Write((uint)BinBlockID.CollisionHeader);
			bw.Write(-1);

			start = bw.BaseStream.Position;
			bw.AlignWriterPow2(0x10);

			var header = new CollisionHeader()
			{
				Key = this.m_key,
				Integer1 = 1,
				NumberOfVertices = this.m_vertices.Length,
				NumberOfIndeces = (short)this.m_pointers.Length,
				Name = this.m_collectionName,
			};

			bw.WriteStruct(header);

			end = bw.BaseStream.Position;
			bw.BaseStream.Position = start - 4;
			bw.Write((int)(end - start));
			bw.BaseStream.Position = end;

			bw.Write((uint)BinBlockID.CollisionVertices);
			bw.Write(-1);

			start = bw.BaseStream.Position;
			bw.AlignWriterPow2(0x10);

			for (int i = 0; i < this.m_vertices.Length; ++i)
			{
				bw.WriteUnmanaged(this.m_vertices[i]);
			}

			end = bw.BaseStream.Position;
			bw.BaseStream.Position = start - 4;
			bw.Write((int)(end - start));
			bw.BaseStream.Position = end;

			bw.Write((uint)BinBlockID.CollisionIndices);
			bw.Write(-1);

			start = bw.BaseStream.Position;
			bw.AlignWriterPow2(0x10);

			for (int i = 0; i< this.m_pointers.Length; ++i)
			{
				bw.Write(this.m_pointers[i]);
			}

			end = bw.BaseStream.Position;
			bw.BaseStream.Position = start - 4;
			bw.Write((int)(end - start));
			bw.BaseStream.Position = end;
		}

		public override void Deserialize(BinaryReader br)
		{
			for (int i = 0; i < 3; ++i)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;
				br.AlignReaderPow2(0x10);

				switch (id)
				{
					case BinBlockID.Empty:
						--i; break; // we can skip padding, I guess

					case BinBlockID.CollisionHeader:
						var header = br.ReadStruct<CollisionHeader>();
						this.m_key = header.Key;
						this.m_collectionName = Hashing.ResolveBinEqual(header.Key, header.Name);
						break;

					case BinBlockID.CollisionVertices:
						var sizeV = size - ((int)(br.BaseStream.Position - offset));
						var countV = sizeV / CollisionVertex.SizeOf;
						this.m_vertices = new CollisionVertex[countV];
						for (int k = 0; k < countV; ++k) this.m_vertices[k] = br.ReadUnmanaged<CollisionVertex>();
						break;

					case BinBlockID.CollisionIndices:
						var sizeI = size - ((int)(br.BaseStream.Position - offset));
						var countI = sizeI >> 1;
						this.m_pointers = new ushort[countI];
						for (int k = 0; k < countI; ++k) this.m_pointers[k] = br.ReadUInt16();
						break;

					default:
						break;
				}

				br.BaseStream.Position = offset + size;
			}
		}
	}
}
