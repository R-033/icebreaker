using System.Collections.Generic;

namespace Hypercooled.Carbon.MapStream
{
	public class ScenerySection : Shared.MapStream.ScenerySection
	{
		private ScenerySectionHeader m_header;
		private readonly List<SceneryInfo> m_sceneryInfos;
		private readonly List<SceneryInstance> m_sceneryInstances;
		private readonly List<SceneryTreeNode> m_sceneryTreeNodes;
		private readonly List<Shared.MapStream.SceneryOverrideHook> m_sceneryOverrideHooks;
		private readonly List<Shared.MapStream.PrecullerInfo> m_precullerInfos;
		private readonly List<LightTextureCollection> m_lightTextureCollections;

		public override ushort SectionNumber => (ushort)this.m_header.SectionNumber;

		public ScenerySectionHeader Header => this.m_header;
		public List<SceneryInfo> SceneryInfos => this.m_sceneryInfos;
		public List<SceneryInstance> SceneryInstances => this.m_sceneryInstances;
		public List<SceneryTreeNode> SceneryTreeNodes => this.m_sceneryTreeNodes;
		public List<Shared.MapStream.SceneryOverrideHook> SceneryOverrideHooks => this.m_sceneryOverrideHooks;
		public List<Shared.MapStream.PrecullerInfo> PrecullerInfos => this.m_precullerInfos;
		public List<LightTextureCollection> LightTextureCollections => this.m_lightTextureCollections;

		public ScenerySection()
		{
			this.m_sceneryInfos = new List<SceneryInfo>();
			this.m_sceneryInstances = new List<SceneryInstance>();
			this.m_sceneryTreeNodes = new List<SceneryTreeNode>();
			this.m_sceneryOverrideHooks = new List<Shared.MapStream.SceneryOverrideHook>();
			this.m_precullerInfos = new List<Shared.MapStream.PrecullerInfo>();
			this.m_lightTextureCollections = new List<LightTextureCollection>();
		}

		public void SetScenerySectionHeader(ScenerySectionHeader header) => this.m_header = header;
	}
}
