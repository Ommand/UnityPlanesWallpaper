using System.Collections.Generic;
using AirportData;
using Coordinates;
using UnityEngine;
using UnityPlanes;

namespace Game.Flight
{
	public class DefaultFlightProvider : IFlightProvider
	{
		private float TakeoffDist => WorldCoordinateHelper.WorldSize * Settings.TakeoffDist;

		public FlightInfo CreateFlight(IReadOnlyList<AirportInfo> availableAirports, LonLat viewCenter, float radius)
		{
			var prob = Random.Range(0, 1f);

			if (prob < Settings.LandingProbability)
				return CreateLandingFlight(availableAirports, viewCenter, radius);

			if (prob < Settings.TakeoffProbability + Settings.LandingProbability)
				return CreateTakeoffFlight(availableAirports, viewCenter, radius);

			return CreatePassingFlight(availableAirports, viewCenter, radius);
		}

		//TODO Replace with spherical calculations
		private FlightInfo CreateTakeoffFlight(IReadOnlyList<AirportInfo> availableAirports, LonLat viewCenter,
			float radius)
		{
			var airport = availableAirports.PickRandom();
			var dir = GetTakeoffDirection(airport);

			var startPoint = WorldCoordinateHelper.LonLatToWorld(airport.LonLat);
			var turnPoint = startPoint + dir * TakeoffDist;
			var turnPointSpherical = WorldCoordinateHelper.WorldToLonLat(turnPoint);

			var endPoint = WorldCoordinateHelper.LonLatToWorld(viewCenter) +
			               Random.insideUnitCircle.normalized * (Settings.SafeDistanceMultiplier * radius);
			var endPointSpherical = WorldCoordinateHelper.WorldToLonLat(endPoint);

			return new FlightInfo(new[] {airport.LonLat, turnPointSpherical, endPointSpherical}, true, false);
		}

		private FlightInfo CreateLandingFlight(IReadOnlyList<AirportInfo> availableAirports, LonLat viewCenter,
			float radius)
		{
			var airport = availableAirports.PickRandom();
			var dir = -GetTakeoffDirection(airport);


			var randomDir = Random.insideUnitCircle.normalized;

			var startPoint = WorldCoordinateHelper.LonLatToWorld(viewCenter) +
			                 randomDir * (Settings.SafeDistanceMultiplier * radius);
			var startPointSpherical = WorldCoordinateHelper.WorldToLonLat(startPoint);

			var endPoint = WorldCoordinateHelper.LonLatToWorld(airport.LonLat);
			var turnPoint = endPoint + dir * TakeoffDist;
			var turnPointSpherical = WorldCoordinateHelper.WorldToLonLat(turnPoint);

			return new FlightInfo(new[] {startPointSpherical, turnPointSpherical, airport.LonLat}, false, true);
		}

		private FlightInfo CreatePassingFlight(IReadOnlyList<AirportInfo> availableAirports, LonLat viewCenter,
			float radius)
		{
			var center = WorldCoordinateHelper.LonLatToWorld(viewCenter);
			var startPoint = center + Random.insideUnitCircle.normalized * (Settings.SafeDistanceMultiplier * radius);
			var dir = (center - startPoint).normalized;
			var angle = Mathf.Atan2(dir.y, dir.x);

			Vector2? turnPoint = null;
			Vector2 endPoint;


			if (Random.Range(0, 1f) < Settings.PassingTurnProbability)
			{
				turnPoint = center + Random.insideUnitCircle * (0.3f * radius);
				const float halfPi = Mathf.PI / 2;
				endPoint = center + GetRandomVectorOnUnitCircle(angle - halfPi, angle + halfPi) *
				           (Settings.SafeDistanceMultiplier * radius);
			}
			else
			{
				const float deg30 = Mathf.Rad2Deg * 30;
				endPoint = center + GetRandomVectorOnUnitCircle(angle - deg30, angle + deg30) *
				           (Settings.SafeDistanceMultiplier * radius);
			}

			var startPointSpherical = WorldCoordinateHelper.WorldToLonLat(startPoint);
			var turnPointSpherical =
				turnPoint.HasValue ? WorldCoordinateHelper.WorldToLonLat(turnPoint.Value) : (LonLat?) null;
			var endPointSpherical = WorldCoordinateHelper.WorldToLonLat(endPoint);

			var points = turnPointSpherical.HasValue
				? new[] {startPointSpherical, turnPointSpherical.Value, endPointSpherical}
				: new[] {startPointSpherical, endPointSpherical};

			return new FlightInfo(points);
		}

		private Vector2 GetTakeoffDirection(AirportInfo airportInfo)
		{
			var hashCode = airportInfo.IATA.GetHashCode();

			var angle = (hashCode % 360) * Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}

		private Vector2 GetRandomVectorOnUnitCircle(float fromRad, float toRad)
		{
			if (fromRad > toRad) (fromRad, toRad) = (toRad, fromRad);

			var angle = Random.Range(fromRad, toRad);
			return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}

	}
}