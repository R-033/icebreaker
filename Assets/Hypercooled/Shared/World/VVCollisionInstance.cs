using CoreExtensions.IO;
using Hypercooled.Utils;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Hypercooled.Shared.World
{
	public class VVCollisionInstance : VVResolvable // ci
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Data
		{
			public Vector4 InvertedMatrixRow0Width;
			public ushort IterationStamp;
			public VVCollisionFlags Flags;

			[MarshalAs(UnmanagedType.U1)]
			public bool UsesBarrierHash;

			public float Height;
			public ushort GroupNumber;
			public ushort RenderInstanceIndex;
			public int Pointer;
			public Vector4 InvertedMatrixRow2Length;
			public Vector4 InvertedPositionRadius;

			public const int SizeOf = 0x40;
		}

		public override UppleUTag Tag => UppleUTag.WCollisionInstance;
		public override int SizeOfThis => Data.SizeOf;
		public override bool DynamicSize => false;

		public Vector3 Position { get; set; }
		public Vector3 Scale { get; set; }
		public Quaternion Rotation { get; set; }
		
		public float Width { get; set; }
		public float Length { get; set; }
		public float Height { get; set; }
		public float Radius { get; set; } // sqrt(Length * Length + Width * Width)
		
		public VVCollisionFlags Flags { get; set; }
		public ushort IterationStamp { get; set; }
		public bool UsesBarrierHash { get; set; }
		public uint SceneryBarrierGroup { get; set; }

		private Matrix4x4 MakeMatrix(Data data)
		{
			var result = new Matrix4x4
			(
				data.InvertedMatrixRow0Width.x, data.InvertedMatrixRow0Width.y, data.InvertedMatrixRow0Width.z, 0.0f,
				0.0f, 1.0f, 0.0f, 0.0f,
				data.InvertedMatrixRow2Length.x, data.InvertedMatrixRow2Length.y, data.InvertedMatrixRow2Length.z, 0.0f,
				data.InvertedPositionRadius.x, data.InvertedPositionRadius.y, data.InvertedPositionRadius.z, 1.0f
			);

			if ((data.Flags & (VVCollisionFlags.YVecNotUp | VVCollisionFlags.Dynamic)) != 0)
			{
				result.M21 = result.M13 * result.M32 - result.M12 * result.M33;
				result.M22 = result.M11 * result.M33 - result.M13 * result.M31;
			}

			return MathExtensions.OrthoInverse(result);
		}
		private Vector4 GetInvertedMatrixRow0Width(Matrix4x4 invertedMatrix)
		{
			return new Vector4(invertedMatrix.M11, invertedMatrix.M12, invertedMatrix.M13, this.Width);
		}
		private Vector4 GetInvertedMatrixRow2Length(Matrix4x4 invertedMatrix)
		{
			return new Vector4(invertedMatrix.M31, invertedMatrix.M32, invertedMatrix.M33, this.Length);
		}
		private Vector4 GetInvertedPositionRadius(Matrix4x4 invertedMatrix)
		{
			return new Vector4(invertedMatrix.M41, invertedMatrix.M42, invertedMatrix.M43, this.Radius);
		}

		public void SetBarrierGroup(uint key)
		{
			this.SceneryBarrierGroup = key;
		}
		public void SetIndexAndGroup(ushort index, ushort group)
		{
			this.SceneryBarrierGroup = (uint)((index << 0x10) | group);
		}
		public ushort GetGroupNumber()
		{
			return (ushort)this.SceneryBarrierGroup;
		}
		public ushort GetIndexNumber()
		{
			return (ushort)(this.SceneryBarrierGroup >> 0x10);
		}

		public override void Read(BinaryReader br)
		{
			var data = br.ReadUnmanaged<Data>();

			var transform = this.MakeMatrix(data);
			var plain = MathExtensions.TransformToPlain(transform);

			this.Position = plain.Position;
			this.Scale = plain.Scale;
			this.Rotation = plain.Rotation;

			this.Width = data.InvertedMatrixRow0Width.w;
			this.Length = data.InvertedMatrixRow2Length.w;
			this.Radius = data.InvertedPositionRadius.w;
			this.Height = data.Height;

			this.Flags = data.Flags;
			this.IterationStamp = data.IterationStamp;
			this.UsesBarrierHash = data.UsesBarrierHash;
			this.SetIndexAndGroup(data.RenderInstanceIndex, data.GroupNumber);
		}

		public override void Write(BinaryWriter bw)
		{
			var matrix = MathExtensions.OrthoInverse(MathExtensions.TRS(this.Position, this.Scale, this.Rotation));

			var data = new Data()
			{
				InvertedMatrixRow0Width = this.GetInvertedMatrixRow0Width(matrix),
				IterationStamp = this.IterationStamp,
				Flags = this.Flags,
				UsesBarrierHash = this.UsesBarrierHash,
				Height = this.Height,
				GroupNumber = this.GetGroupNumber(),
				RenderInstanceIndex = this.GetIndexNumber(),
				Pointer = 0,
				InvertedMatrixRow2Length = this.GetInvertedMatrixRow2Length(matrix),
				InvertedPositionRadius = this.GetInvertedPositionRadius(matrix),
			};

			bw.WriteUnmanaged(data);
		}
	}
}
