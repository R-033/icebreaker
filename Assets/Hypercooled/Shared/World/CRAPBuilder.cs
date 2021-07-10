using System.Collections.Generic;

namespace Hypercooled.Shared.World
{
	public class CRAPBuilder
	{
		public class Group
		{
			public UppleUTag Tag { get; }
			public int Index { get; }
			public List<Group> Groups { get; }
			public List<Data> Datas { get; }
			public bool Indexed => this.Index != -1;

			public Group(UppleUTag tag, int index)
			{
				this.Tag = tag;
				this.Index = index;
				this.Groups = new List<Group>();
				this.Datas = new List<Data>();
			}

			public Group LocateGroup(UppleUTag tag)
			{
				return this.Groups.Find((group) => group.Tag == tag);
			}
			public Data LocateData(UppleUTag tag)
			{
				return this.Datas.Find((data) => data.Tag == tag);
			}
			public UppleUTag CreateTag()
			{
				if (!this.Indexed) return this.Tag; 
				else return (UppleUTag)(((uint)this.Tag << 0x10) | (ushort)this.Index);
			}
		}

		public class Data
		{
			public UppleUTag Tag { get; }
			public int Index { get; }
			public List<VVResolvable> Resolvables { get; }
			public bool Indexed => this.Index != -1;

			public Data(UppleUTag tag, int index)
			{
				this.Tag = tag;
				this.Index = index;
				this.Resolvables = new List<VVResolvable>();
			}

			public UppleUTag CreateTag()
			{
				if (!this.Indexed) return this.Tag;
				else return (UppleUTag)(((uint)this.Tag << 0x10) | (ushort)this.Index);
			}
		}

		public class TagPath
		{
			public UppleUTag Tag { get; }
			public TagPath Next { get; set; }

			public TagPath(UppleUTag tag)
			{
				this.Tag = tag;
			}
		}

		private readonly Group m_root;

		public CRAPBuilder()
		{
			this.m_root = new Group(UppleUTag.Main, -1);
		}

		private UppleUGroup CreateUppleUGroup(Group group)
		{
			var result = new UppleUGroup()
			{
				Indexed = group.Indexed,
				Embedded = true,
				Allocated = false,
			};

			result.AssignTag(group.CreateTag());

			if (group.Groups.Count > 0)
			{
				group.Groups.Sort((x, y) => x.CreateTag().CompareTo(y.CreateTag()));

				result.GroupSorted = true;
				result.Groups = new UppleUGroup[group.Groups.Count];

				for (int i = 0; i < result.Groups.Length; ++i)
				{
					result.Groups[i] = this.CreateUppleUGroup(group.Groups[i]);
				}
			}
			if (group.Datas.Count > 0)
			{
				group.Datas.Sort((x, y) => x.CreateTag().CompareTo(y.CreateTag()));

				result.DataSorted = true;
				result.Datas = new UppleUData[group.Datas.Count];

				for (int i = 0; i < result.Datas.Length; ++i)
				{
					result.Datas[i] = this.CreateUppleUData(group.Datas[i]);
				}
			}

			return result;
		}

		private UppleUData CreateUppleUData(Data data)
		{
			var result = new UppleUData()
			{
				Indexed = data.Indexed,
				Embedded = true,
				Allocated = false,
				Resolvables = data.Resolvables.ToArray(),
			};

			result.AssignTag(data.CreateTag());
			return result;
		}

		public Group AddGroup(TagPath path, int index = -1)
		{
			var group = this.m_root;

			while (!(path.Next is null))
			{
				group = group.LocateGroup(path.Tag);
				if (group is null) return null; // bruh
				path = path.Next;
			}

			var result = new Group(path.Tag, index);
			group.Groups.Add(result);
			return result;
		}

		public Data AddData(TagPath path, int index = -1)
		{
			var group = this.m_root;

			while (!(path.Next is null))
			{
				group = group.LocateGroup(path.Tag);
				if (group is null) return null; // bruh
				path = path.Next;
			}

			var result = new Data(path.Tag, index);
			group.Datas.Add(result);
			return result;
		}

		public void PushResolvable(TagPath path, VVResolvable resolvable)
		{
			var group = this.m_root;

			while (!(path.Next is null))
			{
				group = group.LocateGroup(path.Tag);
				if (group is null) return; // really?
				path = path.Next;
			}

			var data = group.LocateData(path.Tag);

			if (data is null) return;
			if (resolvable.Tag != path.Tag) return; // but why

			data.Resolvables.Add(resolvable);
		}

		public void PushResolvables(TagPath path, IEnumerable<VVResolvable> resolvables)
		{
			var group = this.m_root;

			while (!(path.Next is null))
			{
				group = group.LocateGroup(path.Tag);
				if (group is null) return; // uhhh
				path = path.Next;
			}

			var data = group.LocateData(path.Tag);

			if (data is null) return;
			data.Resolvables.AddRange(resolvables);
		}

		public UppleUGroup GetUppleU()
		{
			return this.CreateUppleUGroup(this.m_root);
		}
	}
}
