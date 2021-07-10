using CoreExtensions.IO;
using Hypercooled.Utils;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public class EmitterObject
	{
		public string CollectionName { get; set; }
		public string EmitterName { get; set; }
		public string SceneryGroupName { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Scale { get; set; }
		public Quaternion Rotation { get; set; }

		public EmitterObject()
		{
		}

		public EmitterObject(Carbon.MapStream.AcidEffect effect)
		{
			this.CollectionName = effect.Key.VltString();
			this.EmitterName = this.CollectionName;

			var transform = MathExtensions.TransformToPlain(effect.Transform);

			this.Position = transform.Position;
			this.Scale = transform.Scale;
			this.Rotation = transform.Rotation;
		}

		public EmitterObject(MostWanted.MapStream.AcidEffect effect)
		{
			this.CollectionName = effect.Key.VltString();
			this.EmitterName = this.CollectionName;

			var transform = MathExtensions.TransformToPlain(effect.Transform);

			this.Position = transform.Position;
			this.Scale = transform.Scale;
			this.Rotation = transform.Rotation;
		}

		public EmitterObject(Underground2.MapStream.AcidEffect effect)
		{
			this.CollectionName = Hashing.ResolveBinEqual(effect.Key, effect.Name);
			this.EmitterName = effect.EmitterKey.BinString();
			this.SceneryGroupName = effect.SceneryGroupHash.BinString();

			var transform = MathExtensions.TransformToPlain(effect.Transform);

			this.Position = transform.Position;
			this.Scale = transform.Scale;
			this.Rotation = transform.Rotation;
		}

		public void Serialize(BinaryWriter bw)
		{
			/* 0x00 */ bw.WriteUnmanaged(this.Position);
			/* 0x0C */ bw.WriteUnmanaged(this.Scale);
			/* 0x18 */ bw.WriteUnmanaged(this.Rotation);

			bw.WriteNullTermUTF8(this.CollectionName);
			bw.WriteNullTermUTF8(this.EmitterName);
			bw.WriteNullTermUTF8(this.SceneryGroupName);
			bw.FillBufferPow2(0x10);
		}

		public void Deserialize(BinaryReader br)
		{
			/* 0x00 */ this.Position = br.ReadUnmanaged<Vector3>();
			/* 0x0C */ this.Scale = br.ReadUnmanaged<Vector3>();
			/* 0x18 */ this.Rotation = br.ReadUnmanaged<Quaternion>();

			this.CollectionName = br.ReadNullTermUTF8();
			this.EmitterName = br.ReadNullTermUTF8();
			this.SceneryGroupName = br.ReadNullTermUTF8();
			br.AlignReaderPow2(0x10);
		}
	}
}
