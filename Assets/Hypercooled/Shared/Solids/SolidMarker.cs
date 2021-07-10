using CoreExtensions.Text;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[DebuggerDisplay("Key: {this.StringKey}")]
	public struct SolidMarker // 0x0013401A
	{
		public uint Key;
		public int IParam0;
		public float FParam0;
		public float FParam1;
		public Matrix4x4 Matrix;

		public string StringKey => this.Key.FastToHexString(false);
		public const int SizeOf = 0x50;
	}
}
