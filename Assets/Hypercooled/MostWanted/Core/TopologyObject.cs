using Hypercooled.Shared.World;
using Hypercooled.Utils;
using UnityEngine;

namespace Hypercooled.MostWanted.Core
{
	public class TopologyObject : Shared.Core.TopologyObject
	{
		private VVCollisionArticle m_article;
		private VVCollisionInstance m_instance;

		public override bool AllowsMovement => true;
		public VVCollisionArticle Article => this.m_article;
		public VVCollisionInstance Instance => this.m_instance;

		public TopologyObject()
		{
			this.m_article = new VVCollisionArticle();
			this.m_instance = new VVCollisionInstance();
		}

		public TopologyObject(VVCollisionArticle article, VVCollisionInstance instance)
		{
			this.m_article = article;
			this.m_instance = instance;

			if (this.m_instance.UsesBarrierHash)
			{
				this.CollectionName = this.m_instance.SceneryBarrierGroup.BinString();
			}
		}

		public override GameObject GetCollision()
		{
			// todo
			return null;
		}
	}
}
