using Hypercooled.Shared.Core;
using UnityEngine;

namespace Hypercooled.Managed.Mono
{
	public class LightObjectMonoWrapper : ObjectMonoWrapper<LightObject>
	{
		private MeshFilter m_meshFilter;
		private MeshRenderer m_meshRenderer;
		private LightObject m_wrappedLight;

		public override LightObject Wrapped => this.m_wrappedLight;

		public override void Initialize(LightObject lightObject)
		{
			if (this.m_meshFilter is null) this.m_meshFilter = this.gameObject.GetComponent<MeshFilter>();
			if (this.m_meshRenderer is null) this.m_meshRenderer = this.gameObject.GetComponent<MeshRenderer>();

			this.m_wrappedLight = lightObject;

			this.transform.position = this.m_wrappedLight.Position;
			this.transform.localScale = new Vector3(this.m_wrappedLight.Radius, this.m_wrappedLight.Radius, this.m_wrappedLight.Radius);
			// todo: rotation - direction

			var block = new MaterialPropertyBlock();
			this.m_meshRenderer.GetPropertyBlock(block);

			block.SetColor("_Color", this.m_wrappedLight.LightColor);
			this.m_meshRenderer.SetPropertyBlock(block);

			// todo : Intensity, FarStart, FarEnd, 
		}
		public override void ResetData()
		{
			this.m_wrappedLight = null;
		}
		public override void UpdateData()
		{

		}
	}
}
