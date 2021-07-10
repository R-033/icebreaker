using System.Runtime.InteropServices;

namespace Hypercooled.Shared.World
{
	public class UppleUData
	{
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct UData
		{
			public uint Tag;
			public int Flags;
			public int NumDatas;
			public uint Offset;

			public const int SizeOf = 0x10;
		}

		private ushort m_tagPartLower;
		private ushort m_tagPartUpper;
		private VVResolvable[] m_resolvables;

		public UppleUTag Tag => this.InternalCreateTag();
		public ushort Index => this.m_tagPartLower;
		public uint Offset { get; set; }

		public int Size { get; set; }       // : 24
		public int Section { get; set; }    // : 5
		public bool Allocated { get; set; } // : 1
		public bool Embedded { get; set; }  // : 1
		public bool Indexed { get; set; }   // : 1

		public VVResolvable[] Resolvables
		{
			get => this.m_resolvables;
			set => this.m_resolvables = value ?? VVResolvable.Empty;
		}

		public UppleUData()
		{
			this.m_resolvables = VVResolvable.Empty;
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

		public int GetMemorySize()
		{
			int size = 0;

			if (this.m_resolvables.Length > 0)
			{
				if (this.m_resolvables[0].DynamicSize)
				{
					for (int i = 0; i < this.m_resolvables.Length; ++i)
					{
						size += this.m_resolvables[i].SizeOfThis;
					}
				}
				else
				{
					size = this.m_resolvables.Length * this.m_resolvables[0].SizeOfThis;
				}
			}

			return size;
		}

		public static UppleUData FromUData(UData udata)
		{
			return new UppleUData()
			{
				m_tagPartLower = (ushort)(udata.Tag),
				m_tagPartUpper = (ushort)(udata.Tag >> 16),
				Offset = udata.Offset,
				Size = udata.Flags >> 8,
				Section = (udata.Flags >> 3) & 0x1F,
				Allocated = ((udata.Flags >> 2) & 1) != 0,
				Embedded = ((udata.Flags >> 1) & 1) != 0,
				Indexed = (udata.Flags & 1) != 0,
				m_resolvables = new VVResolvable[udata.NumDatas],
			};
		}

		public static UData ToUData(UppleUData udata)
		{
			int flags = udata.Indexed ? 1 : 0;
			flags |= (udata.Embedded ? 1 : 0) << 1;
			flags |= (udata.Allocated ? 1 : 0) << 2;
			flags |= udata.Section << 3;
			flags |= udata.GetMemorySize() << 8;

			return new UData()
			{
				Tag = (uint)(udata.m_tagPartUpper << 16) | (udata.m_tagPartLower),
				Flags = flags,
				NumDatas = udata.Tag == UppleUTag.Name ? 0 : udata.m_resolvables.Length, // :D
				Offset = udata.Offset,
			};
		}

		public override string ToString() => $"Tag: {this.Tag} | Objects: {this.m_resolvables.Length}";
	}
}
