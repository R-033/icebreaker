using System;
using System.Diagnostics;
using UnityEngine;

namespace Hypercooled.Managed
{
	class FrameTimer : MonoBehaviour
	{
		public static double AverageFrameTime;
		Stopwatch m_stopwatch;
		double m_timeAccurateLastUpdate;
		double m_timeAccurate;
		double m_elapsed;

		void Awake()
		{
			m_stopwatch = new Stopwatch();
			m_stopwatch.Start();

			AverageFrameTime = 1f;
		}

		void Update()
		{
			double stopwatchTime = m_stopwatch?.ElapsedMilliseconds ?? 0;

			m_timeAccurateLastUpdate = m_timeAccurate;
			m_timeAccurate = stopwatchTime;

			m_elapsed = m_timeAccurate - m_timeAccurateLastUpdate;

			double decay = Math.Pow(0.05, m_elapsed / 1000.0);

			AverageFrameTime = decay * AverageFrameTime + (1.0 - decay) * m_elapsed;
		}
	}
}
