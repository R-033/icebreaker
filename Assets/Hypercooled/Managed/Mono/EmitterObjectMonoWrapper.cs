using Hypercooled.Shared.Core;
using UnityEngine;

namespace Hypercooled.Managed.Mono
{
	public class EmitterObjectMonoWrapper : ObjectMonoWrapper<EmitterObject>
	{
		private EmitterObject m_wrappedEmitter;

		public override EmitterObject Wrapped => this.m_wrappedEmitter;

		public override void Initialize(EmitterObject emitterObject)
		{
			this.m_wrappedEmitter = emitterObject;
		}
		public override void ResetData()
		{
			this.m_wrappedEmitter = null;
		}
		public override void UpdateData()
		{

		}
	}
}
