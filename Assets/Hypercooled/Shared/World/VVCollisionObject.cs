using CoreExtensions.IO;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Hypercooled.Shared.World
{
	public class VVCollisionObject : VVResolvable // co
	{
		public enum ObjectType : byte
		{
			Box = 0,
			Cylinder = 1,
		}

		public enum ObjectFlags : ushort
		{
			Dynamic = 0x01,
			Vehicle = 0x02,
			Character = 0x04,
			PlayerControlled = 0x08,
			HenchControlled = 0x10,
			Disabled = 0x20,
			Unrenderable = 0x40,
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Data
		{
			public Vector4 PositionRadius;
			public Vector4 Dimensions;
			public ObjectType Type;
			public byte Shape;
			public ObjectFlags Flags;
			public ushort RenderInstanceIndex;
			public ushort Padding;
			public uint GridElement;
			public int Pointer;
			public Matrix4x4 Transform;

			public const int SizeOf = 0x70;
		}

		public override UppleUTag Tag => UppleUTag.WCollisionObject;
		public override int SizeOfThis => Data.SizeOf;
		public override bool DynamicSize => false;

		public Vector3 Position { get; set; }
		public Vector3 Dimensions { get; set; }
		public float Radius { get; set; }
		public ObjectType Type { get; set; }
		public byte Shape { get; set; }
		public ObjectFlags Flags { get; set; }
		public ushort InstanceIndex { get; set; }
		public uint GridElement { get; set; }
		public Matrix4x4 Transform { get; set; }

		public override void Read(BinaryReader br)
		{
			var data = br.ReadUnmanaged<Data>();

			this.Position = new Vector3(data.PositionRadius.x, data.PositionRadius.y, data.PositionRadius.z);
			this.Dimensions = new Vector3(data.Dimensions.x, data.Dimensions.y, data.Dimensions.z);
			this.Radius = data.PositionRadius.w;
			this.Type = data.Type;
			this.Shape = data.Shape;
			this.Flags = data.Flags;
			this.InstanceIndex = data.RenderInstanceIndex; // do we need this?
			this.GridElement = data.GridElement;
			this.Transform = data.Transform;
		}

		public override void Write(BinaryWriter bw)
		{
			var data = new Data()
			{
				PositionRadius = new Vector4(this.Position.x, this.Position.y, this.Position.z, this.Radius),
				Dimensions = new Vector4(this.Dimensions.x, this.Dimensions.y, this.Dimensions.z, 0.0f),
				Type = this.Type,
				Shape = this.Shape,
				Flags = this.Flags,
				RenderInstanceIndex = this.InstanceIndex,
				Padding = 0,
				GridElement = this.GridElement,
				Transform = this.Transform,
			};

			bw.WriteUnmanaged(data);
		}
	}
}
