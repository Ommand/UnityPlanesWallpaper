using UnityEngine;

namespace Game.GameObjectModifiers
{
	public class AutoScaler : BaseAutoScaler
	{
		protected override void Scale(float scaler)
		{
			transform.localScale = Vector3.one * scaler;
		}
	}
}