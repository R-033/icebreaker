using Hypercooled.Utils;
using System;
using UnityEngine;

namespace Hypercooled.Shared.Core
{
	public abstract class TopologyObject
	{
		private string m_collectionName;

		public string CollectionName
		{
			get => this.m_collectionName;
			set => this.m_collectionName = value ?? String.Empty;
		}
		public uint Key => this.m_collectionName.BinHash();
		public abstract bool AllowsMovement { get; }

		public TopologyObject()
		{
			this.m_collectionName = String.Empty;
		}

		public abstract GameObject GetCollision();
	}
}
