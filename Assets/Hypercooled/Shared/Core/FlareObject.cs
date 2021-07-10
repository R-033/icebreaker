using CoreExtensions.IO;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public class FlareObject
	{
		public string CollectionName { get; set; }
		public ushort SectionNumber { get; set; }

		public Color ColorTint { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Direction { get; set; }
		public float ReflectPosY { get; set; }
		public byte Type { get; set; }
		public LightFlareFlags Flags { get; set; }

		public FlareObject()
		{
		}

		public FlareObject(LightFlare flare)
		{
			this.CollectionName = flare.Key.BinString();
			this.SectionNumber = flare.ScenerySectionNumber;
			this.ColorTint = Color8UI.ToColor(flare.ColorTint);
			this.Position = new Vector3(flare.Position.x, flare.Position.z, flare.Position.y);
			this.Direction = new Vector3(flare.Direction.x, flare.Direction.z, flare.Direction.y);
			this.ReflectPosY = flare.ReflectPosZ;
			this.Type = flare.Type;
			this.Flags = flare.Flags;

			// Shift color from RGBA to ARGB
			this.ColorTint = new Color()
			{
				a = this.ColorTint.r,
				r = this.ColorTint.g,
				g = this.ColorTint.b,
				b = this.ColorTint.b,
			};
		}

		public void Serialize(BinaryWriter bw)
		{
			/* 0x00 */ bw.WriteUnmanaged(this.ColorTint);
			/* 0x10 */ bw.WriteUnmanaged(this.Position);
			/* 0x1C */ bw.WriteUnmanaged(this.Direction);
			/* 0x28 */ bw.Write(this.ReflectPosY);
			/* 0x2C */ bw.Write((byte)this.Flags);
			/* 0x2D */ bw.Write(this.Type);
			/* 0x2E */ bw.Write((short)0);

			bw.WriteNullTermUTF8(this.CollectionName);
			bw.FillBufferPow2(0x10);
		}

		public void Deserialize(BinaryReader br)
		{
			/* 0x00 */ this.ColorTint = br.ReadUnmanaged<Color>();
			/* 0x10 */ this.Position = br.ReadUnmanaged<Vector3>();
			/* 0x1C */ this.Direction = br.ReadUnmanaged<Vector3>();
			/* 0x28 */ this.ReflectPosY = br.ReadSingle();
			/* 0x2C */ this.Flags = (LightFlareFlags)br.ReadByte();
			/* 0x2D */ this.Type = br.ReadByte();
			/* 0x2E */ br.BaseStream.Position += 2;

			this.CollectionName = br.ReadNullTermUTF8();
			br.AlignReaderPow2(0x10);
		}
	}
}
