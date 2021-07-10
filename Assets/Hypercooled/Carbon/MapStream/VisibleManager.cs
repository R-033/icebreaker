using Hypercooled.Shared.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Hypercooled.Carbon.MapStream
{
	public class VisibleManager
	{
		private readonly Dictionary<ushort, VisibleSectionBoundary> m_visibleSectionBoundaries;
		private readonly Dictionary<ushort, DrivableScenerySection> m_drivableScenerySections;
		private readonly List<Shared.MapStream.LoadingSection> m_loadingSections;
		private readonly List<ElevationPolygon> m_elevationPolygons;

		public VisibleSectionManageInfo ManagerInfo { get; set; }
		public Dictionary<ushort, VisibleSectionBoundary> VisibleSectionBoundaries => this.m_visibleSectionBoundaries;
		public Dictionary<ushort, DrivableScenerySection> DrivableScenerySections => this.m_drivableScenerySections;
		public List<Shared.MapStream.LoadingSection> LoadingSections => this.m_loadingSections;
		public List<ElevationPolygon> ElevationPolygons => this.m_elevationPolygons;

		public VisibleManager()
		{
			this.m_visibleSectionBoundaries = new Dictionary<ushort, VisibleSectionBoundary>();
			this.m_drivableScenerySections = new Dictionary<ushort, DrivableScenerySection>();
			this.m_loadingSections = new List<Shared.MapStream.LoadingSection>();
			this.m_elevationPolygons = new List<ElevationPolygon>();
		}

		public static VisibleSectionManager ToVisibleManager(VisibleManager manager)
		{
			// Manager
			var result = new VisibleSectionManager()
			{
				LODOffset = manager.ManagerInfo.LODOffset,
			};

			// Boundaries
			foreach (var visibleBoundary in manager.m_visibleSectionBoundaries.Values)
			{
				var boundary = new VisibleSectionManager.Boundary()
				{
					SectionNumber = visibleBoundary.SectionBoundary.SectionNumber,
					PanoramaBoundary = visibleBoundary.SectionBoundary.PanoramaBoundary,
					DepthSection1 = visibleBoundary.SectionBoundary.DepthSection1,
					DepthSection2 = visibleBoundary.SectionBoundary.DepthSection2,
					DepthName1 = visibleBoundary.SectionBoundary.DepthKey1,
					DepthName2 = visibleBoundary.SectionBoundary.DepthKey2,
					BBoxMin = visibleBoundary.SectionBoundary.BBoxMin,
					BBoxMax = visibleBoundary.SectionBoundary.BBoxMax,
					Center = visibleBoundary.SectionBoundary.Center,
				};

				foreach (var point in visibleBoundary.Points)
				{
					boundary.Points.Add(point);
				}

				result.Boundaries.Add(boundary.SectionNumber, boundary);
			}

			// Drivables
			foreach (var drivableSection in manager.m_drivableScenerySections.Values)
			{
				var drivable = new VisibleSectionManager.Section.Drivable()
				{
					SectionNumber = drivableSection.SectionNumber,
					MostVisibleSections = drivableSection.SectionParams.MostVisibleSections,
					MaxVisibleSections = drivableSection.SectionParams.MaxVisibleSections,
				};

				for (int i = 0; i < drivableSection.VisibleSections.Length; ++i)
				{
					drivable.VisibleSections.Add(drivableSection.VisibleSections[i]);
				}

				result.Drivables.Add(drivable.SectionNumber, drivable);
			}

			// Loadings
			foreach (var loadingSection in manager.m_loadingSections)
			{
				var loading = new VisibleSectionManager.Section.Loading()
				{
					Name = loadingSection.Name,
				};

				for (int i = 0; i < loadingSection.NumDrivableSections; ++i)
				{
					loading.DrivableSections.Add(loadingSection.DrivableSections[i]);
				}

				for (int i = 0; i < loadingSection.NumExtraSections; ++i)
				{
					loading.ExtraSections.Add(loadingSection.ExtraSections[i]);
				}

				result.Loadings.Add(loading);
			}

			// ElevPolies
			foreach (var elevationPolygon in manager.m_elevationPolygons)
			{
				var elevPoly = new VisibleSectionManager.ElevPoly()
				{
					DepthName = elevationPolygon.DepthName,
					Point1 = new Vector3(elevationPolygon.Vertex1.x, elevationPolygon.Vertex1.z, elevationPolygon.Vertex1.y),
					Point2 = new Vector3(elevationPolygon.Vertex2.x, elevationPolygon.Vertex2.z, elevationPolygon.Vertex2.y),
					Point3 = new Vector3(elevationPolygon.Vertex3.x, elevationPolygon.Vertex3.z, elevationPolygon.Vertex3.y),
				};

				result.ElevPolies.Add(elevPoly);
			}

			return result;
		}
	}
}