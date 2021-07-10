using UnityEngine;

namespace Hypercooled.Shared.Structures
{
	public struct PlainTransform
	{
		public Vector3 Position { get; set; }
		public Vector3 Scale { get; set; }
		public Quaternion Rotation { get; set; }

		public PlainTransform(Vector3 position, Vector3 scale, Quaternion rotation)
		{
			this.Position = position;
			this.Scale = scale;
			this.Rotation = rotation;
		}
	}
}
