using CoreExtensions.IO;
using Hypercooled.Shared.Structures;
using Hypercooled.Shared.World;
using Hypercooled.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hypercooled.Carbon.Core
{
	public class TopologyManager : Shared.Core.TopologyManager
	{
		private UppleUHeader m_header;

		public override ushort SectionNumber => (ushort)this.m_header.SectionNumber;

		public override void Load(BinaryReader br)
		{
			this.ParseTree(br);
		}
		public override void Save(BinaryWriter bw)
		{
			this.BuildTree(bw);
		}

		public override void Generate(GameObject gameObject)
		{

		}

		public override void FromGroupNames(List<string> groups)
		{
			for (int i = 0; i < this.Objects.Count; ++i)
			{
				var topology = this.Objects[i] as TopologyObject;
				var index = topology.Instance.GetGroupNumber();

				topology.CollectionName = index < groups.Count ? groups[index] : i.ToString();
				topology.Instance.SceneryBarrierGroup = topology.CollectionName.BinHash();
				topology.Instance.UsesBarrierHash = true;
			}
		}
		public override void FromGroupIndices(Dictionary<string, int> groups)
		{
			for (int i = 0; i < this.Objects.Count; ++i)
			{
				var topology = this.Objects[i] as TopologyObject;

				if (!groups.TryGetValue(topology.CollectionName, out int index))
				{
					index = 0;
				}

				topology.CollectionName = String.Empty;
				topology.Instance.SetIndexAndGroup((ushort)i, (ushort)index);
				topology.Instance.UsesBarrierHash = false;
			}
		}

		private void ParseTree(BinaryReader br)
		{
			var info = br.ReadUnmanaged<BinBlock.Info>();

			if ((BinBlockID)info.ID != BinBlockID.WCollisionAssets || info.Size == 0)
			{
				return; // why are we here in the first place then?
			}

			br.AlignReaderPow2(0x10);

			this.m_header = br.ReadUnmanaged<UppleUHeader>();

			var reader = new CRAPReader();

			reader.RegisterResolvable<VVNameString>(UppleUTag.Name);
			reader.RegisterResolvable<VVCollisionArticle>(UppleUTag.WCollisionArticle);
			reader.RegisterResolvable<VVCollisionInstance>(UppleUTag.WCollisionInstance);

			reader.Read(br);

			var ugroup = reader.Root.LocateGroup((group) => group.Tag == UppleUTag.Article);
			if (ugroup is null) return; // welp, lol

			foreach (var nameData in ugroup.LocateDatas((data) => data.Tag == UppleUTag.Name))
			{
				if (nameData is null || nameData.Resolvables.Length == 0) continue; // why?

				(nameData.Resolvables[0] as VVNameString).Name.BinHash();
			}

			var instances = ugroup.LocateData((data) => data.Tag == UppleUTag.WCollisionInstance);

			for (int i = 0; i < instances.Resolvables.Length; ++i)
			{
				var instance = instances.Resolvables[i] as VVCollisionInstance;
				var index = instance.UsesBarrierHash ? i : instance.GetIndexNumber();

				var assetDat = ugroup.LocateData((group) => group.Tag == UppleUTag.WCollisionArticle && group.Index == index);

				if (assetDat is null || assetDat.Resolvables.Length == 0) continue; // hmm

				var article = assetDat.Resolvables[0] as VVCollisionArticle;
				this.Objects.Add(new TopologyObject(article, instance));
			}
		}
		private void BuildTree(BinaryWriter bw)
		{
			// Write ID and temporary size
			bw.Write((uint)BinBlockID.WCollisionAssets);
			bw.Write(-1);

			// Save start position
			var start = bw.BaseStream.Position;

			// Align writer because yes
			bw.AlignWriterPow2(0x10);

			// Save position of the header
			var middle = bw.BaseStream.Position;

			// Write temporary space for header
			bw.WriteBytes(0, UppleUHeader.SizeOf);

			// Initialize new builder
			var builder = new CRAPBuilder();

			// Add Article ugroup
			builder.AddGroup(new CRAPBuilder.TagPath(UppleUTag.Article));

			// Add Name resolvable for this section number b/c reasons
			var name = builder.AddData(new CRAPBuilder.TagPath(UppleUTag.Article)
			{
				Next = new CRAPBuilder.TagPath(UppleUTag.Name),
			});

			// Push section number as a string
			name.Resolvables.Add(new VVNameString()
			{
				Name = this.SectionNumber.ToString(),
			});

			// Write all collection names if needed
			if (this.DoWeWriteTopologyNames())
			{
				for (int i = 0; i < this.Objects.Count; ++i)
				{
					var collection = builder.AddData(new CRAPBuilder.TagPath(UppleUTag.Article)
					{
						Next = new CRAPBuilder.TagPath(UppleUTag.Name),
					});

					collection.Resolvables.Add(new VVNameString()
					{
						Name = this.Objects[i].CollectionName,
					});
				}
			}

			// Initialize instance list
			var instances = new VVResolvable[this.Objects.Count];

			// For each topology, add its instance and article
			for (int i = 0; i < this.Objects.Count; ++i)
			{
				// Get topology by current index
				var topology = this.Objects[i] as TopologyObject;

				// Get instance into a variable
				var instance = topology.Instance;

				// Set currently processing instance
				instances[i] = instance;

				// Add new collision article
				var artData = builder.AddData(new CRAPBuilder.TagPath(UppleUTag.Article)
				{
					Next = new CRAPBuilder.TagPath(UppleUTag.WCollisionArticle),
				}, i);

				// Push article from current topology
				artData.Resolvables.Add(topology.Article);
			}

			// Add instance data
			var insData = builder.AddData(new CRAPBuilder.TagPath(UppleUTag.Article)
			{
				Next = new CRAPBuilder.TagPath(UppleUTag.WCollisionInstance),
			}, 0);

			// Use add range on array of instances
			insData.Resolvables.AddRange(instances);

			// Initialize writer
			var writer = new CRAPWriter();

			// Finally, write all generated stuff
			writer.Write(bw, builder.GetUppleU());

			// Save end position
			var end = bw.BaseStream.Position;

			// Go back to the header position
			bw.BaseStream.Position = middle;

			// Set new header size
			this.m_header.Size = (int)(end - middle - UppleUHeader.SizeOf);

			// Write updated header
			bw.WriteUnmanaged(this.m_header);

			// Finally, write correct block size and go to the end
			bw.BaseStream.Position = start - 4;
			bw.Write((int)(end - start));
			bw.BaseStream.Position = end;
		}
	}
}
