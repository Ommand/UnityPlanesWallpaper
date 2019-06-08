using UnityEngine;
using UnityPlanes;

namespace Game.GameObjectModifiers
{
	public abstract class BaseAutoScaler : MonoBehaviour
	{
		[Range(1e-4f, 1)] [SerializeField] private float scaleMultiplier = 0.1f;

		private void Start()
		{
			GameMaster.Instance.MainCamera.OnOrthoSizeChanged += MainCameraOnOnOrthoSizeChanged;
			Scale(GameMaster.Instance.MainCamera.Camera.orthographicSize * scaleMultiplier);
		}

		private void OnDestroy()
		{
			if (GameMaster.Instance && GameMaster.Instance.MainCamera)
				GameMaster.Instance.MainCamera.OnOrthoSizeChanged -= MainCameraOnOnOrthoSizeChanged;
		}

		private void MainCameraOnOnOrthoSizeChanged(Camera arg1, float arg2)
		{
			Scale(GameMaster.Instance.MainCamera.Camera.orthographicSize * scaleMultiplier);
		}

		protected abstract void Scale(float scaler);
	}
}