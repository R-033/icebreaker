using Hypercooled.Underground2.MapStream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Hypercooled.Underground2.Core
{
	public class TopologyObject : Shared.Core.TopologyObject
	{
		private static TopologyCoordinate[] ms_emptyCoordinates = new TopologyCoordinate[0];

		private TopologyCoordinate[] m_coordinates;

		public override bool AllowsMovement => false;
		public TopologyCoordinate[] Coordinates
		{
			get => this.m_coordinates;
			set => this.m_coordinates = value ?? ms_emptyCoordinates;
		}

		public TopologyObject(IEnumerable<TopologyCoordinate> coordinates)
		{
			this.m_coordinates = coordinates?.ToArray() ?? ms_emptyCoordinates;
		}

		public override GameObject GetCollision()
		{
			// todo
			return null;
		}
	}
}
