using Hypercooled.Shared.Structures;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Hypercooled.Utils
{
	public class MathExtensions
	{
		public static bool IsInBoundingBox(Vector2 point, Vector2 min, Vector2 max)
		{
			return min.x <= point.x && point.x <= max.x
				&& min.y <= point.y && point.y <= max.y;
		}

		public static bool IsInBoundingBox(Vector3 point, Vector2 min, Vector2 max)
		{
			return min.x <= point.x && point.x <= max.x
				&& min.y <= point.y && point.y <= max.y;
		}

		public static bool IsInBoundingBox(Vector2 point, Vector3 min, Vector3 max)
		{
			return min.x <= point.x && point.x <= max.x
				&& min.y <= point.y && point.y <= max.y;
		}

		public static bool IsInBoundingBox(Vector2 point, Vector2 min, Vector3 max)
		{
			return min.x <= point.x && point.x <= max.x
				&& min.y <= point.y && point.y <= max.y;
		}

		public static bool IsInBoundingBox(Vector3 point, Vector3 min, Vector3 max)
		{
			return min.x <= point.x && point.x <= max.x
				&& min.y <= point.y && point.y <= max.y
				&& min.z <= point.z && point.z <= max.z;
		}

		public static float Max(params float[] values) => Enumerable.Max(values);

		public static float Min(params float[] values) => Enumerable.Min(values);

		public static short Max(params short[] values) => Enumerable.Max(values);

		public static short Min(params short[] values) => Enumerable.Min(values);

		public static bool IsInPolygon(Vector2[] polygon, Vector2 point)
		{
			bool result = false;
			int j = polygon.Length - 1;

			for (int i = 0; i < polygon.Length; i++)
			{
				var pointI = polygon[i];
				var pointJ = polygon[j];

				if (pointI.y < point.y && pointJ.y >= point.y || pointJ.y < point.y && pointI.y >= point.y)
				{
					if (pointI.x + (point.y - pointI.y) / (pointJ.y - pointI.y) * (pointJ.x - pointI.x) < point.x)
					{
						result = !result;
					}
				}

				j = i;
			}

			return result;
		}

		public static bool IsInPolygon(List<Vector2> polygon, Vector2 point)
		{
			bool result = false;
			int j = polygon.Count - 1;

			for (int i = 0; i < polygon.Count; i++)
			{
				var pointI = polygon[i];
				var pointJ = polygon[j];

				if (pointI.y < point.y && pointJ.y >= point.y || pointJ.y < point.y && pointI.y >= point.y)
				{
					if (pointI.x + (point.y - pointI.y) / (pointJ.y - pointI.y) * (pointJ.x - pointI.x) < point.x)
					{
						result = !result;
					}
				}

				j = i;
			}

			return result;
		}

		public static bool IsInPolygon(Vector2 point, params Vector3[] polygon)
		{
			bool result = false;
			int j = polygon.Length - 1;

			for (int i = 0; i < polygon.Length; i++)
			{
				var pointI = polygon[i];
				var pointJ = polygon[j];

				if (pointI.y < point.y && pointJ.y >= point.y || pointJ.y < point.y && pointI.y >= point.y)
				{
					if (pointI.x + (point.y - pointI.y) / (pointJ.y - pointI.y) * (pointJ.x - pointI.x) < point.x)
					{
						result = !result;
					}
				}

				j = i;
			}

			return result;
		}

		public static bool IsInTriangle(Vector3 point, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			var a1 = (point.x - p2.x) * (p1.z - p2.z) - (p1.x - p2.x) * (point.z - p2.z);
			var a2 = (point.x - p3.x) * (p2.z - p3.z) - (p2.x - p3.x) * (point.z - p3.z);
			var a3 = (point.x - p1.x) * (p3.z - p1.z) - (p3.x - p1.x) * (point.z - p1.z);

			var has_neg = (a1 < 0) || (a2 < 0) || (a3 < 0);
			var has_pos = (a1 > 0) || (a2 > 0) || (a3 > 0);

			return !(has_neg && has_pos);
		}

		public static float GetTriElevation(Vector2 at, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			var cross = Vector3.Cross(p3 - p1, p2 - p1).normalized;

			var arg1 = cross.x * at.x + cross.z * at.y;
			var arg2 = cross.x * p1.x + cross.y * p1.y + cross.z * p1.z;

			return (arg2 - arg1) / cross.y;
		}

		public static Quaternion LookRotation(float m00, float m02, float m01, float m20, float m22, float m21, float m10, float m12, float m11)
		{
			float num8 = m00 + m11 + m22;
			float x, y, z, w;

			if (num8 > 0f)
			{
				var num = Mathf.Sqrt(num8 + 1f);
				w = num * 0.5f;
				num = 0.5f / num;
				x = (m12 - m21) * num;
				y = (m20 - m02) * num;
				z = (m01 - m10) * num;
				return new Quaternion(x, y, z, w).normalized;
			}
			if (m00 >= m11 && m00 >= m22)
			{
				var num7 = Mathf.Sqrt(1f + m00 - m11 - m22);
				var num4 = 0.5f / num7;
				x = 0.5f * num7;
				y = (m01 + m10) * num4;
				z = (m02 + m20) * num4;
				w = (m12 - m21) * num4;
				return new Quaternion(x, y, z, w).normalized;
			}
			if (m11 > m22)
			{
				var num6 = Mathf.Sqrt(1f + m11 - m00 - m22);
				var num3 = 0.5f / num6;
				x = (m10 + m01) * num3;
				y = 0.5f * num6;
				z = (m21 + m12) * num3;
				w = (m20 - m02) * num3;
				return new Quaternion(x, y, z, w).normalized;
			}

			var num5 = Mathf.Sqrt(1f + m22 - m00 - m11);
			var num2 = 0.5f / num5;
			x = (m20 + m02) * num2;
			y = (m21 + m12) * num2;
			z = 0.5f * num5;
			w = (m01 - m10) * num2;
			return new Quaternion(x, y, z, w).normalized;
		}

		/// <summary>
		/// Not working at the moment, do not use!!!
		/// </summary>
		/// <param name="r">Vector formed from [A11 A12 A13] values of 3x3 or 4x4 matrix.</param>
		/// <param name="f">Vector formed from [A21 A22 A23] values of 3x3 or 4x4 matrix.</param>
		/// <param name="u">Vector formed from [A31 A32 A33] values of 3x3 or 4x4 matrix.</param>
		/// <returns></returns>
		public static Quaternion RotationFrom3x3(Vector3 r, Vector3 f, Vector3 u, bool nfsSwap)
		{
			// Should be normalized
			if (r.magnitude != 1.0f) r = r.normalized;
			if (f.magnitude != 1.0f) f = f.normalized;
			if (u.magnitude != 1.0f) u = u.normalized;

			if (nfsSwap)
			{
				// [r.x r.y r.z]   [r.x r.z r.y]
				// [f.x f.y f.z] = [u.x u.z u.y] -> swap 2&3 rows and columns
				// [u.x u.y u.z]   [f.x f.z f.y]

				var t = u;
				r = new Vector3(r.x, r.z, r.y); // row1
				u = new Vector3(f.x, f.z, f.y); // row3
				f = new Vector3(t.x, t.z, t.y); // row2
			}

			float trace = r.x + f.y + u.z;

			if (trace > 0)
			{
				float S = Mathf.Sqrt(trace + 1.0f) * 2;
				float qw = 0.25f * S;
				float qx = (u.y - f.z) / S;
				float qy = (r.z - u.x) / S;
				float qz = (f.x - r.y) / S;
				return new Quaternion(qx, qy, qz, qw);
			}
			else if ((r.x > f.y) & (r.x > u.z))
			{
				float S = Mathf.Sqrt(1.0f + r.x - f.y - u.z) * 2;
				float qw = (u.y - f.z) / S;
				float qx = 0.25f * S;
				float qy = (r.y + f.x) / S;
				float qz = (r.z + u.x) / S;
				return new Quaternion(qx, qy, qz, qw);
			}
			else if (f.y > u.z)
			{
				float S = Mathf.Sqrt(1.0f + f.y - r.x - u.z) * 2;
				float qw = (r.z - u.x) / S;
				float qx = (r.y + f.x) / S;
				float qy = 0.25f * S;
				float qz = (f.z + u.y) / S;
				return new Quaternion(qx, qy, qz, qw);
			}
			else
			{
				float S = Mathf.Sqrt(1.0f + u.z - r.x - f.y) * 2;
				float qw = (f.x - r.y) / S;
				float qx = (r.z + u.x) / S;
				float qy = (f.z + u.y) / S;
				float qz = 0.25f * S;
				return new Quaternion(qx, qy, qz, qw);
			}
		}

		public static PlainTransform TransformToPlain(Matrix4x4 matrix)
		{
			if (Matrix4x4.Decompose(matrix, out var scale, out var rotation, out var position))
			{
				return new PlainTransform(
					new Vector3(position.X, position.Y, position.Z),
					new Vector3(scale.X, scale.Y, scale.Z),
					new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W).normalized
				);
			}
			else
			{
				return new PlainTransform(Vector3.zero, Vector3.zero, Quaternion.identity);
			}
		}

		public static PlainTransform TransformToPlain(UnityEngine.Matrix4x4 matrix)
		{
			var position = new Vector3(matrix.m03, matrix.m13, matrix.m23);

			var vRight = new Vector3(matrix.m00, matrix.m10, matrix.m20);
			var vUpwards = new Vector3(matrix.m01, matrix.m11, matrix.m21);
			var vForward = new Vector3(matrix.m02, matrix.m12, matrix.m22);

			var scale = new Vector3(vRight.magnitude, vUpwards.magnitude, vForward.magnitude);
			var rotation = Quaternion.LookRotation(vForward, vUpwards);

			return new PlainTransform(position, scale, rotation);
		}

		public static Matrix4x4 OrthoInverse(Matrix4x4 matrix)
		{
			var matrixMV41 = -matrix.M41 * matrix.M11 - matrix.M42 * matrix.M12 - matrix.M43 * matrix.M13;
			var matrixMV42 = -matrix.M41 * matrix.M21 - matrix.M42 * matrix.M22 - matrix.M43 * matrix.M23;
			var matrixMV43 = -matrix.M41 * matrix.M31 - matrix.M42 * matrix.M32 - matrix.M43 * matrix.M33;

			return new Matrix4x4
			(
				matrix.M11, matrix.M21, matrix.M31, matrix.M14,
				matrix.M12, matrix.M22, matrix.M32, matrix.M24,
				matrix.M13, matrix.M23, matrix.M33, matrix.M34,
				matrixMV41, matrixMV42, matrixMV43, matrix.M44
			);
		}

		public static Matrix4x4 TRS(Vector3 position, Vector3 scale, Quaternion rotation)
		{
			float num1 = rotation.x * rotation.x;
			float num2 = rotation.y * rotation.y;
			float num3 = rotation.z * rotation.z;
			float num4 = rotation.x * rotation.y;
			float num5 = rotation.z * rotation.w;
			float num6 = rotation.z * rotation.x;
			float num7 = rotation.y * rotation.w;
			float num8 = rotation.y * rotation.z;
			float num9 = rotation.x * rotation.w;

			float sx = scale.x;
			float sy = scale.y;
			float sz = scale.z;

			return new Matrix4x4
			(
				(1f - 2f * (num2 + num3)) * sx, (2f * (num4 + num5)) * sx, (2f * (num6 - num7)) * sx, 0.0f,
				(2f * (num4 - num5)) * sy, (1f - 2f * (num3 + num1)) * sy, (2f * (num8 + num9)) * sy, 0.0f,
				(2f * (num6 + num7)) * sz, (2f * (num8 - num9)) * sz, (1f - 2f * (num2 + num1)) * sz, 0.0f,
				position.x, position.y, position.z, 1.0f
			);
		}
	}
}
