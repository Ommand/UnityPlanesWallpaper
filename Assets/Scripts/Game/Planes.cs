using System.Collections.Generic;
using System.Linq;
using AirportData;
using Coordinates;
using Game.Flight;
using UnityEngine;

namespace UnityPlanes
{
	public class Planes : MonoBehaviour
	{
		[SerializeField] private List<GameObject> planePrefabs;
		[SerializeField] private Transform planesTransform;

		private readonly HashSet<Plane> _planes = new HashSet<Plane>();
		private readonly HashSet<Plane> _freePlanes = new HashSet<Plane>();
		private readonly IFlightProvider _flightProvider = new DefaultFlightProvider();

		public void CreatePlane(IReadOnlyList<AirportInfo> visibleAirports)
		{
			LaunchPlane(GetPlane(), visibleAirports);
		}

		private void LaunchPlane(Plane plane, IReadOnlyList<AirportInfo> visibleAirports)
		{
			var centerWorld = GameMaster.Instance. MainCamera.transform.position;
			var center = WorldCoordinateHelper.WorldToLonLat(centerWorld);

			var cornerWorld = GameMaster.Instance.MainCamera.Camera.ViewportToWorldPoint(Vector3.one);

			var radius = Vector2.Distance(centerWorld, cornerWorld);
			
			var flight = _flightProvider.CreateFlight( visibleAirports, center, radius);

			plane.Fly(flight);
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
	}
}