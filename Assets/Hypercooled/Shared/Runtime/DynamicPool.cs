namespace Hypercooled.Shared.Runtime
{
	public class DynamicPool<T>
	{
		private readonly T[] m_pool;
		private readonly int m_size;
		private readonly object m_lock;
		private int m_position;

		public int Count => this.m_size;

		public DynamicPool(int capacity)
		{
			if (capacity <= 0x10)
			{
				capacity = 0x10;
			}

			this.m_pool = new T[capacity];
			this.m_size = capacity;
			this.m_lock = new object();
		}

		public bool Push(T value)
		{
			lock (this.m_lock)
			{
				if (this.m_position == this.m_size)
				{
					return false;
				}
				else
				{
					this.m_pool[this.m_position++] = value;
					return true;
				}
			}
		}

		public bool Pull(out T result)
		{
			lock (this.m_lock)
			{
				if (this.m_position == 0)
				{
					result = default;
					return false;
				}
				else
				{
					result = this.m_pool[--this.m_position];
					this.m_pool[this.m_position] = default;
					return true;
				}
			}
		}
	}
}
