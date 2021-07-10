using System.IO;
using System.Runtime.InteropServices;
using CoreExtensions.IO;
using UnityEngine;

namespace Hypercooled.MostWanted.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct VisibleSectionBoundInternal
	{
		public long Padding;
		public ushort SectionNumber;
		public byte NumPoints;
		public byte PanoramaBoundary;
		public Vector2 BBoxMin;
		public Vector2 BBoxMax;
		public Vector2 Center;

		public const int SizeOf = 0x24;
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
				this.m_points[i] = new Vector2(br.ReadSingle(), br.ReadSingle());
			}
		}

		public int SizeOf => VisibleSectionBoundInternal.SizeOf + this.m_points.Length * 0x08;
	}
}
