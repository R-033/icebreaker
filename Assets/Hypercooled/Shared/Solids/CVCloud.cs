using CoreExtensions.IO;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Solids
{
	public class CVCloud
	{
		private Vector3[] m_points;

		public Vector3[] Points
		{
			get => this.m_points;
			set => this.m_points = value ?? new Vector3[0];
		}

		public CVCloud()
		{
			this.m_points = new Vector3[0];
		}

		public void Read(BinaryReader br)
		{
			var count = br.ReadInt32();
			this.m_points = new Vector3[count];
			br.BaseStream.Position += 0x0C;

			for (int i = 0; i < count; ++i)
			{
				this.m_points[i] = br.ReadUnmanaged<Vector3>();
				br.BaseStream.Position += 4;
			}
		}

		public void Write(BinaryWriter bw)
		{
			bw.Write(this.m_points.Length);
			bw.WriteBytes(0, 0x0C);

			for (int i = 0; i < this.m_points.Length; ++i)
			{
				bw.WriteUnmanaged(this.m_points[i]);
				bw.Write(0);
			}
		}
	}
}
