using Hypercooled.Managed;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public class ProjectHeader
	{
		public class Overlay
		{
			private static ushort[] ms_emptySections = new ushort[0];

			public string Name { get; set; }
			public string Description { get; set; }
			public Vector3 Position { get; set; }
			public Quaternion Rotation { get; set; }
			public ushort[] Sections { get; set; }

			public Overlay()
			{
				this.Name = String.Empty;
				this.Description = String.Empty;
				this.Sections = Overlay.ms_emptySections;
			}
		}

		[JsonIgnore()]
		public string Folder { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public Game GameType { get; set; }

		public bool UnityOptimized { get; set; }

		public List<string> Global { get; }
		public List<string> Extras { get; }
		public HashSet<ushort> Sections { get; }
		public List<Overlay> Overlays { get; }
		
		public ProjectHeader()
		{
			this.Folder = String.Empty;
			this.Global = new List<string>();
			this.Extras = new List<string>();
			this.Sections = new HashSet<ushort>();
			this.Overlays = new List<Overlay>();
		}

		public static ProjectHeader Deserialize(string path)
		{
			var text = File.ReadAllText(path);
			var json = JsonConvert.DeserializeObject<ProjectHeader>(text);

			json.Folder = Path.GetDirectoryName(path);
			return json;
		}
	}
}
