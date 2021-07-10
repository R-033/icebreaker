using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Hypercooled.Utils
{
	public class ObjectExport
	{
		// https://stackoverflow.com/a/51317663
		private static Texture2D Decompress(Texture2D source)
		{
			RenderTexture renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

			Graphics.Blit(source, renderTex);
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTex;
			Texture2D readableText = new Texture2D(source.width, source.height);
			readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			readableText.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(renderTex);
			return readableText;
		}

		private class ObjExporter
		{
			MeshFilter m_filter;
			Transform m_transform;
			string m_name;

			public ObjExporter(Transform trans)
			{
				m_name = trans.name;
				m_transform = trans;
				m_filter = trans.GetComponent<MeshFilter>();
			}

			private void ProcessTransform(StreamWriter writer)
			{
				const char separator = '/';
				var s = m_transform.localScale;
				var p = m_transform.localPosition;
				var r = m_transform.localRotation;

				var mesh = m_filter.sharedMesh;
				if (!mesh) Debug.LogError("MeshFilter missing mesh.");
				var mats = m_filter.GetComponent<Renderer>().sharedMaterials;

				var vertexMap = new Dictionary<Vector3, int>();
				var normalMap = new Dictionary<Vector3, int>();
				var uvMap = new Dictionary<Vector2, int>();

				foreach (var vertexx in mesh.vertices)
				{
					if (!vertexMap.ContainsKey(vertexx))
					{
						var vertex = m_transform.TransformPoint(vertexx);
						writer.WriteLine($"v {vertex.x} {vertex.y} {-vertex.z}");
						vertexMap.Add(vertexx, vertexMap.Count + 1);
					}
				}

				writer.WriteLine();

				foreach (var normall in mesh.normals)
				{
					if (!normalMap.ContainsKey(normall))
					{
						var normal = r * normall;
						writer.WriteLine($"vn {-normal.x} {-normal.y} {normal.z}");
						normalMap.Add(normall, normalMap.Count + 1);
					}
				}

				writer.WriteLine();

				foreach (var uv in mesh.uv)
				{
					if (!uvMap.ContainsKey(uv))
					{
						writer.WriteLine($"vt {uv.x} {uv.y}");
						uvMap.Add(uv, uvMap.Count + 1);
					}
				}

				writer.WriteLine();
				writer.WriteLine($"g {m_transform.name}");

				for (var material = 0; material < mesh.subMeshCount; material++)
				{
					if (mats[material].mainTexture == null) continue;

					writer.WriteLine();
					writer.WriteLine($"usemtl {mats[material].mainTexture?.name ?? "UNKNOWN"}");

					var triangles = mesh.GetTriangles(material);
					var sb = new StringBuilder();

					for (var i = 0; i < triangles.Length; i += 3)
					{
						var i1 = triangles[i];
						var i2 = triangles[i + 1];
						var i3 = triangles[i + 2];

						var v1 = vertexMap[mesh.vertices[i3]];
						var v2 = vertexMap[mesh.vertices[i2]];
						var v3 = vertexMap[mesh.vertices[i1]];

						var vn1 = normalMap[mesh.normals[i3]];
						var vn2 = normalMap[mesh.normals[i2]];
						var vn3 = normalMap[mesh.normals[i1]];

						var vt1 = uvMap[mesh.uv[i3]];
						var vt2 = uvMap[mesh.uv[i2]];
						var vt3 = uvMap[mesh.uv[i1]];

						sb.Clear();
						sb.Append("f ");

						sb.Append(v1); sb.Append(separator); sb.Append(vt1);
						sb.Append(separator); sb.Append(vn1); sb.Append(" ");

						sb.Append(v2); sb.Append(separator); sb.Append(vt2);
						sb.Append(separator); sb.Append(vn2); sb.Append(" ");

						sb.Append(v3); sb.Append(separator); sb.Append(vt3);
						sb.Append(separator); sb.Append(vn3);

						writer.WriteLine(sb.ToString());
					}
				}
			}

			public void ExportMtl(string directory)
			{
				if (m_filter == null)
				{
					Debug.LogError("MeshFilter not present on selected object.");
					return;
				}

				using (StreamWriter writer = new StreamWriter(Path.Combine(directory, m_name + ".mtl")))
				{
					var mesh = m_filter.sharedMesh;
					if (!mesh) Debug.LogError("MeshFilter missing mesh.");
					var mats = m_filter.GetComponent<Renderer>().sharedMaterials;

					for (var material = 0; material < mesh.subMeshCount; material++)
					{
						writer.WriteLine($"newmtl {mats[material].mainTexture?.name ?? "UNKNOWN"}");

						// all the hardcodey :)
						writer.WriteLine("Ns 0");
						writer.WriteLine("Ka 0.00 0.00 0.00");
						writer.WriteLine("Kd 0.80 0.80 0.80");
						writer.WriteLine("Ks 0.00 0.00 0.00");
						writer.WriteLine("d 1");
						writer.WriteLine("illum 2");
						writer.WriteLine($"map_Kd {mats[material].mainTexture?.name ?? "UNKNOWN"}.png");

						writer.WriteLine();
					}
				}
			}

			public void ExportObj(string directory, bool shiftToOrigin)
			{
				if (m_filter == null)
				{
					Debug.LogError("MeshFilter not present on selected object.");
					return;
				}

				Vector3 originalPosition = m_transform.position;
				if (shiftToOrigin) m_transform.position = Vector3.zero;

				using (StreamWriter writer = new StreamWriter(Path.Combine(directory, m_name + ".obj")))
				{
					writer.AutoFlush = true;

					writer.WriteLine($"# {m_name}");
					writer.WriteLine("# Exported by Hypercooled");
					writer.WriteLine($"# {DateTime.Now:dd/MM/yyyy hh:mm tt}");
					writer.WriteLine();
					writer.WriteLine($"mtllib {m_name + ".mtl"}");
					writer.WriteLine();

					ProcessTransform(writer);

					writer.Flush();
					writer.Close();
				}

				
				m_transform.position = originalPosition;
			}
		}

		public static void ExportObj(GameObject rootObject)
		{
			ExportObj(rootObject, null, true, false);
		}

		public static void ExportObj(GameObject rootObject, string directory)
		{
			ExportObj(rootObject, directory, true, false);
		}

		public static void ExportObj(GameObject rootObject, string directory, bool shiftToOrigin, bool openShell = false)
		{
			var obj = new ObjExporter(rootObject.transform);
			Directory.CreateDirectory(directory);

			obj.ExportObj(directory, shiftToOrigin);
			obj.ExportMtl(directory);

			// export textures
			var renderer = rootObject.GetComponent<MeshRenderer>();

			if (renderer == null)
			{
				Debug.LogError("renderer == null");
				return;
			}

			foreach (var material in renderer.materials)
			{
				var mainTexture = material.mainTexture as Texture2D;

				if (mainTexture != null)
				{
					var decompressed = Decompress(mainTexture);
					var bytes = decompressed.EncodeToPNG();

					File.WriteAllBytes(Path.Combine(directory, mainTexture.name + ".png"), bytes);
					UnityEngine.Object.Destroy(decompressed);
				}
			}

			if (openShell)
			{
				System.Diagnostics.Process.Start(Path.Combine(Path.GetTempPath(), "Hypercooled"));
			}
		}
	}
}
