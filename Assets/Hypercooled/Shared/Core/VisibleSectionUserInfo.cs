using CoreExtensions.IO;
using Hypercooled.Managed;
using Hypercooled.Utils;
using System.IO;
using System.Collections.Generic;

namespace Hypercooled.Shared.Core
{
	public class VisibleSectionUserInfo
	{
		public struct SerializationHeader
		{
			/* 0x00 */ public Game GameType;
			/* 0x04 */ public int SectionNumber;

			/* 0x08 */ public int SceneryObjectCount;
			/* 0x0C */ public int LightObjectCount;
			/* 0x10 */ public int FlareObjectCount;
			/* 0x14 */ public int TriggerObjectCount;
			/* 0x18 */ public int EmitterObjectCount;
			/* 0x1C */ public int StringListCount;

			/* 0x20 */ public int SceneryObjectOffset;
			/* 0x24 */ public int LightObjectOffset;
			/* 0x28 */ public int FlareObjectOffset;
			/* 0x2C */ public int TriggerObjectOffset;
			/* 0x30 */ public int EmitterObjectOffset;
			/* 0x34 */ public int StringListOffset;

			public const int SizeOf = 0x38;
		}

		public Game GameType { get; }
		public ushort SectionNumber { get; }

		public List<SceneryObject> SceneryObjects { get; }
		public List<LightObject> LightObjects { get; }
		public List<FlareObject> FlareObjects { get; }
		public List<TriggerObject> TriggerObjects { get; }
		public List<EmitterObject> EmitterObjects { get; }

		public VisibleSectionUserInfo(Game game, ushort sectionNumber)
		{
			this.GameType = game;
			this.SectionNumber = sectionNumber;
			this.SceneryObjects = new List<SceneryObject>();
			this.LightObjects = new List<LightObject>();
			this.FlareObjects = new List<FlareObject>();
			this.TriggerObjects = new List<TriggerObject>();
			this.EmitterObjects = new List<EmitterObject>();
		}

		public static void Serialize(VisibleSectionUserInfo userInfo, string path, bool overwrite)
		{
			if (!overwrite && File.Exists(path)) return;

			byte[] data = null;

			using (var ms = new MemoryStream(0x10000))
			using (var bw = new BinaryWriter(ms))
			{
				Serialize(userInfo, bw);
				data = ms.ToArray();
			}

			File.WriteAllBytes(path, data);
		}

		public static void Serialize(VisibleSectionUserInfo userInfo, BinaryWriter bw)
		{
			bw.Write((uint)BinBlockID.VisibleSectionUserInfo);
			bw.Write(-1);

			var start = bw.BaseStream.Position;

			bw.WriteBytes(0, SerializationHeader.SizeOf);

			var header = new SerializationHeader()
			{
				GameType = userInfo.GameType,
				SectionNumber = userInfo.SectionNumber,
				SceneryObjectCount = userInfo.SceneryObjects.Count,
				LightObjectCount = userInfo.LightObjects.Count,
				FlareObjectCount = userInfo.FlareObjects.Count,
				TriggerObjectCount = userInfo.TriggerObjects.Count,
				EmitterObjectCount = userInfo.EmitterObjects.Count,
				SceneryObjectOffset = SerializationHeader.SizeOf,
			};

			for (int i = 0; i < userInfo.SceneryObjects.Count; ++i)
			{
				userInfo.SceneryObjects[i].Serialize(bw);
			}

			header.LightObjectOffset = (int)(bw.BaseStream.Position - start);

			for (int i = 0; i < userInfo.LightObjects.Count; ++i)
			{
				userInfo.LightObjects[i].Serialize(bw);
			}

			header.FlareObjectOffset = (int)(bw.BaseStream.Position - start);

			for (int i = 0; i < userInfo.FlareObjects.Count; ++i)
			{
				userInfo.FlareObjects[i].Serialize(bw);
			}

			header.TriggerObjectOffset = (int)(bw.BaseStream.Position - start);

			for (int i = 0; i < userInfo.TriggerObjects.Count; ++i)
			{
				userInfo.TriggerObjects[i].Serialize(bw);
			}

			header.EmitterObjectOffset = (int)(bw.BaseStream.Position - start);

			for (int i = 0; i < userInfo.EmitterObjects.Count; ++i)
			{
				userInfo.EmitterObjects[i].Serialize(bw);
			}

			header.StringListOffset = (int)(bw.BaseStream.Position - start);
			header.StringListCount = WriteAllSceneryStrings(userInfo, bw);

			var end = bw.BaseStream.Position;

			bw.BaseStream.Position = start - 4;
			bw.Write((uint)(end - start));
			bw.WriteUnmanaged(header);
			bw.BaseStream.Position = end;
		}

		public static VisibleSectionUserInfo Deserialize(string path)
		{
			if (!File.Exists(path)) return null;
			var buffer = File.ReadAllBytes(path);

			using (var br = new BinaryReader(new MemoryStream(buffer)))
			{
				var id = (BinBlockID)br.ReadUInt32();
				var size = br.ReadInt32();

				if (id != BinBlockID.VisibleSectionUserInfo)
				{
					return null;
				}
				else
				{
					return Deserialize(br);
				}
			}
		}

		public static VisibleSectionUserInfo Deserialize(BinaryReader br)
		{
			var start = br.BaseStream.Position;

			var header = br.ReadUnmanaged<SerializationHeader>();
			var userInfo = new VisibleSectionUserInfo(header.GameType, (ushort)header.SectionNumber);

			br.BaseStream.Position = header.SceneryObjectOffset + start;

			for (int i = 0; i < header.SceneryObjectCount; ++i)
			{
				var obj = SceneryObject.GetSceneryObject(userInfo.GameType);
				obj.Deserialize(br);
				userInfo.SceneryObjects.Add(obj);
			}

			br.BaseStream.Position = header.LightObjectOffset + start;

			for (int i = 0; i < header.LightObjectCount; ++i)
			{
				var obj = new LightObject();
				obj.Deserialize(br);
				userInfo.LightObjects.Add(obj);
			}

			br.BaseStream.Position = header.FlareObjectOffset + start;

			for (int i = 0; i < header.FlareObjectCount; ++i)
			{
				var obj = new FlareObject();
				obj.Deserialize(br);
				userInfo.FlareObjects.Add(obj);
			}

			br.BaseStream.Position = header.TriggerObjectOffset + start;

			for (int i = 0; i < header.TriggerObjectCount; ++i)
			{
				var obj = new TriggerObject();
				obj.Deserialize(br);
				userInfo.TriggerObjects.Add(obj);
			}

			br.BaseStream.Position = header.EmitterObjectOffset + start;

			for (int i = 0; i < header.EmitterObjectCount; ++i)
			{
				var obj = new EmitterObject();
				obj.Deserialize(br);
				userInfo.EmitterObjects.Add(obj);
			}

			br.BaseStream.Position = header.StringListOffset + start;

			for (int i = 0; i < header.StringListCount; ++i)
			{
				br.ReadNullTermUTF8().BinHash();
			}

			return userInfo;
		}

		private static int WriteAllSceneryStrings(VisibleSectionUserInfo userInfo, BinaryWriter bw)
		{
			if (userInfo.SceneryObjects.Count == 0) return 0;

			var stringList = new HashSet<string>();

			foreach (var sceneryObject in userInfo.SceneryObjects)
			{
				foreach (var sceneryGroup in sceneryObject.SceneryGroups)
				{
					stringList.Add(sceneryGroup.BinString());
				}
			}

			foreach (var stringValue in stringList)
			{
				bw.WriteNullTermUTF8(stringValue);
			}

			bw.FillBufferPow2(0x10);
			return stringList.Count;
		}
	}
}
