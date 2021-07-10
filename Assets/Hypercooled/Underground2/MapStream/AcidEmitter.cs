using System.Runtime.InteropServices;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Hypercooled.Underground2.MapStream
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AcidEmitterPackHeader
	{
		public long Padding;
		public int Version;
		public int NumEmitters;

		public const int SizeOf = 0x10;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct AcidEmitter // 0x00035021
	{
		public long Padding0;
		public uint NameHash;
		public uint TextureNameHash;
		public uint GroupNameHash;
		public int Padding1;
		public byte SpreadAsDisc;
		public byte ContactSheetW;
		public byte ContactSheetH;
		public byte AnimFPS;
		public byte RandomStartFrame;
		public byte RandomRotationDirection;
		public byte MotionLive;
		public byte Padding2;
		public int OnCycle;
		public float OnCycleVariance;
		public int OffCycle;
		public float OffCycleVariance;
		public int NumParticles;
		public float NumParticlesVariance;
		public float Life;
		public float LifeVariance;
		public float Speed;
		public float SpeedVariance;
		public float InitialAngleRange;
		public float SpreadAngle;
		public float MotionInherit;
		public float MotionInheritVariance;
		public float CarPosition;
		public int Padding3;
		public Vector4 VolumeCenter;
		public Vector4 VolumeExtent;
		public float FarClip;
		public float Gravity;
		public float Drag;
		public float MaxPixelSize;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x04)]
		public float[] KeyPositions;

		public Matrix4x4 ColourMatrix;
		public Vector4 Size;
		public Vector4 RelativeAngle;
		public Matrix4x4 ColourBasis;
		public Matrix4x4 ExtraBasis;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
		public string Name;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x30)]
		public string TextureName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x30)]
		public string GroupName;

		public const int SizeOf = 0x220;
	}
}
