using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hypercooled.Shared.Core;
using Hypercooled.Shared.Runtime;
using SFB;
using UnityEngine;
#if !UNITY_WEBGL
using System.IO;
#endif

namespace Hypercooled.Managed
{
	public class Main : MonoBehaviour
	{
		public static bool IsUnix =>
			Application.platform != RuntimePlatform.WindowsEditor &&
			Application.platform != RuntimePlatform.WindowsPlayer;

		public static string DefaultDirectory => IsUnix ? "/" : @"C:\";

		public TrackMode Mode;
		public TrackStreamer Streamer;

		public bool RunTestsOnly;

		public bool RenderScenery = true;
		public bool RenderLights;
		public bool RenderFlares;
		public bool RenderTriggers;
		public bool RenderEmitters;
		public bool RenderTopology;
		public bool RenderBoundary;

		public List<ushort> EditingSections;
		public Camera MainCamera;

		public GameObject BarrierBaseObject;
		public Lazy<MeshRenderer> BarrierMeshRenderer;

		public GameObject SceneryObjectPrefab;
		public GameObject LightObjectPrefab;
		public GameObject FlareObjectPrefab;
		public GameObject TriggerObjectPrefab;
		public GameObject EmitterObjectPrefab;
		public GameObject TopologyObjectPrefab;

		public Shader UnlitCutoutShader;
		public Shader TransparentShader;
		public Shader ParticleAdditiveShader;
		public Shader fallbackShader;

		private void PreinitStatic()
		{
			PreProcessing.Assertions.AssertAllSupports();
			Newtonsoft.Json.Unity.ConverterInitializer.Initialize();

			TrackStreamer.SceneryObjectPrefab = this.SceneryObjectPrefab;
			TrackStreamer.LightObjectPrefab = this.LightObjectPrefab;
			TrackStreamer.FlareObjectPrefab = this.FlareObjectPrefab;
			TrackStreamer.TriggerObjectPrefab = this.TriggerObjectPrefab;
			TrackStreamer.EmitterObjectPrefab = this.EmitterObjectPrefab;
			//HyperStreamer.TopologyObjectPrefab = this.TopologyObjectPrefab;
		}

		private TrackUnpacker GetTrackUnpacker(Game game, string folder, string global, string locale)
		{
			switch (game)
			{
				case Game.Underground1: return null;
				case Game.Underground2: return new Underground2.Core.TrackUnpacker(folder, global, locale);
				case Game.MostWanted: return new MostWanted.Core.TrackUnpacker(folder, global, locale);
				case Game.Carbon: return new Carbon.Core.TrackUnpacker(folder, global, locale);
				default: return null;
			}
		}
		private void ShowMessage(string text)
		{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
			System.Windows.Forms.MessageBox.Show(text);
#else
			Debug.Log(text);
#endif
		}

		public string SelectExistingProject()
		{
			var hyperFilter = new ExtensionFilter[] { new ExtensionFilter("Hyper project file", "hyper") };

			var global = StandaloneFileBrowser.OpenFilePanel("Select hyper project file", DefaultDirectory, hyperFilter, false);

			if (global.Length == 0)
			{
			}
			else
			{
				if (File.Exists(global[0]))
				{
					return global[0];
				}
				else
				{
					//this.ShowMessage("Selected file does not exist. Please try again.");
				}
			}

			return "";
		}

		public string SelectAndVerifyGlobalFile(string defaultValue)
		{
			var lzcFilter = new ExtensionFilter[] { new ExtensionFilter("LZC binary files", "LZC") };

			var global = StandaloneFileBrowser.OpenFilePanel("Select GLOBALB.LZC file", DefaultDirectory, lzcFilter, false);

			if (global.Length == 0)
			{
			}
			else
			{
				if (File.Exists(global[0]))
				{
					return global[0];
				}
				else
				{
					//this.ShowMessage("Selected file does not exist. Please try again.");
				}
			}

			return defaultValue;
		}
		public string SelectAndVerifyLocationFile(string defaultValue)
		{
			var bunFilter = new ExtensionFilter[] { new ExtensionFilter("BUN binary files", "BUN") };

			var locale = StandaloneFileBrowser.OpenFilePanel("Select LXRY.BUN location file", DefaultDirectory, bunFilter, false);

			if (locale.Length == 0)
			{
			}
			else
			{
				if (File.Exists(locale[0]))
				{
					return locale[0];
				}
				else
				{
					//this.ShowMessage("Selected file does not exist. Please try again.");
				}
			}

			return defaultValue;
		}
		public string SelectAndVerifyUnpackDirectory(string defaultValue)
		{
			var unpack = StandaloneFileBrowser.OpenFolderPanel("Select directory to unpack project into", DefaultDirectory, false);

			if (unpack.Length == 0)
			{
			}
			else
			{
				if (Directory.Exists(unpack[0]))
				{
					return unpack[0];
				}
				else
				{
					//this.ShowMessage("Selected directory does not exist. Please try again.");
				}
			}

			return defaultValue;
		}
		public IEnumerator UnpackProject(Game game, string global, string locale, string folder)
		{
			var unpacker = this.GetTrackUnpacker(game, folder, global, locale);
			unpacker.Initialize();
			bool done = false;

			this.WaitForUnpackerToFinishUnloadingDirectory(unpacker, () => done = true);
			while (!done) yield return null;

			Configuration.LastSelectedProjectPath = Path.Combine(folder, Namings.MainProj + Namings.HyperExt);

			StartCoroutine(LoadTrackProject(game, Configuration.LastSelectedProjectPath));
		}

		private async void WaitForUnpackerToFinishUnloadingDirectory(TrackUnpacker unpacker, Action callback)
		{
			await unpacker.Unpack();
			callback?.Invoke();
		}

		private Type GetTrackStreamer(Game game)
		{
			switch (game)
			{
				case Game.Underground1: return null;
				case Game.Underground2: return typeof(Underground2.Runtime.TrackStreamer);
				case Game.MostWanted: return typeof(MostWanted.Runtime.TrackStreamer);
				case Game.Carbon: return typeof(Carbon.Runtime.TrackStreamer);
				default: return null;
			}
		}
		public IEnumerator LoadTrackProject(Game game, string path)
		{
			var header = ProjectHeader.Deserialize(path);
			this.Streamer = this.gameObject.AddComponent(this.GetTrackStreamer(game)) as TrackStreamer;
			this.Streamer.RenderCamera = this.MainCamera;
			this.EditingSections = this.Streamer.RequestedSections;

			yield return this.StartCoroutine(this.InternalTrackStreamerLoaderAwaiter(header));
		}
		private IEnumerator InternalTrackStreamerLoaderAwaiter(ProjectHeader header)
		{
			bool done = false;
			this.Streamer.Load(header, () => done = true);
			while (!done) yield return null;
		}

		private void UpdateRenderingLayers()
		{
			var visibility = TrackVisibility.EmptyObjects;

			if (this.RenderScenery) visibility |= TrackVisibility.SceneryObjects;
			if (this.RenderLights) visibility |= TrackVisibility.LightObjects;
			if (this.RenderFlares) visibility |= TrackVisibility.FlareObjects;
			if (this.RenderTriggers) visibility |= TrackVisibility.TriggerObjects;
			if (this.RenderEmitters) visibility |= TrackVisibility.EmitterObjects;
			if (this.RenderTopology) visibility |= TrackVisibility.TopologyObjects;

			this.Streamer.PleaseChangeAllVisibilities(visibility);
		}

		private void Tests()
		{
			int breakpoint = 0;
		}

		private void Awake()
		{
			Application.targetFrameRate = -1;
		}
		private void Start()
		{
			this.PreinitStatic();

			this.BarrierMeshRenderer = new Lazy<MeshRenderer>(() => BarrierBaseObject.GetComponent<MeshRenderer>());

			if (this.RunTestsOnly) this.Tests();
		}
		private void OnDestroy()
		{
			this.Streamer?.Close();
		}
		private void Update()
		{
			if (this.RunTestsOnly || this.Streamer is null || this.Streamer.State == TrackState.Empty)
			{
				return;
			}

			this.Streamer.SwitchTrackMode(this.Mode);
			this.UpdateRenderingLayers();
		}
		private void OnDrawGizmos()
		{
			if (this.RunTestsOnly ||
				this.RenderBoundary == false ||
				this.Streamer is null ||
				this.Streamer.State == TrackState.Empty ||
				this.Streamer.VisibleSectionManager is null)
			{
				return;
			}
		
			var color = Gizmos.color;
			Gizmos.color = Color.red;
				
			foreach (var visible in this.Streamer.VisibleSectionManager.Boundaries.Values)
			{
				if (visible.Points.Count < 2) continue;
				if (!this.Streamer.VisibleSectionManager.Drivables.ContainsKey(visible.SectionNumber)) continue;
		
				var bounds = visible.Points;
				var l = bounds.Count - 1;
		
				for (int i = 0, k = 1; i < l; ++i, ++k)
				{
					var point1 = new Vector3(bounds[i].x, 0.0f, bounds[i].y);
					var point2 = new Vector3(bounds[k].x, 0.0f, bounds[k].y);
					Gizmos.DrawLine(point1, point2);
				}
		
				var close1 = new Vector3(bounds[l].x, 0.0f, bounds[l].y);
				var close2 = new Vector3(bounds[0].x, 0.0f, bounds[0].y);
				Gizmos.DrawLine(close1, close2);
			}
		
			Gizmos.color = Color.green;

			if (this.Streamer.Mode == TrackMode.Streaming)
			{
				var section = this.Streamer.GetCurrentSectionWeAreIn();

				if (this.Streamer.VisibleSectionManager.Boundaries.TryGetValue(section, out var boundary) &&
					boundary.Points.Count >= 2)
				{
					var bounds = boundary.Points;
					var l = bounds.Count - 1;

					for (int i = 0, k = 1; i < l; ++i, ++k)
					{
						var point1 = new Vector3(bounds[i].x, 0.0f, bounds[i].y);
						var point2 = new Vector3(bounds[k].x, 0.0f, bounds[k].y);
						Gizmos.DrawLine(point1, point2);
					}

					var close1 = new Vector3(bounds[l].x, 0.0f, bounds[l].y);
					var close2 = new Vector3(bounds[0].x, 0.0f, bounds[0].y);
					Gizmos.DrawLine(close1, close2);
				}
			}
			else
			{
				foreach (var section in this.Streamer.CurrentSections)
				{
					if (this.Streamer.VisibleSectionManager.Boundaries.TryGetValue(section, out var boundary))
					{
						var bounds = boundary.Points;
						var l = bounds.Count - 1;

						for (int i = 0, k = 1; i < l; ++i, ++k)
						{
							var point1 = new Vector3(bounds[i].x, 0.0f, bounds[i].y);
							var point2 = new Vector3(bounds[k].x, 0.0f, bounds[k].y);
							Gizmos.DrawLine(point1, point2);
						}

						var close1 = new Vector3(bounds[l].x, 0.0f, bounds[l].y);
						var close2 = new Vector3(bounds[0].x, 0.0f, bounds[0].y);
						Gizmos.DrawLine(close1, close2);
					}
				}
			}

			Gizmos.color = Color.blue;

			foreach (var elevation in this.Streamer.VisibleSectionManager.ElevPolies)
			{
				Gizmos.DrawLine(elevation.Point1, elevation.Point2);
				Gizmos.DrawLine(elevation.Point2, elevation.Point3);
				Gizmos.DrawLine(elevation.Point3, elevation.Point1);
			}

			Gizmos.color = color;
		}
	}
}
