using UnityEngine;

namespace Game.GameObjectModifiers
{
	[RequireComponent(typeof(TrailRenderer))]
	public class TrailRendererAutoScaler : BaseAutoScaler
	{
		private const float BaseTrailWidth = 0.33f;
		private TrailRenderer _renderer;

		private TrailRenderer TrailRenderer => _renderer ? _renderer : _renderer = GetComponent<TrailRenderer>();

		protected override void Scale(float scaler)
		{
			TrailRenderer.startWidth = BaseTrailWidth * scaler;
			TrailRenderer.endWidth = BaseTrailWidth * scaler;
		}
	}
}