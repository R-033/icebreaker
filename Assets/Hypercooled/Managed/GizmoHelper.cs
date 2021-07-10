using UnityEngine;

namespace Hypercooled.Managed
{
	internal class GizmoHelper : MonoBehaviour
	{
		public static GizmoHelper Instance;

		// #TODO(avail): configurable
		int m_gizmoWidth = 6;

		public enum GizmoMode
		{
			Translate,
			Rotate,
			Scale
		}

		private enum GizmoAxis
		{
			X,
			Y,
			Z
		}

		public Shader shader;
		public Shader fallbackShader;

		private void CreateTranslateGizmoAxis(GameObject parent, GizmoAxis axis, Color color)
		{
			GameObject go = new GameObject($"TranslateGizmo_{axis}");
			go.transform.parent = parent.transform;

			LineRenderer line = go.AddComponent<LineRenderer>();
			line.sortingLayerName = "OnTop";
			line.sortingOrder = 5;

			line.receiveShadows = false;

			line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			line.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;

			line.positionCount = 2;

			line.useWorldSpace = true;

			line.startWidth = m_gizmoWidth;
			line.endWidth = m_gizmoWidth;

			line.startColor = color;
			line.endColor = color;

			line.material = new Material(shader == null ? fallbackShader : shader);

			// #TODO(avail): raycasts
			/*MeshCollider meshCollider = go.AddComponent<MeshCollider>();

			Mesh mesh = new Mesh();
			line.BakeMesh(mesh, true);

			meshCollider.sharedMesh = mesh;
			meshCollider.convex = true;
			meshCollider.isTrigger = true;*/
		}

		private void UpdateTranslateGizmo()
		{
			var trans = m_controlledObject.transform;

			void UpdateLineRenderer(int idx, Vector3 to)
			{
				var go = m_translateGizmo.transform.GetChild(idx);
				var lr = go.GetComponent<LineRenderer>();
				lr.SetPosition(0, trans.position);
				lr.SetPosition(1, to);
			}

			UpdateLineRenderer(0, trans.position + Vector3.right); // x
			UpdateLineRenderer(1, trans.position + Vector3.up); // y
			UpdateLineRenderer(2, trans.position + Vector3.forward); // z
		}

		GameObject m_translateGizmo;

		private void Start()
		{
			Instance = this;

			GameObject parent = new GameObject("GizmoParent");

			// create translate gizmo
			m_translateGizmo = new GameObject("TranslateGizmo");
			m_translateGizmo.transform.parent = parent.transform;

			CreateTranslateGizmoAxis(m_translateGizmo, GizmoAxis.X, Color.red);
			CreateTranslateGizmoAxis(m_translateGizmo, GizmoAxis.Y, Color.green);
			CreateTranslateGizmoAxis(m_translateGizmo, GizmoAxis.Z, Color.blue);

			// create rotate gizmo

			// create scale gizmo

			DontDestroyOnLoad(parent);

			// set default
			m_mode = GizmoMode.Translate;
		}

		GameObject m_controlledObject;
		GizmoMode m_mode;

		public GameObject GetControlledObject()
		{
			return m_controlledObject;
		}

		public void SetControlledObject(GameObject go)
		{
			m_controlledObject = go;
		}

		public void SetMode(GizmoMode mode)
		{
			m_mode = mode;
		}

		private void Update()
		{
			return;

			if (m_controlledObject != null)
			{
				switch (m_mode)
				{
					case GizmoMode.Translate:
						UpdateTranslateGizmo();
						break;

					default:
						break;
				}
			}
		}
	}
}
