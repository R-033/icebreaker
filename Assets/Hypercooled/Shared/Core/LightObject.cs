using CoreExtensions.IO;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public class LightObject
	{
		public string CollectionName { get; set; }
		public ushort SectionNumber { get; set; }

		public LightSourceType LightType { get; set; }
		public LightSourceAttenuation AttenuationType { get; set; }
		public LightSourceShape Shape { get; set; }
		public LightSourceState State { get; set; }
		public string ExcludeName { get; set; }
		public Color LightColor { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Direction { get; set; }
		public float Radius { get; set; }
		public float Intensity { get; set; }
		public float FarStart { get; set; }
		public float FarEnd { get; set; }
		public float Falloff { get; set; }

		public LightObject()
		{
		}

		public LightObject(LightSource lightSource)
		{
			this.CollectionName = Hashing.ResolveBinEqual(lightSource.Key, lightSource.Name);
			this.SectionNumber = lightSource.ScenerySectionNumber;
			this.LightType = lightSource.LightType;
			this.AttenuationType = lightSource.AttenuationType;
			this.Shape = lightSource.Shape;
			this.State = lightSource.State;
			this.ExcludeName = lightSource.ExcludeNameHash.BinString();
			this.LightColor = Color8UI.ToColor(lightSource.Color);
			this.Position = new Vector3(lightSource.Position.x, lightSource.Position.z, lightSource.Position.y);
			this.Direction = new Vector3(lightSource.Position.x, lightSource.Position.z, lightSource.Position.y);
			this.Radius = lightSource.Size;
			this.Intensity = lightSource.Intensity;
			this.FarStart = lightSource.FarStart;
			this.FarEnd = lightSource.FarEnd;
			this.Falloff = lightSource.Falloff;
		}

		public void Serialize(BinaryWriter bw)
		{
			/* 0x00 */ bw.WriteUnmanaged(this.LightColor);
			/* 0x10 */ bw.WriteUnmanaged(this.Position);
			/* 0x1C */ bw.WriteUnmanaged(this.Direction);
			/* 0x28 */ bw.Write(this.Radius);
			/* 0x2C */ bw.Write((byte)this.LightType);
			/* 0x2D */ bw.Write((byte)this.AttenuationType);
			/* 0x2E */ bw.Write((byte)this.Shape);
			/* 0x2F */ bw.Write((byte)this.State);
			/* 0x30 */ bw.Write(this.Intensity);
			/* 0x34 */ bw.Write(this.FarStart);
			/* 0x38 */ bw.Write(this.FarEnd);
			/* 0x3C */ bw.Write(this.Falloff);

			bw.WriteNullTermUTF8(this.CollectionName);
			bw.WriteNullTermUTF8(this.ExcludeName);
			bw.FillBufferPow2(0x10);
		}

		public void Deserialize(BinaryReader br)
		{
			/* 0x00 */ this.LightColor = br.ReadUnmanaged<Color>();
			/* 0x10 */ this.Position = br.ReadUnmanaged<Vector3>();
			/* 0x1C */ this.Direction = br.ReadUnmanaged<Vector3>();
			/* 0x28 */ this.Radius = br.ReadSingle();
			/* 0x2C */ this.LightType = (LightSourceType)br.ReadByte();
			/* 0x2D */ this.AttenuationType = (LightSourceAttenuation)br.ReadByte();
			/* 0x2E */ this.Shape = (LightSourceShape)br.ReadByte();
			/* 0x2F */ this.State = (LightSourceState)br.ReadByte();
			/* 0x30 */ this.Intensity = br.ReadSingle();
			/* 0x34 */ this.FarStart = br.ReadSingle();
			/* 0x38 */ this.FarEnd = br.ReadSingle();
			/* 0x3C */ this.Falloff = br.ReadSingle();

			this.CollectionName = br.ReadNullTermUTF8();
			this.ExcludeName = br.ReadNullTermUTF8();
			br.AlignReaderPow2(0x10);
		}
	}
}
