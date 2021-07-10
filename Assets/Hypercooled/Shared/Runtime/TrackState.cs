using System;

namespace Hypercooled.Shared.Runtime
{
	public enum TrackState : int
	{
		Empty,
		Unloaded,
		Loading,
		Loaded,
		Activating,
		Activated,
		Cancelled,
	}

	public enum TrackMode : int
	{
		None,
		Editing,
		Streaming,
	}

	[Flags()]
	public enum TrackVisibility : int
	{
		EmptyObjects = 0,
		SceneryObjects = 1 << 0,
		LightObjects = 1 << 1,
		FlareObjects = 1 << 2,
		TriggerObjects = 1 << 3,
		EmitterObjects = 1 << 4,
		TopologyObjects = 1 << 5,

		AllObjects = SceneryObjects | LightObjects | FlareObjects | TriggerObjects | EmitterObjects | TopologyObjects,
	}

	public enum TrackLOD : int
	{
		A = 0,
		B = 1,
		C = 2,
		D = 3,
	}
}
