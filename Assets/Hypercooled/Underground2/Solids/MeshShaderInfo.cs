using Hypercooled.Shared.Solids;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Underground2.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[DebuggerDisplay("Texture: {this.TextureIndex} | Shader: {this.ShaderIndex}")]
	public struct MeshShaderInfo // 0x00134B02
	{
		public Vector3 BoundsMin;
		public int NumPolygons;
		public Vector3 BoundsMax;
		public int TextureIndex;
		public int ShaderIndex;
		public long Padding0;
		public long Padding1;
		public int PolygonIndex;
		public MeshEntryFlags Flags;

		public const int SizeOf = 0x3C;
	}
}
