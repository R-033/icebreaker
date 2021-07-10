using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.World
{
	public class UppleUGroup
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct UGroup
		{
			public uint Tag;
			public int Flags;
			public int NumDatas;
			public uint Offset;

			public const int SizeOf = 0x10;
		}

		private static UppleUGroup[] ms_emptyGroups = new UppleUGroup[0];
		private static UppleUData[] ms_emptyDatas = new UppleUData[0];

		private ushort m_tagPartLower;
		private ushort m_tagPartUpper;
		private UppleUGroup[] m_groups;
		private UppleUData[] m_datas;

		public UppleUTag Tag => this.InternalCreateTag();
		public ushort Index => this.m_tagPartLower;
		public uint Offset { get; set; }

		public bool DataSorted { get; set; }
		public bool GroupSorted { get; set; }
		public bool Allocated { get; set; }
		public bool Embedded { get; set; }
		public bool Indexed { get; set; }

		public UppleUGroup[] Groups
		{
			get => this.m_groups;
			set => this.m_groups = value ?? ms_emptyGroups;
		}
		public UppleUData[] Datas
		{
			get => this.m_datas;
			set => this.m_datas = value ?? ms_emptyDatas;
		}

		public UppleUGroup()
		{
			this.m_groups = ms_emptyGroups;
			this.m_datas = ms_emptyDatas;
		}

		private UppleUTag InternalCreateTag()
		{
			return this.Indexed ? (UppleUTag)this.m_tagPartUpper : (UppleUTag)((this.m_tagPartUpper << 16) | this.m_tagPartLower);
		}

		public void AssignTag(UppleUTag tag)
		{
			this.m_tagPartLower = (ushort)((uint)tag & 0xFFFF);
			this.m_tagPartUpper = (ushort)((uint)tag >> 16);
		}

		public UppleUGroup LocateGroup(Predicate<UppleUGroup> predicate)
		{
			if (predicate is null || this.Groups is null)
			{
				return null;
			}

			for (int i = 0; i < this.Groups.Length; ++i)
			{
				if (predicate(this.Groups[i]))
				{
					return this.Groups[i];
				}
			}

			return null;
		}

		public UppleUData LocateData(Predicate<UppleUData> predicate)
		{
			if (predicate is null || this.Datas is null)
			{
				return null;
			}

			for (int i = 0; i < this.Datas.Length; ++i)
			{
				if (predicate(this.Datas[i]))
				{
					return this.Datas[i];
				}
			}

			return null;
		}

		public IEnumerable<UppleUGroup> LocateGroups(Predicate<UppleUGroup> predicate)
		{
			if (predicate is null || this.Groups is null)
			{
				yield break;
			}

			foreach (var ugroup in this.Groups)
			{
				if (predicate(ugroup))
				{
					yield return ugroup;
				}
			}
		}

		public IEnumerable<UppleUData> LocateDatas(Predicate<UppleUData> predicate)
		{
			if (predicate is null || this.Datas is null)
			{
				yield break;
			}

			foreach (var udata in this.Datas)
			{
				if (predicate(udata))
				{
					yield return udata;
				}
			}
		}

		public static UppleUGroup FromUGroup(UGroup ugroup)
		{
			var group = new UppleUGroup()
			{
				m_tagPartLower = (ushort)(ugroup.Tag),
				m_tagPartUpper = (ushort)(ugroup.Tag >> 16),
				Offset = ugroup.Offset,
				DataSorted = ((ugroup.Flags >> 4) & 1) != 0,
				GroupSorted = ((ugroup.Flags >> 3) & 1) != 0,
				Allocated = ((ugroup.Flags >> 2) & 1) != 0,
				Embedded = ((ugroup.Flags >> 1) & 1) != 0,
				Indexed = (ugroup.Flags & 1) != 0,
			};

			group.m_groups = new UppleUGroup[ugroup.Flags >> 5];
			group.m_datas = new UppleUData[ugroup.NumDatas];

			return group;
		}

		public static UGroup ToUGroup(UppleUGroup ugroup)
		{
			int flags = ugroup.Indexed ? 1 : 0;
			flags |= (ugroup.Embedded ? 1 : 0) << 1;
			flags |= (ugroup.Allocated ? 1 : 0) << 2;
			flags |= (ugroup.GroupSorted ? 1 : 0) << 3;
			flags |= (ugroup.DataSorted ? 1 : 0) << 4;
			flags |= ugroup.Groups.Length << 5;

			return new UGroup()
			{
				Tag = (uint)(ugroup.m_tagPartUpper << 16) | ugroup.m_tagPartLower,
				Flags = flags,
				NumDatas = ugroup.Datas.Length,
				Offset = ugroup.Offset,
			};
		}

		public override string ToString()
		{
			return $"Tag: {this.Tag} | Groups: {this.Groups.Length} | Datas: {this.Datas.Length}";
		}
	}
}
