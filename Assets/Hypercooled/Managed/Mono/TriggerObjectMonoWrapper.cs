using Hypercooled.Shared.Core;
using UnityEngine;

namespace Hypercooled.Managed.Mono
{
	public class TriggerObjectMonoWrapper : ObjectMonoWrapper<TriggerObject>
	{
		private TriggerObject m_wrappedTrigger;

		public override TriggerObject Wrapped => this.m_wrappedTrigger;

		public override void Initialize(TriggerObject triggerObject)
		{
			this.m_wrappedTrigger = triggerObject;
		}
		public override void ResetData()
		{
			this.m_wrappedTrigger = null;
		}
		public override void UpdateData()
		{

		}
	}
}
