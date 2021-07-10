using Hypercooled.Shared.Core;
using System.Collections.Generic;

namespace Hypercooled.Underground2.MapStream
{
	public class VisibleManager
	{
		private readonly Dictionary<ushort, VisibleSectionBoundary> m_visibleSectionBoundaries;
		private readonly Dictionary<ushort, DrivableScenerySection> m_drivableScenerySections;
		private readonly Dictionary<ushort, TrackSpecificSection> m_trackSpecificSections;
		private readonly List<Shared.MapStream.LoadingSection> m_loadingSections;

		public VisibleSectionManageInfo ManagerInfo { get; set; }
		public Dictionary<ushort, VisibleSectionBoundary> VisibleSectionBoundaries => this.m_visibleSectionBoundaries;
		public Dictionary<ushort, DrivableScenerySection> DrivableScenerySections => this.m_drivableScenerySections;
		public Dictionary<ushort, TrackSpecificSection> TrackSpecificSections => this.m_trackSpecificSections;
		public List<Shared.MapStream.LoadingSection> LoadingSections => this.m_loadingSections;

		public VisibleManager()
		{
			this.m_visibleSectionBoundaries = new Dictionary<ushort, VisibleSectionBoundary>();
			this.m_drivableScenerySections = new Dictionary<ushort, DrivableScenerySection>();
			this.m_trackSpecificSections = new Dictionary<ushort, TrackSpecificSection>();
			this.m_loadingSections = new List<Shared.MapStream.LoadingSection>();
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
					MostVisibleSections = drivableSection.MostVisibleSections,
					MaxVisibleSections = drivableSection.MaxVisibleSections,
				};

				for (int i = 0; i < drivableSection.NumVisibleSections; ++i)
				{
					drivable.VisibleSections.Add(drivableSection.VisibleSections[i]);
				}

				result.Drivables.Add(drivable.SectionNumber, drivable);
			}

			// Specifics
			foreach (var specificSection in manager.m_trackSpecificSections.Values)
			{
				var specific = new VisibleSectionManager.Section.Specific()
				{
					TrackID = (ushort)specificSection.TrackID,
				};

				for (int i = 0; i < specificSection.NumSections; ++i)
				{
					specific.ExtraSections.Add(specificSection.SectionIndeces[i]);
				}

				result.Specifics.Add(specific.TrackID, specific);
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

			return result;
		}
	}
}
