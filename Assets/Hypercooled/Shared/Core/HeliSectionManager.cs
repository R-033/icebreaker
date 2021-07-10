using CoreExtensions.IO;
using Hypercooled.Shared.MapStream;
using Hypercooled.Shared.Structures;
using Hypercooled.Utils;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public class HeliSectionManager
	{
		public struct HeliCoordinate
		{
			public Vector3 Point1 { get; set; }
			public Vector3 Point2 { get; set; }
			public Vector3 Point3 { get; set; }

			public HeliCoordinate(Vector3 p1, Vector3 p2, Vector3 p3)
			{
				this.Point1 = p1;
				this.Point2 = p2;
				this.Point3 = p3;
			}

			public static HeliCoordinate FromHeliPolygon(HeliPolygon polygon)
			{
				return new HeliCoordinate
				(
					new Vector3(polygon.X1 * 0.25f, polygon.Z1 * 0.0625f, polygon.Y1 * 0.25f),
					new Vector3(polygon.X2 * 0.25f, polygon.Z2 * 0.0625f, polygon.Y2 * 0.25f),
					new Vector3(polygon.X3 * 0.25f, polygon.Z3 * 0.0625f, polygon.Y3 * 0.25f)
				);
			}

			public static HeliPolygon ToHeliPolygon(HeliCoordinate coordinate)
			{
				return new HeliPolygon()
				{
					X1 = (short)(coordinate.Point1.x * 4.0f),
					X2 = (short)(coordinate.Point2.x * 4.0f),
					X3 = (short)(coordinate.Point3.x * 4.0f),
					Y1 = (short)(coordinate.Point1.z * 4.0f),
					Y2 = (short)(coordinate.Point2.z * 4.0f),
					Y3 = (short)(coordinate.Point3.z * 4.0f),
					Z1 = (short)(coordinate.Point1.y * 16.0f),
					Z2 = (short)(coordinate.Point2.y * 16.0f),
					Z3 = (short)(coordinate.Point3.y * 16.0f),
				};
			}
		}

		private static readonly HeliCoordinate[] ms_heliEmpty;

		private ushort m_sectionNumber;
		private HeliCoordinate[] m_coordinates;

		public ushort SectionNumber => this.m_sectionNumber;
		public HeliCoordinate[] HeliCoordinates
		{
			get => this.m_coordinates;
			set => this.m_coordinates = value ?? ms_heliEmpty;
		}

		static HeliSectionManager()
		{
			HeliSectionManager.ms_heliEmpty = new HeliCoordinate[0];
		}
		public HeliSectionManager()
		{
			this.m_coordinates = HeliSectionManager.ms_heliEmpty;
		}

		public void Load(BinaryReader br)
		{
			this.ParseHeliShit(br);
		}
		public void Save(BinaryWriter bw)
		{
			this.BuildHeliShit(bw);
		}

		private void ParseHeliShit(BinaryReader br)
		{
			var position = br.BaseStream.Position;
			var info = br.ReadUnmanaged<BinBlock.Info>();
			var block = new BinBlock(info, position);

			if ((BinBlockID)block.ID != BinBlockID.HeliSheetManager || block.Size == 0)
			{
				return;
			}

			br.AlignReaderPow2(0x10);

			var header = br.ReadUnmanaged<HeliSection>();

			this.m_sectionNumber = (ushort)header.SectionNumber;
			this.m_coordinates = new HeliCoordinate[header.NumPolies];

			for (int i = 0; i < header.NumPolies; ++i)
			{
				this.m_coordinates[i] = HeliCoordinate.FromHeliPolygon(br.ReadUnmanaged<HeliPolygon>());
			}
		}
		private void BuildHeliShit(BinaryWriter bw)
		{
			bw.Write((uint)BinBlockID.HeliSheetManager);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.AlignWriterPow2(0x10);

			bw.WriteUnmanaged(new HeliSection()
			{
				SectionNumber = this.m_sectionNumber,
				NumPolies = this.m_coordinates.Length,
			});

			for (int i = 0; i < this.m_coordinates.Length; ++i)
			{
				bw.WriteUnmanaged(HeliCoordinate.ToHeliPolygon(this.m_coordinates[i]));
			}

			bw.FillBufferPow2(0x04);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((int)(end - start));
			bw.BaseStream.Position = end;
		}
	}
}
