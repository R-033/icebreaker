using CoreExtensions.IO;
using Hypercooled.Utils;
using System.IO;
using System.Runtime.InteropServices;

namespace Hypercooled.Carbon.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DrivableScenerySectionInternal
	{
		public long Padding1;
		public int Padding2;
		public ushort SectionNumber;
		public byte MostVisibleSections;
		public byte MaxVisibleSections;
		public ushort NumVisibleSections;

		public const int SizeOf = 0x12;
	}

	public class DrivableScenerySection // 0x00034153
	{
		private ushort[] m_visibleSections;

		public ushort SectionNumber => this.SectionParams.SectionNumber;
		public DrivableScenerySectionInternal SectionParams { get; set; }
		public ushort[] VisibleSections
		{
			get => this.m_visibleSections;
			set => this.m_visibleSections = value ?? new ushort[0];
		}

		public void Read(BinaryReader br)
		{
			this.SectionParams = br.ReadUnmanaged<DrivableScenerySectionInternal>();
			this.m_visibleSections = new ushort[this.SectionParams.NumVisibleSections];

			var endPosition = br.BaseStream.Position + (this.SectionParams.MaxVisibleSections << 1);

			for (int i = 0; i < this.SectionParams.NumVisibleSections; ++i)
			{
				this.m_visibleSections[i] = br.ReadUInt16();
			}

			br.BaseStream.Position = endPosition;
			br.AlignReaderPow2(0x04);
		}

		public int SizeOf => DrivableScenerySectionInternal.SizeOf + this.m_visibleSections.Length * 0x02;
	}
}
