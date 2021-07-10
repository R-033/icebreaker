using UnityEngine;

namespace Hypercooled.Managed.Mono
{
	public abstract class ObjectMonoWrapper<T> : MonoBehaviour
	{
		public abstract T Wrapped { get; }

		public abstract void Initialize(T value);
		public abstract void ResetData();
		public abstract void UpdateData();
	}
}
