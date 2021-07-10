using CoreExtensions.IO;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypercooled.Shared.World
{
	public class CRAPReader
	{
		private readonly Dictionary<UppleUTag, Func<VVResolvable>> m_tagToResolvable;

		public UppleUGroup Root { get; private set; }

		public CRAPReader()
		{
			this.m_tagToResolvable = new Dictionary<UppleUTag, Func<VVResolvable>>();
			this.RegisterResolvable<VVNameString>(UppleUTag.Name);
		}

		public void RegisterResolvable<T>(UppleUTag tag) where T : VVResolvable, new()
		{
			if (!this.m_tagToResolvable.ContainsKey(tag))
			{
				this.m_tagToResolvable.Add(tag, () => new T());
			}
		}

		public void Read(BinaryReader br)
		{
			if (br is null) return;
			this.Root = this.ReadUppleUGroup(br);
		}

		private UppleUGroup ReadUppleUGroup(BinaryReader br)
		{
			var start = br.BaseStream.Position;
			var ugroup = UppleUGroup.FromUGroup(br.ReadUnmanaged<UppleUGroup.UGroup>());

			int gindex = 0;
			int dindex = 0;

			while (gindex < ugroup.Groups.Length)
			{
				br.BaseStream.Position = start + UppleUGroup.UGroup.SizeOf * (ugroup.Offset + gindex);
				ugroup.Groups[gindex++] = this.ReadUppleUGroup(br);
			}

			while (dindex < ugroup.Datas.Length)
			{
				br.BaseStream.Position = start + UppleUData.UData.SizeOf * (ugroup.Offset + gindex + dindex);
				ugroup.Datas[dindex++] = this.ReadUppleUData(br);
			}

			return ugroup;
		}

		private UppleUData ReadUppleUData(BinaryReader br)
		{
			var start = br.BaseStream.Position;
			var udata = UppleUData.FromUData(br.ReadUnmanaged<UppleUData.UData>());

			br.BaseStream.Position = start + udata.Offset;

			// For some reason name tags have size 0
			if (udata.Tag == UppleUTag.Name)
			{
				var ctor = this.m_tagToResolvable[UppleUTag.Name];
				var resolvable = ctor();
				resolvable.Read(br);
				udata.Resolvables = new VVResolvable[] { resolvable };
			}
			else if (this.m_tagToResolvable.TryGetValue(udata.Tag, out var ctor))
			{
				for (int i = 0; i < udata.Resolvables.Length; ++i)
				{
					var resolvable = ctor();
					resolvable.Read(br);
					udata.Resolvables[i] = resolvable;
				}
			}

			return udata;
		}
	}
}
