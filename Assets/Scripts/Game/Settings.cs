using System;
using UnityEngine;

namespace UnityPlanes
{
	public class Settings : MonoBehaviour
	{
		#region Singleton

		public static Settings Instance { get; private set; }

		private void Awake()
		{
			if (Instance)
				throw new Exception($"There are two instances of {nameof(Settings)} in scene");

			Instance = this;
		}

		#endregion

		#region World Settings

		public const float BaseCameraSize = 15;

		public static float CameraSizeMultiplier =>
			GameMaster.Instance.MainCamera.Camera.orthographicSize / BaseCameraSize;

		[Header("World settings")] [Range(0, 1f)] [SerializeField]
		private float intensity = 0.5f;

		public static float Intensity => Instance.intensity;

		#endregion

		#region Plane settings

		[Header("Plane settings")] [Range(0.1f, 10), SerializeField]
		private float minPlaneSpeed;

		[Range(0.1f, 10)] [SerializeField] private float maxPlaneSpeed;

		[Range(0.05f, 0.25f)] [SerializeField] private float minPlaneSize;
		[Range(0.05f, 0.25f)] [SerializeField] private float maxPlaneSize;

		[Range(0.1f, 10)] [SerializeField] private float planeLandingAnimationDuration;

		public static float MinPlaneSpeed => Instance.minPlaneSpeed * CameraSizeMultiplier;

		public static float MaxPlaneSpeed => Instance.maxPlaneSpeed * CameraSizeMultiplier;

		public static float MinPlaneSize => Instance.minPlaneSize;

		public static float MaxPlaneSize => Instance.maxPlaneSize;

		public static float PlaneLandingAnimationDuration => Instance.planeLandingAnimationDuration;

		#endregion

		#region Flights Settigns

		[Header("Flight settings")] [Range(0.0f, 1f)] [SerializeField]
		private float landingProbability = 0.25f;

		[Range(0.0f, 1f)] [SerializeField] private float takeoffProbability = 0.25f;
		[Range(0.0f, 1f)] [SerializeField] private float passingTurnProbability = 0.8f;

		[Range(0.1f, 10)] [SerializeField] private float safeDistanceMultiplier = 1.5f;
		[Range(1e-2f, 5e-2f)] [SerializeField] private float takeoffDist = 3e-2f;

		public static float LandingProbability => Instance.landingProbability;

		public static float TakeoffProbability => Instance.takeoffProbability;

		public static float PassingTurnProbability => Instance.passingTurnProbability;

		public static float SafeDistanceMultiplier => Instance.safeDistanceMultiplier;

		public static float TakeoffDist => Instance.takeoffDist * CameraSizeMultiplier;

		#endregion

		private void OnValidate()
		{
			if (minPlaneSize > maxPlaneSize) maxPlaneSize = minPlaneSize;
			if (minPlaneSpeed > maxPlaneSpeed) maxPlaneSpeed = minPlaneSpeed;

			if (landingProbability + takeoffProbability > 1)
				takeoffProbability = 1 - landingProbability;
		}
	}
}