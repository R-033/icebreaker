using System.Collections.Generic;

namespace Hypercooled.Shared.Core
{
	public class AnimObject
	{
		public string CollectionName { get; set; }
		public uint Key { get; set; }

		public int FramesPerSecond { get; set; }
		public int NumberOfFrames { get; set; }
		public int TimeBase { get; set; }
		public List<string> Frames { get; set; }

		public AnimObject()
		{
			this.Frames = new List<string>();
		}
	}
}
