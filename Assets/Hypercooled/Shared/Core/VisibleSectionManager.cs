using Hypercooled.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public class VisibleSectionManager
	{
		public class Section
		{
			public class Drivable
			{
				public ushort SectionNumber { get; set; }
				public byte MostVisibleSections { get; set; }
				public byte MaxVisibleSections { get; set; }
				public List<ushort> VisibleSections { get; }

				public Drivable() => this.VisibleSections = new List<ushort>();

				public bool ContainsVisible(ushort sectionNumber)
				{
					for (int i = 0; i < this.VisibleSections.Count; ++i)
					{
						if (this.VisibleSections[i] == sectionNumber)
						{
							return true;
						}
					}

					return false;
				}
			}

			public class Specific
			{
				public ushort TrackID { get; set; }
				public List<ushort> ExtraSections { get; }

				public Specific() => this.ExtraSections = new List<ushort>();
			}

			public class Loading
			{
				public string Name { get; set; }
				public List<ushort> DrivableSections { get; }
				public List<ushort> ExtraSections { get; }

				public Loading()
				{
					this.DrivableSections = new List<ushort>();
					this.ExtraSections = new List<ushort>();
				}

				public bool ContainsDrivable(ushort sectionNumber)
				{
					for (int i = 0; i < this.DrivableSections.Count; ++i)
					{
						if (this.DrivableSections[i] == sectionNumber)
						{
							return true;
						}
					}

					return false;
				}
			}
		}

		public class Boundary
		{
			public ushort SectionNumber { get; set; }
			public byte PanoramaBoundary { get; set; }
			public ushort DepthSection1 { get; set; }
			public ushort DepthSection2 { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint DepthName1 { get; set; }

			[JsonConverter(typeof(BinStringConverter))]
			public uint DepthName2 { get; set; }

			public Vector2 BBoxMin { get; set; }
			public Vector2 BBoxMax { get; set; }
			public Vector2 Center { get; set; }
			public List<Vector2> Points { get; }

			public Boundary() => this.Points = new List<Vector2>();
		}

		public class ElevPoly
		{
			[JsonConverter(typeof(BinStringConverter))]
			public uint DepthName { get; set; }
			public Vector3 Point1 { get; set; }
			public Vector3 Point2 { get; set; }
			public Vector3 Point3 { get; set; }
		}

		public int LODOffset { get; set; }
		public Dictionary<ushort, Boundary> Boundaries { get; }
		public Dictionary<ushort, Section.Drivable> Drivables { get; }
		public Dictionary<ushort, Section.Specific> Specifics { get; }
		public List<Section.Loading> Loadings { get; }
		public List<ElevPoly> ElevPolies { get; }

		public VisibleSectionManager()
		{
			this.Boundaries = new Dictionary<ushort, Boundary>();
			this.Drivables = new Dictionary<ushort, Section.Drivable>();
			this.Specifics = new Dictionary<ushort, Section.Specific>();
			this.Loadings = new List<Section.Loading>();
			this.ElevPolies = new List<ElevPoly>();
		}

		public Boundary GetDrivableBoundaryByPoint(Vector2 point)
		{
			foreach (var boundary in this.Boundaries.Values)
			{
				if (!this.Drivables.ContainsKey(boundary.SectionNumber)) continue;

				if (MathExtensions.IsInBoundingBox(point, boundary.BBoxMin, boundary.BBoxMax))
				{
					if (MathExtensions.IsInPolygon(boundary.Points, point))
					{
						return boundary;
					}
				}
			}

			return null;
		}
		public Boundary GetDrivableBoundaryByPoint(Vector3 point)
		{
			return this.GetDrivableBoundaryByPoint(new Vector2(point.x, point.z));
		}
		public uint GetDepthNameByPoint(Vector3 point)
		{
			var position = new Vector2(point.x, point.z);
			var minimum = -999.0f; // from carbon
			var result = 0u;

			foreach (var elevPoly in this.ElevPolies)
			{
				if (MathExtensions.IsInTriangle(point, elevPoly.Point1, elevPoly.Point2, elevPoly.Point3))
				{
					var elevation = MathExtensions.GetTriElevation(position, elevPoly.Point1, elevPoly.Point2, elevPoly.Point3);

					if (elevation < point.y && elevation > minimum)
					{
						minimum = elevation;
						result = elevPoly.DepthName;
					}
				}
			}

			return result;
		}
	}
}
