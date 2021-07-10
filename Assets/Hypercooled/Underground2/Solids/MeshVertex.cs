using Hypercooled.Shared.Structures;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[DebuggerDisplay("Position: {this.Position.ToString()}")]
	public struct MeshVertex // 0x00134B01
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Color8UI Color;
		public Vector2 UV; // (U is +) & (V is +)

		public const int SizeOf = 0x24;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct MeshReducedVertex // 0x00134B01
	{
		public Vector3 Position;
		public Color8UI Color;
		public Vector2 UV; // (U is +) & (V is +)

		public const int SizeOf = 0x18;

		public MeshVertex ToMeshVertex()
		{
			return new MeshVertex()
			{
				Position = Position,
				Normal = Vector3.zero,
				Color = Color,
				UV = UV,
			};
		}
	}
}
