using CoreExtensions.IO;
using Hypercooled.Shared.Structures;
using Hypercooled.Underground2.MapStream;
using Hypercooled.Utils;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

namespace Hypercooled.Underground2.Core
{
	public class TopologyManager : Shared.Core.TopologyManager
	{
		private class Section
		{
			public List<int> Indeces { get; }

			public Vector2 LowerLeft { get; }
			public Vector2 LowerRight { get; }
			public Vector2 UpperLeft { get; }
			public Vector2 UpperRight { get; }

			public Section(Vector2 lowerLeft, Vector2 lowerRight, Vector2 upperLeft, Vector2 upperRight)
			{
				this.Indeces = new List<int>();
				this.LowerLeft = lowerLeft;
				this.LowerRight = lowerRight;
				this.UpperLeft = upperLeft;
				this.UpperRight = upperRight;
			}

			public bool Contains(TopologyCoordinate coordinate)
			{
				// For each vertex in quad passed we check whether it is in the section or at least in its borders
				var vertex1 = coordinate.GetCoordinate1Vector2();
				var vertex2 = coordinate.GetCoordinate2Vector2();
				var vertex3 = coordinate.GetCoordinate3Vector2();
				var vertex4 = coordinate.GetCoordinate4Vector2();

				int minX = coordinate.GetMinXIndex();
				int minZ = coordinate.GetMinZIndex();
				int maxX = coordinate.GetMaxXIndex();
				int maxZ = coordinate.GetMaxZIndex();

				// First check if at least one of the vertices of the coordinate is inside
				if (this.IsAtLeastOneVertexInside(vertex1, vertex2, vertex3, vertex4)) return true;

				// Second check whether topology coordinate overlaps this section
				// Overlaps if and only if at least one of its edges intersects section's boundaries
				if (this.IsIntersectingSection(vertex1, vertex2)) return true;
				if (this.IsIntersectingSection(vertex2, vertex3)) return true;
				if (this.IsIntersectingSection(vertex3, vertex4)) return true;
				if (this.IsIntersectingSection(vertex4, vertex1)) return true;

				// Third check whether section itself is within quad. To check this, we need
				// to find whether at least one vertex of this section is inside the quad
				if (this.IsPointInPolygon(this.LowerLeft, minX, minZ, maxX, maxZ, vertex1, vertex2, vertex3, vertex4)) return true;
				if (this.IsPointInPolygon(this.LowerRight, minX, minZ, maxX, maxZ, vertex1, vertex2, vertex3, vertex4)) return true;
				if (this.IsPointInPolygon(this.UpperLeft, minX, minZ, maxX, maxZ, vertex1, vertex2, vertex3, vertex4)) return true;
				if (this.IsPointInPolygon(this.UpperRight, minX, minZ, maxX, maxZ, vertex1, vertex2, vertex3, vertex4)) return true;

				// Return false if shape is completely outside
				return false;
			}

			private bool IsAtLeastOneVertexInside(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
			{
				return (this.IsInXRange(a.x) && this.IsInYRange(a.y)) ||
					   (this.IsInXRange(b.x) && this.IsInYRange(b.y)) ||
					   (this.IsInXRange(c.x) && this.IsInYRange(c.y)) ||
					   (this.IsInXRange(d.x) && this.IsInYRange(d.y));
			}
			private bool IsIntersectingSection(Vector2 a, Vector2 b)
			{
				var dx = b.x - a.x;
				var dy = b.y - a.y;

				if (dx == 0.0f || dy == 0.0f) return false;

				var Lx = dx / dy;
				var Ly = dy / dx;

				// Check if intersecting lower bound (horizontal line)
				var lower = ((this.LowerLeft.y - a.y) / Ly) + a.x;
				if (this.IsInXRange(lower) && this.IsXOnLine(lower, a, b)) return true;

				// Check if intersecting upper bound (horizontal line)
				var upper = ((this.UpperRight.y - a.y) / Ly) + a.x;
				if (this.IsInXRange(upper) && this.IsXOnLine(upper, a, b)) return true;

				// Check if intersecting left bound (vertical line)
				var left = ((this.UpperLeft.x - a.x) / Lx) + a.y;
				if (this.IsInYRange(left) && this.IsYOnLine(left, a, b)) return true;

				// Check if intersecting right bound (vertical line)
				var right = ((this.LowerRight.x - a.x) / Lx) + a.y;
				if (this.IsInYRange(right) && this.IsYOnLine(right, a, b)) return true;

				// If nothing intersects, return false
				return false;
			}
			private bool IsPointInPolygon(Vector2 p, int minX, int minZ, int maxX, int maxZ, params Vector2[] quad)
			{
				// If not in boundary
				if (p.x < quad[minX].x || p.x > quad[maxX].x || p.y < quad[minZ].y || p.y > quad[maxZ].y)
				{
					return false;
				}

				// https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html
				bool inside = false;

				for (int i = 0, j = 3; i < 4; j = i++)
				{
					var qi = quad[i];
					var qj = quad[j];

					if ((qi.y > p.y) != (qj.y > p.y) && p.x < (qj.x - qi.x) * (p.y - qi.y) / (qj.y - qi.y) + qi.x)
					{
						inside = !inside;
					}
				}

				return inside;
			}
			private bool IsInXRange(float x) => this.LowerLeft.x < x && x < this.LowerRight.x;
			private bool IsInYRange(float y) => this.LowerLeft.y < y && y < this.UpperLeft.y;
			private bool IsXOnLine(float x, Vector2 a, Vector2 b) => a.x <= x && x <= b.x;
			private bool IsYOnLine(float y, Vector2 a, Vector2 b) => a.y <= y && y <= b.y;
		}

		private ushort m_sectionNumber;

		public override ushort SectionNumber => this.m_sectionNumber;

		public TopologyManager() : base()
		{
		}

		public override void Load(BinaryReader br)
		{
			this.ParseTree(br);
		}
		public override void Save(BinaryWriter bw)
		{
			this.BuildTree(bw);
		}

		public override void Generate(GameObject gameObject)
		{
			// todo
		}

		public override void FromGroupNames(List<string> groups)
		{
			for (byte i = 0; i < this.Objects.Count; ++i)
			{
				var topology = this.Objects[i] as TopologyObject;

				var index = topology.Coordinates[0].BarrierIndex; // at least 1 coordinate for sure

				topology.CollectionName = index < groups.Count ? groups[index] : i.ToString();

				for (int k = 0; k < topology.Coordinates.Length; ++k)
				{
					topology.Coordinates[k].BarrierIndex = i;
				}
			}
		}
		public override void FromGroupIndices(Dictionary<string, int> groups)
		{
			for (int i = 0; i < this.Objects.Count; ++i)
			{
				var topology = this.Objects[i] as TopologyObject;

				if (!groups.TryGetValue(topology.CollectionName, out int index))
				{
					index = 0;
				}

				topology.CollectionName = String.Empty;
				var barrier = (byte)index;

				for (int k = 0; k < topology.Coordinates.Length; ++k)
				{
					topology.Coordinates[k].BarrierIndex = barrier;
				}
			}
		}

		private void ParseTree(BinaryReader br)
		{
			var position = br.BaseStream.Position;
			var info = br.ReadUnmanaged<BinBlock.Info>();
			var block = new BinBlock(info, position);

			if ((BinBlockID)block.ID != BinBlockID.TopologyTree) return;

			while (br.BaseStream.Position < block.End)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;

				if (id == BinBlockID.TopologyTreeHeader)
				{
					br.AlignReaderPow2(0x10);
					var header = br.ReadUnmanaged<TopologyTreeHeader>();
					this.m_sectionNumber = header.SectionNumber;
				}
				if (id == BinBlockID.TopologyCoordinatesUG2)
				{
					this.ReadTopologyCoordinates(br, size);
				}
				if (id == BinBlockID.TopologyObjectNames)
				{
					this.ReadTopologyObjectNames(br, size);
				}

				br.BaseStream.Position = offset + size;
			}
		}
		private void BuildTree(BinaryWriter bw)
		{
			// If we have no objects, then why are we here in the first place?
			if (this.Objects.Count == 0)
			{
				return;
			}

			// Calculate total coordinate number
			int total = 0;

			// For each topology add its number of coordinates
			foreach (TopologyObject topology in this.Objects)
			{
				total += topology.Coordinates.Length;
			}

			// Not sure if this is possible, but oh well
			if (total == 0)
			{
				return; // wtf
			}

			// Check if we need to write topology names as well
			bool writeNames = this.DoWeWriteTopologyNames();

			// Recalculate shapes
			foreach (TopologyObject topology in this.Objects)
			{
				for (int i = 0; i < topology.Coordinates.Length; ++i)
				{
					topology.Coordinates[i].ShapeForm = topology.Coordinates[i].GetShapeValue();
				}
			}

			// Recalculate barrier indices if needed
			if (writeNames)
			{
				for (int i = 0; i < this.Objects.Count; ++i)
				{
					var topology = this.Objects[i] as TopologyObject;

					for (int k = 0; k < topology.Coordinates.Length; ++k)
					{
						topology.Coordinates[k].BarrierIndex = (byte)i;
					}
				}
			}

			// Get very first valid topology coordinate
			var first = this.GetFirstTopologyCoordinate();

			// Get absolute minimum and absolute maximum coordinates
			var minX = first.GetMinXValue();
			var minZ = first.GetMinZValue();
			var maxX = first.GetMaxXValue();
			var maxZ = first.GetMaxZValue();

			// Iterate through all quads and get min/max values
			foreach (TopologyObject topology in this.Objects)
			{
				for (int i = 0; i < topology.Coordinates.Length; ++i)
				{
					var coordinate = topology.Coordinates[i];

					minX = Math.Min(minX, coordinate.GetMinXValue());
					minZ = Math.Min(minZ, coordinate.GetMinZValue());
					maxX = Math.Max(maxX, coordinate.GetMaxXValue());
					maxZ = Math.Max(maxZ, coordinate.GetMaxZValue());
				}
			}

			// Get boundaries
			var bBoxMin = new Vector4(minX / 8.0f, minZ / 8.0f, -127.0f, 0f);
			var bBoxMax = new Vector4(maxX / 8.0f, maxZ / 8.0f, 300.0f, 0f);

			// Get pivot point
			var pivot = new Vector2((float)Math.Floor(minX / 256.0f) * 32.0f, (float)Math.Floor(minZ / 256.0f) * 32.0f);

			// Generate header
			var header = new TopologyTreeHeader()
			{
				SectionNumber = this.m_sectionNumber,
				Flag1 = 0,
				Flag2 = 0,
				Pivot = pivot,
				BBoxMin = bBoxMin,
				BBoxMax = bBoxMax,
				Width = (ushort)Math.Ceiling((bBoxMax.x - pivot.x) / 32),
				Height = (ushort)Math.Ceiling((bBoxMax.y - pivot.y) / 32),
			};

			// Create sections and populate them with boundaries
			var sections = new Section[header.Width * header.Height];

			// Fill in all sections by their boundaries
			for (int h = 0, c = 0; h < header.Height; ++h)
			{
				for (int w = 0; w < header.Width; ++w)
				{
					var lowerLeft = new Vector2(pivot.x + (w << 5), pivot.y + (h << 5));
					var lowerRight = new Vector2(lowerLeft.x + 32.0f, lowerLeft.y);
					var upperLeft = new Vector2(lowerLeft.x, lowerLeft.y + 32.0f);
					var upperRight = new Vector2(lowerRight.x, upperLeft.y);
					var section = new Section(lowerLeft, lowerRight, upperLeft, upperRight);
					sections[c++] = section;
				}
			}

			// coordinates.Length * sections.Length iterations
			// We iterate through every single quad and check if it is in section
			for (int k = 0, i = 0; k < this.Objects.Count; ++k)
			{
				var topology = this.Objects[k] as TopologyObject;

				for (int n = 0; n < topology.Coordinates.Length; ++n)
				{
					var coordinate = topology.Coordinates[n];

					for (int s = 0; s < sections.Length; ++s)
					{
						var section = sections[s];

						if (section.Contains(coordinate))
						{
							section.Indeces.Add(i);
						}
					}

					++i;
				}
			}

			// Get total number of indices used
			int sum = 0;

			// Increment number of indices by each section
			foreach (var section in sections)
			{
				sum += section.Indeces.Count;
			}

			// Initialize indexations and orderings
			var indexations = new TopologyIndexation[sum];
			var orderings = new TopologyOrdering[sections.Length];

			// For each section make new ordering, as well as corresponsing number of indexations
			for (int i = 0, k = 0; i < sections.Length; ++i)
			{
				var section = sections[i];
				orderings[i] = new TopologyOrdering(k, section.Indeces.Count);

				for (int m = 0; m < section.Indeces.Count; ++m)
				{
					indexations[k++] = new TopologyIndexation(section.Indeces[m]);
				}
			}

			// Finally, write all stuff into a chunk
			bw.Write((uint)BinBlockID.TopologyTree);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			this.WriteTopologyTreeHeader(bw, header);
			this.WriteTopologyCoordinates(bw, total);
			this.WriteTopologyIndexations(bw, indexations);
			this.WriteTopologyOrderings(bw, orderings);
			this.WriteTopologyObjectNames(bw, writeNames);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((int)(end - start));
			bw.BaseStream.Position = end;
		}

		private TopologyCoordinate GetFirstTopologyCoordinate()
		{
			for (int i = 0; i < this.Objects.Count; ++i)
			{
				var topology = this.Objects[i] as TopologyObject;

				if (topology.Coordinates.Length > 0)
				{
					return topology.Coordinates[0];
				}
			}

			return default; // we should never be here
		}
		private void ReadTopologyCoordinates(BinaryReader br, int size)
		{
			var count = size / TopologyCoordinate.SizeOf;
			var lists = new Dictionary<byte, List<TopologyCoordinate>>();

			for (int i = 0; i < count; ++i)
			{
				var coordinate = br.ReadStruct<TopologyCoordinate>();

				if (!lists.TryGetValue(coordinate.BarrierIndex, out var list))
				{
					lists.Add(coordinate.BarrierIndex, list = new List<TopologyCoordinate>());
				}

				list.Add(coordinate);
			}

			foreach (var list in lists.Values)
			{
				this.Objects.Add(new TopologyObject(list));
			}
		}
		private void ReadTopologyObjectNames(BinaryReader br, int size)
		{
			if (size == 0) return;

			int count = br.ReadInt32();

			for (int i = 0; i < count; ++i)
			{
				var name = br.ReadNullTermUTF8();
				this.Objects[i].CollectionName = name;
			}
		}
		private void WriteTopologyTreeHeader(BinaryWriter bw, TopologyTreeHeader header)
		{
			bw.Write((uint)BinBlockID.TopologyTreeHeader);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.AlignWriterPow2(0x10);
			bw.WriteUnmanaged(header);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((int)(end - start));
			bw.BaseStream.Position = end;
		}
		private void WriteTopologyCoordinates(BinaryWriter bw, int total)
		{
			bw.Write((uint)BinBlockID.TopologyCoordinatesUG2);
			bw.Write(total * TopologyCoordinate.SizeOf);

			for (int i = 0; i < this.Objects.Count; ++i)
			{
				var topology = this.Objects[i] as TopologyObject;

				for (int k = 0; k < topology.Coordinates.Length; ++k)
				{
					bw.WriteStruct(topology.Coordinates[k]);
				}
			}
		}
		private void WriteTopologyIndexations(BinaryWriter bw, TopologyIndexation[] indexations)
		{
			bw.Write((uint)BinBlockID.TopologyIndexationUG2);
			bw.Write(indexations.Length * TopologyIndexation.SizeOf);

			for (int i = 0; i < indexations.Length; ++i)
			{
				bw.WriteUnmanaged(indexations[i]);
			}
		}
		private void WriteTopologyOrderings(BinaryWriter bw, TopologyOrdering[] orderings)
		{
			bw.Write((uint)BinBlockID.TopologyOrdrering);
			bw.Write(orderings.Length * TopologyOrdering.SizeOf);

			for (int i = 0; i < orderings.Length; ++i)
			{
				bw.WriteUnmanaged(orderings[i]);
			}
		}
		private void WriteTopologyObjectNames(BinaryWriter bw, bool writeNames)
		{
			if (!writeNames) return;

			bw.Write((uint)BinBlockID.TopologyObjectNames);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.Write(this.Objects.Count);

			for (int i = 0; i < this.Objects.Count; ++i)
			{
				bw.WriteNullTermUTF8(this.Objects[i].CollectionName);
			}

			bw.FillBufferPow2(0x10);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((int)(end - start));
			bw.BaseStream.Position = end;
		}
	}
}
