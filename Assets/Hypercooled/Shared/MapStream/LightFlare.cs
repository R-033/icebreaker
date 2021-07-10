using Hypercooled.Shared.Structures;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.MapStream
{
	public enum LightFlareFlags : byte
	{
		ELF_BIDIRECTIONAL = 1,
		ELF_N_DIRECTIONAL = 2,
		ELF_UNI_DIRECTIONAL = 4,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct LightFlare // 0x00135102
	{
		public long Padding;
		public uint Key;
		public Color8UI ColorTint;
		public Vector3 Position;
		public float ReflectPosZ;
		public Vector3 Direction;
		public byte Type;
		public LightFlareFlags Flags;
		public ushort ScenerySectionNumber;

		public const int SizeOf = 0x30;
	}
}
