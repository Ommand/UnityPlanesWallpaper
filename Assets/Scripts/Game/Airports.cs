using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AirportData;
using Coordinates;
using KdTree;
using KdTree.Math;
using UnityEngine;
using Utils;
using Debug = UnityEngine.Debug;

namespace Game
{
	public class Airports : MonoBehaviour
	{
		[SerializeField] private GameObject airportPrefab;
		[SerializeField] private Transform airportsTransform;

		[SerializeField] private Camera viewCamera;

		private KdTree<float, AirportInfo> _airportsTree;
		private HashSet<GameObject> _loadedAirports = new HashSet<GameObject>();
		private (float size, Vector3 pos, Rect pixelRect) _lastCameraProps;

		public IReadOnlyList<AirportInfo> VisibleAirports => _visibleAirports;

		private void Awake()
		{
			airportsTransform = airportsTransform ? airportsTransform : transform;

			BuildAirportsTree();
		}

		private void BuildAirportsTree()
		{
			_airportsTree = new KdTree<float, AirportInfo>(2, new GeoMath());

			foreach (var airport in _airportsProvider.Airports.Where(airportInfo =>
				WorldCoordinateHelper.IsLonLatAllowed(airportInfo.LonLat)))
				_airportsTree.Add(new[] {airport.LonLat.Lat.DegValue, airport.LonLat.Lon.DegValue}, airport);
		}

		private void Update()
		{
			UpdateAirportsView();
		}

		private void UpdateAirportsView()
		{
			if (!airportPrefab || !viewCamera || !IsCameraUpdated) return;

			var centerWorld = viewCamera.transform.position;
			var center = WorldCoordinateHelper.WorldToLonLat(centerWorld);

			var cornerWorld = viewCamera.ViewportToWorldPoint(center.Lat.RadValue < 0 ? Vector3.zero : Vector3.one);
			var corner = WorldCoordinateHelper.WorldToLonLat(cornerWorld);

			var radius = center.DistanceTo(corner);

			var sw = Stopwatch.StartNew();
			var visibleAirports =
				_airportsTree.RadialSearch(new[] {center.Lat.DegValue, center.Lon.DegValue}, radius / 1000);
			sw.Stop();

			foreach (var airport in _loadedAirports) SimplePool.Despawn(airport);

			_loadedAirports.Clear();
			_visibleAirports.Clear();

			foreach (var airport in visibleAirports)
			{
				var spawned = SimplePool.Spawn(airportPrefab, Vector3.zero, Quaternion.identity);
				spawned.transform.SetParent(airportsTransform, false);
				spawned.transform.localPosition = WorldCoordinateHelper.LonLatToWorld(airport.Value.LonLat);
				_loadedAirports.Add(spawned);

				_visibleAirports.Add(airport.Value);
			}
		}

		private bool IsCameraUpdated
		{
			get
			{
				if (_lastCameraProps == (viewCamera.orthographicSize, viewCamera.transform.position,
					    viewCamera.pixelRect))
					return false;
				_lastCameraProps = (viewCamera.orthographicSize, viewCamera.transform.position, viewCamera.pixelRect);

				return true;
			}
		}

		private readonly IAirportDataProvider _airportsProvider = new DefaultAirportDataProvider();
		private readonly List<AirportInfo> _visibleAirports = new List<AirportInfo>();
	}
}