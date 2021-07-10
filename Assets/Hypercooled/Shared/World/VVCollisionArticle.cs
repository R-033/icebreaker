using CoreExtensions.IO;
using Hypercooled.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Hypercooled.Shared.World
{
	public class VVCollisionArticle : VVResolvable // ca
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VVCollisionHeader
		{
			public ushort StripCount;
			public ushort StripSize;
			public ushort EdgeCount;
			public ushort EdgeSize;

			[MarshalAs(UnmanagedType.U1)]
			public bool HasDamagedVersionFollowing;

			public byte SurfacesCount;
			public ushort SurfacesSize;
			public ushort CompartmentID;
			public ushort Flags; // ???

			public const int SizeOf = 0x10;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VVCollisionStripSphere
		{
			public Vector3 Position;
			public ushort Radius; // boundary radius
			public ushort Offset; // offset of tris

			public const int SizeOf = 0x10;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VVCollisionVertex
		{
			[Flags()]
			public enum VertexFlags : byte
			{
				FacingUp = 1,
				FacingUnknown = 2,
			}

			public short X;
			public short Z;
			public short Y;
			public byte Surface;
			public byte Flags;

			public const int SizeOf = 0x08;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VVCollisionSurface
		{
			public uint SurfaceKey;

			public const int SizeOf = 0x04;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct VVCollisionBarrier
		{
			public Vector3 PointMin;
			public byte Padding;
			public VVCollisionFlags Flags;
			public ushort Unknown; // ???
			public Vector3 PointMax;
			public float NormalDirection;

			public const int SizeOf = 0x20;
		}

		public class VVCollisionStrip
		{
			public Vector3 Position { get; set; }
			public float Radius { get; set; }
			public int PointStart { get; set; }
			public int PointCount { get; set; }
		}

		public class VVCollisionPoint
		{
			public Vector3 Point { get; set; }
			public byte Surface { get; set; }
			public byte Flags { get; set; }
		}

		public class VVCollisionEdge
		{
			public Vector3 PointMin { get; set; }
			public Vector3 PointMax { get; set; }
			public VVCollisionFlags Flags { get; set; }
			public ushort Unknown { get; set; }
			public float NormalDirection { get; set; }
		}

		public override UppleUTag Tag => UppleUTag.WCollisionArticle;
		public override int SizeOfThis => this.InternalGetSizeOfThis();
		public override bool DynamicSize => true;

		public ushort ComparmentID { get; set; }
		public ushort Flags { get; set; }
		public VVCollisionStrip[] Strips { get; set; }
		public VVCollisionPoint[] Points { get; set; }
		public VVCollisionEdge[] Edges { get; set; }
		public string[] Surfaces { get; set; }

		public VVCollisionArticle()
		{
			this.Strips = new VVCollisionStrip[0];
			this.Points = new VVCollisionPoint[0];
			this.Edges = new VVCollisionEdge[0];
			this.Surfaces = new string[0];
		}

		public override void Read(BinaryReader br)
		{
			var header = br.ReadUnmanaged<VVCollisionHeader>();
			this.ComparmentID = header.CompartmentID;
			this.Flags = header.Flags;

			var stripoff = header.StripCount * VVCollisionStripSphere.SizeOf;
			var vertcount = (header.StripSize - stripoff) / VVCollisionVertex.SizeOf;

			var strips = new VVCollisionStripSphere[header.StripCount];
			var edges = new VVCollisionBarrier[header.EdgeCount];
			var points = new VVCollisionVertex[vertcount];
			var surfaces = new VVCollisionSurface[header.SurfacesCount];

			var end = br.BaseStream.Position;

			for (int i = 0; i < strips.Length; ++i)
			{
				strips[i] = br.ReadUnmanaged<VVCollisionStripSphere>();
			}
			for (int i = 0; i < points.Length; ++i)
			{
				points[i] = br.ReadUnmanaged<VVCollisionVertex>();
			}
			for (int i = 0; i < edges.Length; ++i)
			{
				edges[i] = br.ReadUnmanaged<VVCollisionBarrier>();
			}
			for (int i = 0; i < surfaces.Length; ++i)
			{
				surfaces[i] = br.ReadUnmanaged<VVCollisionSurface>();
			}

			this.Strips = new VVCollisionStrip[strips.Length];
			this.Points = new VVCollisionPoint[points.Length];
			this.Edges = new VVCollisionEdge[edges.Length];
			this.Surfaces = new string[surfaces.Length];

			for (int i = 0; i < this.Edges.Length; ++i)
			{
				var edge = edges[i];

				this.Edges[i] = new VVCollisionEdge()
				{
					Flags = edge.Flags,
					NormalDirection = edge.NormalDirection,
					Unknown = edge.Unknown,
					PointMin = new Vector3(edge.PointMin.x, edge.PointMin.y, edge.PointMin.z),
					PointMax = new Vector3(edge.PointMax.x, edge.PointMax.y, edge.PointMax.z),
				};
			}
			for (int i = 0; i < this.Points.Length; ++i)
			{
				var point = points[i];

				this.Points[i] = new VVCollisionPoint()
				{
					Flags = point.Flags,
					Surface = point.Surface,
					Point = new Vector3(point.X * 0.0078125f, point.Y * 0.0078125f, point.Z * 0.0078125f), // 1 / 128
				};
			}
			for (int i = 0; i < this.Strips.Length; ++i)
			{
				var strip = strips[i];

				var next = (i + 1) == this.Strips.Length
					? header.StripSize
					: strips[i + 1].Offset;

				this.Strips[i] = new VVCollisionStrip()
				{
					Position = strip.Position,
					Radius = strip.Radius * 0.0078125f, // 1 / 128
					PointStart = (strip.Offset - stripoff) / VVCollisionVertex.SizeOf,
					PointCount = (next - strip.Offset) / VVCollisionVertex.SizeOf,
				};
			}
			for (int i = 0; i < this.Surfaces.Length; ++i)
			{
				this.Surfaces[i] = surfaces[i].SurfaceKey.VltString();
			}

			end += header.StripSize;
			end += header.EdgeSize;
			end += header.SurfacesSize;

			br.BaseStream.Position = end;
		}

		public override void Write(BinaryWriter bw)
		{
			var start = bw.BaseStream.Position;

			var header = new VVCollisionHeader()
			{
				StripCount = (ushort)this.Strips.Length,
				StripSize = (ushort)(this.Strips.Length * VVCollisionStripSphere.SizeOf + this.Points.Length * VVCollisionVertex.SizeOf),
				EdgeCount = (ushort)this.Edges.Length,
				EdgeSize = (ushort)(this.Edges.Length * VVCollisionBarrier.SizeOf),
				HasDamagedVersionFollowing = false,
				SurfacesCount = (byte)this.Surfaces.Length,
				SurfacesSize = (ushort)(this.Surfaces.Length * VVCollisionSurface.SizeOf),
				CompartmentID = this.ComparmentID,
				Flags = 0, // ???
			};

			bw.WriteUnmanaged(header);

			for (int i = 0, k = this.Strips.Length * VVCollisionStripSphere.SizeOf; i < this.Strips.Length; ++i)
			{
				var mstrip = this.Strips[i];

				var cstrip = new VVCollisionStripSphere()
				{
					Position = mstrip.Position,
					Radius = (ushort)(mstrip.Radius * 128.0f),
					Offset = (ushort)k,
				};

				bw.WriteUnmanaged(cstrip);
				k += mstrip.PointCount * VVCollisionVertex.SizeOf;
			}

			for (int i = 0; i < this.Points.Length; ++i)
			{
				var mpoint = this.Points[i];

				var cpoint = new VVCollisionVertex()
				{
					X = (short)(mpoint.Point.x * 128.0f),
					Y = (short)(mpoint.Point.y * 128.0f),
					Z = (short)(mpoint.Point.z * 128.0f),
					Surface = mpoint.Surface,
					Flags = mpoint.Flags,
				};

				bw.WriteUnmanaged(cpoint);
			}

			for (int i = 0; i < this.Edges.Length; ++i)
			{
				var medge = this.Edges[i];

				var cedge = new VVCollisionBarrier()
				{
					PointMin = medge.PointMin,
					Padding = 0,
					Flags = medge.Flags,
					Unknown = 0x7C91, // whaaaat
					PointMax = medge.PointMax,
					NormalDirection = medge.NormalDirection,
				};

				bw.WriteUnmanaged(cedge);
			}

			for (int i = 0; i < this.Surfaces.Length; ++i)
			{
				bw.Write(this.Surfaces[i].VltHash());
			}

			var end = bw.BaseStream.Position;
			int dif = (int)(end - start);

			bw.WriteBytes(0, GenericHelper.CeilToTheNearestPow2(dif, 0x40) - dif);
		}

		private int InternalGetSizeOfThis()
		{
			int result = VVCollisionHeader.SizeOf;
			result += this.Strips.Length * VVCollisionStripSphere.SizeOf;
			result += this.Points.Length * VVCollisionVertex.SizeOf;
			result += this.Edges.Length * VVCollisionBarrier.SizeOf;
			result += this.Surfaces.Length * VVCollisionSurface.SizeOf;

			return GenericHelper.CeilToTheNearestPow2(result, 0x40);
		}
	}
}
