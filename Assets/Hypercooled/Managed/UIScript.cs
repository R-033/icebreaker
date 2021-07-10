using Hypercooled.Underground2.Core;
using Hypercooled.Underground2.MapStream;
using UnityEngine;
using UnityEngine.UI;

namespace Hypercooled.Managed
{
	public class UIScript : MonoBehaviour
	{
		int m_frameCounter = 0;
		float m_timeCounter = 0.0f;
		float m_lastFramerate = 0.0f;
		public float FpsRefreshTime = 0.5f;

		public Text m_debugText;
		Shared.Runtime.TrackStreamer m_trackManager;
		GizmoHelper m_gizmoHelper;
		Camera m_camera;

		private void FetchRequiredComponents()
		{
			if (m_trackManager == null)
			{
				m_trackManager = FindObjectOfType<Main>().Streamer;
			}

			if (m_gizmoHelper == null)
			{
				m_gizmoHelper = GizmoHelper.Instance;
			}

			if (m_camera == null)
			{
				m_camera = FindObjectsOfType<Camera>()[0];
			}
		}

		private void Awake()
		{
			FetchRequiredComponents();
		}

		private void Update()
		{
			if (m_timeCounter < FpsRefreshTime)
			{
				m_timeCounter += Time.deltaTime;
				m_frameCounter++;
			}
			else
			{
				m_lastFramerate = (float)m_frameCounter / m_timeCounter;
				m_frameCounter = 0;
				m_timeCounter = 0.0f;
			}
		}

		private void LateUpdate()
		{
			FetchRequiredComponents();

			if (m_debugText != null)
			{
				string dbgTextString = "";

				// fps
				dbgTextString += $"{m_lastFramerate:0.00} FPS".PadLeft(6);
				dbgTextString += " | ";

				// frame time
				dbgTextString += $"{FrameTimer.AverageFrameTime:0.00000}ms".PadLeft(6);

				// track manager stuff
				if (!(m_trackManager is null) && m_trackManager.State != Shared.Runtime.TrackState.Empty)
				{
					if (m_trackManager.State == Shared.Runtime.TrackState.Loading ||
						m_trackManager.State == Shared.Runtime.TrackState.Loaded ||
						m_trackManager.State == Shared.Runtime.TrackState.Activating)
					{
						dbgTextString += "| Loading... Please Stand By...";
					}

					if (m_trackManager.Mode == Shared.Runtime.TrackMode.Editing)
					{
						dbgTextString += $" | Sections Loaded: {m_trackManager.CurrentSections.Length}";
					}

					if (m_trackManager.Mode == Shared.Runtime.TrackMode.Streaming)
					{
						dbgTextString += $" | Section Render: {m_trackManager.RenderingSection}";
					}
				}

				// gizmo
				//if (m_gizmoHelper != null)
				//{
				//	var gizmoControlledObject = m_gizmoHelper.GetControlledObject();
				//
				//	dbgTextString += " | Selected object: " +
				//	                 (ReferenceEquals(gizmoControlledObject, null) || !gizmoControlledObject ? "(none)" : gizmoControlledObject.name);
				//}

				// position
				var position = m_camera.transform.position;
				dbgTextString += $" | Position: <{position.x}, {position.y}, {position.z}>";

				m_debugText.text = dbgTextString;
			}
		}

		public void ToggleMenu(bool enable)
		{
			this.gameObject.SetActive(enable);
		}
	}
}
