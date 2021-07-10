using CoreExtensions.IO;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Shared.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hypercooled.Utils
{
	public class BlockReader
	{
		public struct ReaderOptions
		{
			public bool IsDefault { get; }
			public BinaryReader Reader { get; set; }
			public long ReaderOffset { get; set; }
			public long SizeToRead { get; set; }
			public long AbsoluteOffset { get; set; }

			private ReaderOptions(BinaryReader br)
			{
				this.IsDefault = true;
				this.Reader = br;
				this.ReaderOffset = 0;
				this.SizeToRead = br.BaseStream.Length;
				this.AbsoluteOffset = 0;
			}

			public static ReaderOptions GetDefault(BinaryReader br) => new ReaderOptions(br);
		}

		private readonly Dictionary<uint, Func<IParser>> m_generators;

		public List<IParser> Parsers { get; }

		public BlockReader()
		{
			this.m_generators = new Dictionary<uint, Func<IParser>>();
			this.Parsers = new List<IParser>();
		}

		private IParser GetParserByID(uint id)
		{
			if (this.m_generators.TryGetValue(id, out var generator))
			{
				return generator();
			}
			return null;
		}

		public void RegisterParser(uint id, Func<IParser> parserGenerator)
		{
			this.m_generators[id] = parserGenerator;
		}

		private void InternalLoaderChunks(ReaderOptions options)
		{
			var br = options.Reader;
			var original = br.BaseStream.Position;

			br.BaseStream.Position = options.ReaderOffset;
			var max = options.ReaderOffset + options.SizeToRead;

			while (br.BaseStream.Position < max)
			{
				var currentOffset = br.BaseStream.Position;
				var info = br.ReadUnmanaged<BinBlock.Info>();
				var end = currentOffset + info.Size + 8;

				if (end > max) break;

				var parser = this.GetParserByID(info.ID);

				if (!(parser is null))
				{
					var abs = options.AbsoluteOffset + currentOffset;
					parser.Prepare(new BinBlock(info, abs));
					this.Parsers.Add(parser);
					parser.Disassemble(br);
				}

				br.BaseStream.Position = end;
			}

			br.BaseStream.Position = original;
		}

		public void LoadChunks(ReaderOptions options)
		{
			if (!options.IsDefault)
			{
				var br = options.Reader;

				if (options.ReaderOffset < 0 || options.ReaderOffset > br.BaseStream.Length) return;
				if (options.SizeToRead <= 0) return;
				if (options.ReaderOffset + options.SizeToRead > br.BaseStream.Length) return;
			}

			this.InternalLoaderChunks(options);
		}

		public async Task LoadChunksAsync(ReaderOptions options)
		{
			await Task.Run(() => this.LoadChunks(options));
		}
	}
}
