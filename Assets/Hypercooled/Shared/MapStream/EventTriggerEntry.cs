using System.Runtime.InteropServices;
using UnityEngine;

namespace Hypercooled.Shared.MapStream
{
	public enum TriggerEventID : int
	{
		CAR_ON_FERN = 0x10003,
		VIEW_DRIVING_LINE = 0x10005,
		ACTIVATE_TRAIN = 0x10006,
		SOUND = 0x10007,
		GUIDE_ARROW = 0x10008,
		ACTIVATE_PLANE = 0x10009,
		INITIATE_PURSUIT = 0x20000,
		CALL_FOR_BACKUP = 0x20001,
		CALL_FOR_ROADBLOCK = 0x20002,
		STRATEGY_INITIATE = 0x20003,
		COLLISION = 0x20004,
		ANNOUNCE_ARREST = 0x20005,
		STRATEGY_OUTCOME = 0x20006,
		ROADBLOCK_UPDATE = 0x20007,
		CANCEL_PURSUIT = 0x20008,
		START_SIREN = 0x40000,
		STOP_SIREN = 0x40001,
	};

	public enum TriggerParameterType : uint
	{
		TREE = 0x001D5D4F,
		TUNNEL_ENTRY = 0x2C15BD86,
		INTERSECTION = 0x495F75B6,
		PILLAR = 0x72F66B23,
		FOUNTAIN = 0xB2B5A6E3,
		TRAFFIC_LIGHT = 0xB6BD9C95,
		FREEWAY_SIGN = 0xF3ABE1C2,
		LAMPPOST = 0xF40A48EF,
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct EventTriggerEntry // 0x00036003
	{
		public uint Key;
		public TriggerEventID EventID;
		public TriggerParameterType Parameter;
		public int TrackDirectionMask; // is it always 3 ???
		public Vector3 Position;
		public float Radius;

		public const int SizeOf = 0x20;
	}
}
