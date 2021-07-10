using Hypercooled.Shared.Structures;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.MostWanted.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[DebuggerDisplay("Position: {this.Position.ToString()}")]
	public struct MeshVertex // 0x00134B01
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Color8UI Color;
		public Vector2 UV;
		public Vector3 Tangent; // is that tangent tho lol?

		public const int SizeOf = 0x30;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal struct MeshReducedVertex // 0x00134B01
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Color8UI Color;
		public Vector2 UV;

		public const int SizeOf = 0x24;

		public MeshVertex ToMeshVertex()
		{
			return new MeshVertex()
			{
				Position = Position,
				Normal = Normal,
				Color = Color,
				UV = UV,
				Tangent = Vector3.zero,
			};
		}
	}
}
