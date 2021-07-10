using CoreExtensions.IO;
using Hypercooled.Shared.MapStream;
using Hypercooled.Utils;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public class TriggerObject
	{
		public string CollectionName { get; set; }
		public ushort SectionNumber { get; set; }

		public uint EventID { get; set; }
		public TriggerParameterType Parameter { get; set; }
		public int TrackDirectionMask { get; set; }
		public Vector3 Position { get; set; }
		public float Radius { get; set; }

		public TriggerObject()
		{
		}

		public TriggerObject(ushort sectionNumber, EventTriggerEntry eventTrigger)
		{
			this.CollectionName = eventTrigger.Key.BinString();
			this.SectionNumber = sectionNumber;
			this.EventID = (uint)eventTrigger.EventID;
			this.Parameter = eventTrigger.Parameter;
			this.TrackDirectionMask = eventTrigger.TrackDirectionMask;
			this.Radius = eventTrigger.Radius;
			this.Position = new Vector3(eventTrigger.Position.x, eventTrigger.Position.z, eventTrigger.Position.y);
		}

		public void Serialize(BinaryWriter bw)
		{
			/* 0x00 */ bw.WriteUnmanaged(this.Position);
			/* 0x0C */ bw.Write(this.Radius);
			/* 0x10 */ bw.Write(this.EventID);
			/* 0x14 */ bw.Write((uint)this.Parameter);
			/* 0x18 */ bw.Write(this.TrackDirectionMask);
			/* 0x1C */ bw.Write(0);

			bw.WriteNullTermUTF8(this.CollectionName);
			bw.FillBufferPow2(0x10);
		}

		public void Deserialize(BinaryReader br)
		{
			/* 0x00 */ this.Position = br.ReadUnmanaged<Vector3>();
			/* 0x0C */ this.Radius = br.ReadSingle();
			/* 0x10 */ this.EventID = br.ReadUInt32();
			/* 0x14 */ this.Parameter = (TriggerParameterType)br.ReadUInt32();
			/* 0x18 */ this.TrackDirectionMask = br.ReadInt32();
			/* 0x1C */ br.BaseStream.Position += 4;

			this.CollectionName = br.ReadNullTermUTF8();
			br.AlignReaderPow2(0x10);
		}
	}
}
