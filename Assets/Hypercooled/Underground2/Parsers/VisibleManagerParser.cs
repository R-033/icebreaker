using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using Hypercooled.Underground2.MapStream;
using System.IO;

namespace Hypercooled.Underground2.Parsers
{
	public class VisibleManagerParser : IParser
	{
		private BinBlock m_block;
		private VisibleManager m_visibleSectionManager;

		/// <inheritdoc/>
		public uint BlockID => (uint)BinBlockID.VisibleSectionManager;

		public VisibleManager SectionManager => this.m_visibleSectionManager;

		/// <inheritdoc/>
		public void Prepare(BinBlock block)
		{
			this.m_block = block;
		}

		/// <inheritdoc/>
		public void Disassemble(BinaryReader br)
		{
			if (br is null) return;
			this.m_visibleSectionManager = new VisibleManager();
			br.BaseStream.Position = this.m_block.Start;

			while (br.BaseStream.Position < this.m_block.End)
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();
				var offset = br.BaseStream.Position;

				switch (id)
				{
					case BinBlockID.VisibleSectionManageInfo: this.ReadVisibleSectionManageInfo(br); break;
					case BinBlockID.VisibleSectionBoundaries: this.ReadVisibleSectionBoundaries(br, size); break;
					case BinBlockID.DrivableScenerySections: this.ReadDrivableScenerySections(br, size); break;
					case BinBlockID.TrackSpecificSections: this.ReadTrackSpecificSections(br, size); break;
					case BinBlockID.LoadingSections: this.ReadLoadingSections(br, size); break;
					default: break;
				}

				br.BaseStream.Position = offset + size;
			}
		}

		private void ReadVisibleSectionManageInfo(BinaryReader br)
		{
			this.m_visibleSectionManager.ManagerInfo = br.ReadStruct<VisibleSectionManageInfo>();
		}

		private void ReadVisibleSectionBoundaries(BinaryReader br, int size)
		{
			var current = br.BaseStream.Position;

			while (br.BaseStream.Position < current + size)
			{
				var boundary = new VisibleSectionBoundary();
				boundary.Read(br);
				this.m_visibleSectionManager.VisibleSectionBoundaries.Add(boundary.SectionNumber, boundary);
			}
		}

		private void ReadDrivableScenerySections(BinaryReader br, int size)
		{
			var count = size / DrivableScenerySection.SizeOf;

			for (int i = 0; i < count; ++i)
			{
				var drivable = br.ReadStruct<DrivableScenerySection>();
				this.m_visibleSectionManager.DrivableScenerySections.Add(drivable.SectionNumber, drivable);
			}
		}

		private void ReadTrackSpecificSections(BinaryReader br, int size)
		{
			var count = size / TrackSpecificSection.SizeOf;

			for (int i = 0; i < count; ++i)
			{
				var specific = br.ReadStruct<TrackSpecificSection>();
				this.m_visibleSectionManager.TrackSpecificSections.Add((ushort)specific.TrackID, specific);
			}
		}

		private void ReadLoadingSections(BinaryReader br, int size)
		{
			var count = size / Shared.MapStream.LoadingSection.SizeOf;
			this.m_visibleSectionManager.LoadingSections.Capacity = count;

			for (int i = 0; i < count; ++i)
			{
				var loadingSection = br.ReadStruct<Shared.MapStream.LoadingSection>();
				this.m_visibleSectionManager.LoadingSections.Add(loadingSection);
			}
		}
	}
}
