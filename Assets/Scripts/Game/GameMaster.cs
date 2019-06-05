using System;
using System.Collections.Generic;
using System.Linq;
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

		[SerializeField] private List<GameObject> planePrefabs;
		[SerializeField] private Transform planesTransform;
		[SerializeField] private CameraEvents mainCamera;

		private readonly HashSet<Plane> _planes = new HashSet<Plane>();
		private readonly HashSet<Plane> _freePlanes = new HashSet<Plane>();

		private float _lastCameraSize = 0;

		public CameraEvents MainCamera => mainCamera;

		public void CreatePlane()
		{
			var plane = GetPlane();
			LaunchPlane(plane);
		}

		private void LaunchPlane(Plane plane)
		{
			var (from, path) = GetRandomFlight();
			plane.transform.position = @from;
			plane.Fly(path);
		}

		private Plane GetPlane()
		{
			if (_freePlanes.Any())
			{
				var plane = _freePlanes.First();
				_freePlanes.Remove(plane);
				plane.gameObject.SetActive(true);
				return plane;
			}

			var newPlane = Instantiate(GetPlanePrefab(), planesTransform).GetComponent<Plane>();
			newPlane.OnFlightFinished += () => OnPlaneFlightFinished(newPlane);
			_planes.Add(newPlane);
			return newPlane;
		}

		private void OnPlaneFlightFinished(Plane plane)
		{
			_freePlanes.Add(plane);
			plane.gameObject.SetActive(false);
		}

		private GameObject GetPlanePrefab()
		{
			return planePrefabs.PickRandom();
		}

		private (Vector3, Vector3[]) GetRandomFlight()
		{
			return (
				new Vector3(Random.Range(0, 256.0f), Random.Range(0, 256.0f), 0),
				new[]
				{
					new Vector3(Random.Range(0, 256.0f), Random.Range(0, 256.0f), 0),
					new Vector3(Random.Range(0, 256.0f), Random.Range(0, 256.0f), 0),
					new Vector3(Random.Range(0, 256.0f), Random.Range(0, 256.0f), 0)
				});
		}
	}
}