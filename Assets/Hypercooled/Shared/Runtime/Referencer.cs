using System;
using System.Collections.Concurrent;

namespace Hypercooled.Shared.Runtime
{
	public class Referencer<T> where T : class
	{
		private class Ref
		{
			public int Counter { get; set; }
			public T Value { get; set; }

			public Ref(T value)
			{
				this.Value = value;
				this.Counter = 1;
			}
		}

		private readonly ConcurrentDictionary<uint, Ref> m_map;
		private readonly Action<T> m_cleanup;

		public Referencer(Action<T> cleanupAction)
		{
			if (cleanupAction is null) cleanupAction = (T arg) => { };
			this.m_map = new ConcurrentDictionary<uint, Ref>();
			this.m_cleanup = cleanupAction;
		}

		public int Count => this.m_map.Count;

		public void Add(uint key, T value)
		{
			var reference = new Ref(value);
			this.m_map.TryAdd(key, reference);
		}

		public void Clear()
		{
			foreach (var reference in this.m_map.Values)
			{
				this.m_cleanup.Invoke(reference.Value);
			}

			this.m_map.Clear();
		}

		public bool Contains(uint key)
		{
			return this.m_map.ContainsKey(key);
		}

		public T Get(uint key)
		{
			if (this.m_map.TryGetValue(key, out var reference))
			{
				reference.Counter++;
				return reference.Value;
			}

			return null;
		}

		public T GetNoRef(uint key)
		{
			if (this.m_map.TryGetValue(key, out var reference))
			{
				return reference.Value;
			}
			else
			{
				return null;
			}
		}

		public void Release(uint key)
		{
			if (this.m_map.TryGetValue(key, out var reference))
			{
				reference.Counter--;

				if (reference.Counter == 0)
				{
					this.m_map.TryRemove(key, out _);
					this.m_cleanup.Invoke(reference.Value);
				}
			}
		}

		public void Release(uint key, int times)
		{
			if (this.m_map.TryGetValue(key, out var reference))
			{
				reference.Counter -= times;

				if (reference.Counter <= 0)
				{
					this.m_map.TryRemove(key, out _);
					this.m_cleanup.Invoke(reference.Value);
				}
			}
		}

		public void TryAdd(uint key, T value)
		{
			if (!this.m_map.TryGetValue(key, out var reference))
			{
				reference = new Ref(value);
				this.m_map.TryAdd(key, reference);
			}
			else
			{
				reference.Counter++;
			}
		}

		public bool TryGet(uint key, out T value)
		{
			if (this.m_map.TryGetValue(key, out var reference))
			{
				reference.Counter++;
				value = reference.Value;
				return true;
			}
			else
			{
				value = null;
				return false;
			}
		}

		public bool TryLock(uint key)
		{
			if (this.m_map.TryGetValue(key, out var reference))
			{
				reference.Counter++;
				return true;
			}
			else
			{
				return false;
			}
		}

		public T Unlock(uint key)
		{
			if (this.m_map.TryRemove(key, out var reference))
			{
				return reference.Value;
			}

			return null;
		}
	}
}
