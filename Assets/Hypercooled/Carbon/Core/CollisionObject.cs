using CoreExtensions.IO;
using Hypercooled.Shared.Solids;
using Hypercooled.Utils;
using System.IO;

namespace Hypercooled.Carbon.Core
{
	public class CollisionObject : Shared.Core.CollisionObject
	{
		private string m_collectionName;
		private uint m_key;
		private CVBound[] m_bounds;
		private CVCloud[] m_clouds;

		public override string CollectionName
		{
			get
			{
				return this.m_collectionName;
			}
			set
			{
				this.m_collectionName = value;
				this.m_key = value.BinHash();
			}
		}
		public override uint Key => this.m_key;
		public bool IsResolved { get; set; }

		public CVBound[] Bounds
		{
			get => this.m_bounds;
			set => this.m_bounds = value ?? new CVBound[0];
		}

		public CVCloud[] Clouds
		{
			get
			{
				return this.m_clouds;
			}
			set
			{
				if (value is null)
				{
					this.m_clouds = new CVCloud[0];
				}
				else
				{
					this.m_clouds = value;

					for (int i = 0; i < value.Length; ++i)
					{
						if (value[i] is null) value[i] = new CVCloud();
					}
				}
			}
		}

		public CollisionObject()
		{
			this.m_bounds = new CVBound[0];
			this.m_clouds = new CVCloud[0];
		}

		public override void Serialize(BinaryWriter bw)
		{
			bw.Write((uint)BinBlockID.CollisionBody);
			bw.Write(-1);

			var start = bw.BaseStream.Position;
			bw.AlignWriterPow2(0x10);

			bw.Write(this.m_key);
			bw.Write(this.m_bounds.Length);
			bw.Write(this.IsResolved ? 1 : 0);
			bw.Write(0);

			for (int i = 0; i < this.m_bounds.Length; ++i)
			{
				bw.WriteUnmanaged(this.m_bounds[i]);
			}

			bw.Write(this.m_clouds.Length);
			bw.WriteBytes(0, 0x0C);

			for (int i = 0; i < this.m_clouds.Length; ++i)
			{
				this.m_clouds[i].Write(bw);
			}

			var end = bw.BaseStream.Position;
			bw.BaseStream.Position = start - 4;
			bw.Write((int)(end - start));
			bw.BaseStream.Position = end;
		}

		public override void Deserialize(BinaryReader br)
		{
			var id = (BinBlockID)br.ReadUInt32();
			var size = br.ReadInt32();

			if (id != BinBlockID.CollisionBody || size == 0) return;

			br.AlignReaderPow2(0x10);
			var header = br.ReadUnmanaged<CVHeader>();

			this.m_key = header.Key;
			this.m_collectionName = this.m_key.VltString();
			this.IsResolved = header.IsResolved != 0;

			this.m_bounds = new CVBound[header.NumBounds];

			for (int i = 0; i < this.m_bounds.Length; ++i)
			{
				this.m_bounds[i] = br.ReadUnmanaged<CVBound>();
			}

			this.m_clouds = new CVCloud[br.ReadInt32()];
			br.BaseStream.Position += 0x0C;

			for (int i = 0; i < this.m_clouds.Length; ++i)
			{
				var cloud = new CVCloud();
				cloud.Read(br);
				this.m_clouds[i] = cloud;
			}
		}
	}
}
