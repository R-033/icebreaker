using Hypercooled.Shared.Core;
using UnityEngine;

namespace Hypercooled.Managed.Mono
{
	public class FlareObjectMonoWrapper : ObjectMonoWrapper<FlareObject>
	{
		private FlareObject m_wrappedFlare;

		public override FlareObject Wrapped => this.m_wrappedFlare;

		public override void Initialize(FlareObject flareObject)
		{
			this.m_wrappedFlare = flareObject;
		}
		public override void ResetData()
		{
			this.m_wrappedFlare = null;
		}
		public override void UpdateData()
		{

		}
	}
}
