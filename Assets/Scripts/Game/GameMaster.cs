using System;
using System.Collections;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityPlanes
{
	public class GameMaster : MonoBehaviour
	{
		#region Singleton

		public static GameMaster Instance { get; private set; }

		private void Awake()
		{
			if (Instance)
				throw new Exception($"There are two instances of {nameof(GameMaster)} in scene");

			Instance = this;
		}

		#endregion

		[SerializeField] private CameraEvents mainCamera;
		[SerializeField] private Airports airports;
		[SerializeField] private Planes planes;

		private const float SpawnTimeout = 1;

		public CameraEvents MainCamera => mainCamera;

		private void Start()
		{
			StartCoroutine(SpawnLoop());
		}

		private IEnumerator SpawnLoop()
		{
			while (true)
			{
				yield return new WaitForSeconds(SpawnTimeout);
				
				if (ShouldSpawnPlane)
					CreatePlane();
			}
		}

		public void CreatePlane()
		{
			planes.CreatePlane(airports.VisibleAirports);
		}

		private bool ShouldSpawnPlane => Random.Range(0, 1f) < ProbabilityFunc(planes.ActiveCount);

		private static float ProbabilityFunc(float planesActive) =>
			(Mathf.Exp(1.0f / Mathf.Pow( planesActive, 1-Settings.Intensity/2)) - 1.0f) *
			(1 - Mathf.Pow(planesActive / ((Settings.Intensity * 0.75f + 0.25f) * 30 + 15), 4));
	}
}