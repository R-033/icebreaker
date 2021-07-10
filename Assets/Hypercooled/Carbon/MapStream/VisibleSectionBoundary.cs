using System.IO;
using System.Runtime.InteropServices;
using CoreExtensions.IO;
using UnityEngine;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VisibleSectionBoundInternal
	{
		public long Padding1;
		public ushort SectionNumber;
		public byte NumPoints;
		public byte PanoramaBoundary;
		public ushort DepthSection1;
		public ushort DepthSection2;
		public uint DepthKey1;
		public uint DepthKey2;
		public Vector2 BBoxMin;
		public Vector2 BBoxMax;
		public Vector2 Center;

		public const int SizeOf = 0x30;
	}

	public class VisibleSectionBoundary // 0x00034152
	{
		private Vector2[] m_points;

		public ushort SectionNumber => this.SectionBoundary.SectionNumber;
		public VisibleSectionBoundInternal SectionBoundary { get; set; }

		public Vector2[] Points
		{
			get => this.m_points;
			set => this.m_points = value ?? new Vector2[0];
		}

		public VisibleSectionBoundary()
		{
			this.m_points = new Vector2[0];
		}

		public void Read(BinaryReader br)
		{
			this.SectionBoundary = br.ReadUnmanaged<VisibleSectionBoundInternal>();
			this.m_points = new Vector2[this.SectionBoundary.NumPoints];

			for (int i = 0; i < this.SectionBoundary.NumPoints; ++i)
			{
				this.m_points[i] = br.ReadUnmanaged<Vector2>();
			}
		}

		public int SizeOf => VisibleSectionBoundInternal.SizeOf + this.m_points.Length * 0x08;
	}
}
