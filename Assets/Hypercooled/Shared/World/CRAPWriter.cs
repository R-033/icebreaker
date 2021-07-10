using CoreExtensions.IO;
using Hypercooled.Utils;
using System.IO;

namespace Hypercooled.Shared.World
{
	public class CRAPWriter
	{
		public UppleUGroup Root { get; private set; }

		public CRAPWriter()
		{
		}

		public void Write(BinaryWriter bw, UppleUGroup root)
		{
			if (root is null) return;
			if (bw is null) return;

			this.Root = root;
			this.InternalWriteCARP(bw);
		}

		private void InternalWriteCARP(BinaryWriter bw)
		{
			int size = this.PrecalculateHeaderSize();
			var start = bw.BaseStream.Position;

			bw.WriteBytes(0xAA, size);

			var middle = bw.BaseStream.Position;

			this.InternalWriteAllGroups(bw, start, middle);
			this.InternalAlignWriter(bw, start);
		}

		private int PrecalculateHeaderSize()
		{
			return GenericHelper.CeilToTheNearestPow2(AddAllGroupSizes(this.Root), 0x100);

			int AddAllGroupSizes(UppleUGroup ugroup)
			{
				int result = UppleUGroup.UGroup.SizeOf;

				foreach (var group in ugroup.Groups)
				{
					result += AddAllGroupSizes(group);
				}

				return result + ugroup.Datas.Length * UppleUData.UData.SizeOf;
			}
		}

		private void InternalAlignWriter(BinaryWriter bw, long offset)
		{
			var difference = bw.BaseStream.Position - offset;

			var align = GenericHelper.CeilToTheNearestPow2(difference, 0x100);
			bw.WriteBytes(0xAA, (int)(align - difference));
		}

		private void InternalWriteAllGroups(BinaryWriter bw, long headerOffset, long dataOffset)
		{
			uint totalHeadersWritten = 1;

			bw.BaseStream.Position = headerOffset;
			WriteAllGroupsAndDatas(this.Root);

			void WriteAllGroupsAndDatas(UppleUGroup ugroup)
			{
				ugroup.Offset = totalHeadersWritten - (uint)((bw.BaseStream.Position - headerOffset) / UppleUGroup.UGroup.SizeOf); // pls work

				bw.WriteUnmanaged(UppleUGroup.ToUGroup(ugroup));

				var groupOffset = headerOffset + totalHeadersWritten * UppleUGroup.UGroup.SizeOf;
				var pointer = groupOffset + ugroup.Groups.Length * UppleUGroup.UGroup.SizeOf;

				totalHeadersWritten += (uint)ugroup.Groups.Length;
				totalHeadersWritten += (uint)ugroup.Datas.Length;

				for (int i = 0; i < ugroup.Datas.Length; ++i)
				{
					var udata = ugroup.Datas[i];
					bw.BaseStream.Position = pointer + i * UppleUData.UData.SizeOf;
					udata.Offset = (uint)(dataOffset - bw.BaseStream.Position);

					dataOffset = this.WriteUppleUData(bw, udata, dataOffset);
				}

				for (int i = 0; i < ugroup.Groups.Length; ++i)
				{
					bw.BaseStream.Position = groupOffset + i * UppleUGroup.UGroup.SizeOf;
					WriteAllGroupsAndDatas(ugroup.Groups[i]);
				}
			}
		}

		private long WriteUppleUData(BinaryWriter bw, UppleUData udata, long offset)
		{
			if (udata.Tag == UppleUTag.Name) // :D
			{
				bw.WriteUnmanaged(UppleUData.ToUData(udata));
				bw.BaseStream.Position = offset;

				if (udata.Resolvables.Length > 0)
				{
					udata.Resolvables[0].Write(bw);
				}
			}
			else
			{
				if (udata.Resolvables.Length == 0)
				{
					udata.Offset = 0;
				}

				bw.WriteUnmanaged(UppleUData.ToUData(udata));
				bw.BaseStream.Position = offset;

				for (int i = 0; i < udata.Resolvables.Length; ++i)
				{
					udata.Resolvables[i].Write(bw);
				}
			}

			var prepos = bw.BaseStream.Position;
			var result = GenericHelper.CeilToTheNearestPow2(bw.BaseStream.Position, 0x10);
			bw.WriteBytes(0xAA, (int)(result - prepos));
			return result;
		}
	}
}
