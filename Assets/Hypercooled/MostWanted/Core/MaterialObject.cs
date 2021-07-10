using CoreExtensions.IO;
using Hypercooled.MostWanted.Solids;
using System.IO;
using UnityEngine;

namespace Hypercooled.MostWanted.Core
{
	public class MaterialObject : Shared.Core.MaterialObject
	{
		public override string CollectionName { get; }
		public override uint Key { get; }
		public LightMaterial Data { get; }

		public override Color DiffuseMinColor
		{
			get
			{
				return new Color()
				{
					a = this.Data.DiffuseMinLevel,
					r = this.Data.DiffuseMinRed,
					g = this.Data.DiffuseMinGreen,
					b = this.Data.DiffuseMinBlue,
				};
			}
		}
		public override Color DiffuseMaxColor
		{
			get
			{
				return new Color()
				{
					a = this.Data.DiffuseMaxLevel,
					r = this.Data.DiffuseMaxRed,
					g = this.Data.DiffuseMaxGreen,
					b = this.Data.DiffuseMaxBlue,
				};
			}
		}
		public override float DiffuseMinAlpha
		{
			get
			{
				return this.Data.DiffuseMinAlpha;
			}
		}
		public override float DiffuseMaxAlpha
		{
			get
			{
				return this.Data.DiffuseMaxAlpha;
			}
		}

		public override float SpecularPower
		{
			get
			{
				return this.Data.SpecularPower;
			}
		}
		public override Color SpecularMinColor
		{
			get
			{
				return new Color()
				{
					a = this.Data.SpecularMinLevel,
					r = this.Data.SpecularMinRed,
					g = this.Data.SpecularMinGreen,
					b = this.Data.SpecularMinBlue,
				};
			}
		}
		public override Color SpecularMaxColor
		{
			get
			{
				return new Color()
				{
					a = this.Data.SpecularMaxLevel,
					r = this.Data.SpecularMaxRed,
					g = this.Data.SpecularMaxGreen,
					b = this.Data.SpecularMaxBlue,
				};
			}
		}

		public override float EnvmapPower
		{
			get
			{
				return this.Data.EnvmapPower;
			}
		}
		public override Color EnvmapMinColor
		{
			get
			{
				return new Color()
				{
					a = this.Data.EnvmapMinLevel,
					r = this.Data.EnvmapMinRed,
					g = this.Data.EnvmapMinGreen,
					b = this.Data.EnvmapMinBlue,
				};
			}
		}
		public override Color EnvmapMaxColor
		{
			get
			{
				return new Color()
				{
					a = this.Data.EnvmapMaxLevel,
					r = this.Data.EnvmapMaxRed,
					g = this.Data.EnvmapMaxGreen,
					b = this.Data.EnvmapMaxBlue,
				};
			}
		}

		public MaterialObject(uint key, string name, LightMaterial data) : base()
		{
			this.Key = key;
			this.CollectionName = name;
			this.Data = data;
		}

		public static MaterialObject Read(BinaryReader br)
		{
			var id = (BinBlockID)br.ReadUInt32();
			var size = br.ReadInt32();

			if (id != BinBlockID.LightMaterials || size != 0xA8) return null;

			br.BaseStream.Position += 0x0C;
			uint key = br.ReadUInt32();
			br.BaseStream.Position += 0x04;

			var name = br.ReadNullTermUTF8(0x1C);
			var data = br.ReadUnmanaged<LightMaterial>();

			return new MaterialObject(key, name, data);
		}
	}
}

