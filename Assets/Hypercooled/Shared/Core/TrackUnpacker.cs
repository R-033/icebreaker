using Hypercooled.Managed;
using Hypercooled.Shared.Interfaces;
using Hypercooled.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public abstract class TrackUnpacker
	{
		protected struct StreamingSection
		{
			public ushort SectionNumber { get; set; }
			public int Offset { get; set; }
			public int Size { get; set; }

			public StreamingSection(ushort sectionNumber, int offset, int size)
			{
				this.SectionNumber = sectionNumber;
				this.Offset = offset;
				this.Size = size;
			}
		}

		protected readonly string m_folder;
		protected readonly string m_global;
		protected readonly string m_lxry;
		protected readonly string m_stream;

		protected readonly string m_pathGlobal;
		protected readonly string m_pathLocation;
		protected readonly string m_pathStream;

		protected readonly ProjectHeader m_header;

		protected readonly Dictionary<ushort, List<string>> m_currentSectionPaths;
		protected readonly Dictionary<ushort, VisibleSectionUserInfo> m_userInfos;
		protected readonly List<StreamingSection> m_streamingSections;

		public TrackUnpacker(Game gameType, string folder, string globalB, string lxry)
		{
			IOHelper.AssertFileExists(globalB);
			IOHelper.AssertFileExists(lxry);

			this.m_folder = folder;
			this.m_global = globalB;
			this.m_lxry = lxry;
			this.m_stream = this.GetStreamName();

			IOHelper.AssertFileExists(this.m_stream);

			this.m_header = new ProjectHeader()
			{
				GameType = gameType,
			};

			this.m_pathGlobal = Path.Combine(this.m_folder, Namings.GlobalFolder);
			this.m_pathLocation = Path.Combine(this.m_folder, Namings.LocationFolder);
			this.m_pathStream = Path.Combine(this.m_folder, Namings.StreamFolder);

			this.m_userInfos = new Dictionary<ushort, VisibleSectionUserInfo>();
			this.m_currentSectionPaths = new Dictionary<ushort, List<string>>();
			this.m_streamingSections = new List<StreamingSection>();
		}

		protected abstract void InternalUnpackFixup();
		protected abstract void RegisterLocationParsers(BlockReader reader);
		protected abstract void RegisterStreamParsers(BlockReader reader);
		protected abstract void ResolveLocationParser(IParser iparser);
		protected abstract void ResolveStreamParsers(List<IParser> iparsers, ushort sectionNumber);

		private string GetStreamName()
		{
			var directory = Path.GetDirectoryName(this.m_lxry);
			var filename = Path.GetFileName(this.m_lxry);
			return Path.Combine(directory, "STREAM" + filename);
		}
		private BlockReader.ReaderOptions GetSectionOptions(BinaryReader br, StreamingSection section)
		{
			return new BlockReader.ReaderOptions()
			{
				Reader = br,
				ReaderOffset = section.Offset,
				SizeToRead = section.Size,
				AbsoluteOffset = 0,
			};
		}

		private void UnpackGlobalFile()
		{
			var path = Path.Combine(this.m_pathGlobal, Namings.GlobalFile);
			File.Copy(this.m_global, path, true);
			this.m_header.Global.Add(Namings.GlobalFile);
		}
		private void UnpackLocationFile()
		{
			var reader = new BlockReader();
			this.RegisterLocationParsers(reader);

			using (var br = IOHelper.GetBinaryReader(this.m_lxry, true))
			{
				var options = BlockReader.ReaderOptions.GetDefault(br);
				reader.LoadChunks(options);
			}

			foreach (var iparser in reader.Parsers)
			{
				this.ResolveLocationParser(iparser);
			}
		}
		private void UnpackStreamFile()
		{
			if (this.m_streamingSections.Count == 0) return;

			var reader = new BlockReader();
			this.RegisterStreamParsers(reader);

			using (var br = IOHelper.GetBinaryReader(this.m_stream, false))
			{
				foreach (var section in this.m_streamingSections)
				{
					reader.Parsers.Clear();

					var options = this.GetSectionOptions(br, section);
					reader.LoadChunks(options);

					this.ResolveStreamParsers(reader.Parsers, section.SectionNumber);
				}
			}

			Debug.Log($"Exported {this.m_streamingSections.Count} streaming sections.");
		}

		private void SerializeUserInfos()
		{
			foreach (var userInfo in this.m_userInfos)
			{
				var username = Namings.UserInfo + Namings.BinExt;
				var path = Path.Combine(this.m_pathStream, userInfo.Key.ToString(), username);
				VisibleSectionUserInfo.Serialize(userInfo.Value, path, true);
			}
		}
		private void SerializeAssetLists()
		{
			foreach (var assetList in this.m_currentSectionPaths)
			{
				var name = Namings.Assets + Namings.HyperExt;
				var path = Path.Combine(this.m_pathStream, assetList.Key.ToString(), name);

				File.WriteAllLines(path, assetList.Value);
			}
		}
		private void SerializeHeader()
		{
			foreach (var section in this.m_currentSectionPaths.Keys)
			{
				this.m_header.Sections.Add(section);
			}

			var path = Path.Combine(this.m_folder, Namings.MainProj + Namings.HyperExt);
			var json = JsonConvert.SerializeObject(this.m_header, Formatting.Indented);

			File.WriteAllText(path, json);
		}

		protected void AddAssetName(ushort sectionNumber, string asset)
		{
			if (!this.m_currentSectionPaths.TryGetValue(sectionNumber, out var list))
			{
				list = new List<string>();
				this.m_currentSectionPaths.Add(sectionNumber, list);

				var path = Path.Combine(this.m_pathStream, sectionNumber.ToString());
				Directory.CreateDirectory(path);
			}

			list.Add(asset);
		}
		protected VisibleSectionUserInfo CreateNewUserInfo(Game gameType, ushort sectionNumber)
		{
			var userInfo = new VisibleSectionUserInfo(gameType, sectionNumber);

			this.m_userInfos.Add(sectionNumber, userInfo);
			this.AddAssetName(sectionNumber, Namings.UserInfo + Namings.BinExt);

			return userInfo;
		}

		public void Initialize()
		{
			Directory.CreateDirectory(this.m_folder);
			Directory.CreateDirectory(this.m_pathGlobal);
			Directory.CreateDirectory(this.m_pathLocation);
			Directory.CreateDirectory(this.m_pathStream);
		}
		public async Task Unpack()
		{
			await Task.Run(() =>
			{
				Debug.Log("Starting unpacking...");

				this.UnpackGlobalFile();
				this.UnpackLocationFile();
				this.UnpackStreamFile();

				Debug.Log("Finished unpacking. Starting fixups...");

				this.InternalUnpackFixup();

				Debug.Log("Finished fixing. Starting serializing...");

				this.SerializeUserInfos();
				this.SerializeAssetLists();
				this.SerializeHeader();

				Debug.Log("Finished serializing. Finished unpacking.");
			});
		}
	}
}
