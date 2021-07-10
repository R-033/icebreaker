using CoreExtensions.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hypercooled.Shared.Solids
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[DebuggerDisplay("Key: {this.StringKey}")]
	public struct SolidMaterial // 0x00134013
	{
		public uint Key;
		public int Padding;

		public string StringKey => this.Key.FastToHexString(false);
		public const int SizeOf = 0x08;
	}
}
