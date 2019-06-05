using System.Collections.Generic;
using AirportData;
using Coordinates;
using UnityEngine;

namespace Game.Flight
{
	public class DefaultFlightProvider : IFlightProvider
	{
		private const float LandingProbability = 0.3f;
		private const float TakeoffProbability = 0.3f;
		private const float PassingTurnProbability = 0.75f;

		private const float SafeDistanceMultiplier = 2f;
		private const float TakeoffDist = 100_000;

		public FlightInfo CreateFlight(IReadOnlyList<AirportInfo> availableAirports, LonLat viewCenter, float radius)
		{
			var prob = Random.Range(0, 1f);

			if (prob < LandingProbability)
				return CreateLandingFlight(availableAirports, viewCenter, radius);

			if (prob < TakeoffProbability + LandingProbability)
				return CreateTakeoffFlight(availableAirports, viewCenter, radius);

			return CreatePassingFlight(availableAirports, viewCenter, radius);
		}

		//TODO Replace with spherical calculations
		private FlightInfo CreateTakeoffFlight(IReadOnlyList<AirportInfo> availableAirports, LonLat viewCenter, float radius)
		{
			var airport = availableAirports.PickRandom();
			var dir = GetTakeoffDirection(airport);

			var startPoint = WorldCoordinateHelper.LonLatToWorld(airport.LonLat);
			var turnPoint = startPoint + dir * TakeoffDist;
			var turnPointSpherical = WorldCoordinateHelper.WorldToLonLat(turnPoint);

			var endPoint = WorldCoordinateHelper.LonLatToWorld(viewCenter) +
			               Random.insideUnitCircle.normalized * (SafeDistanceMultiplier * radius);
			var endPointSpherical = WorldCoordinateHelper.WorldToLonLat(endPoint);

			return new FlightInfo(new[] {airport.LonLat, turnPointSpherical, endPointSpherical}, true, false);
		}

		private FlightInfo CreateLandingFlight(IReadOnlyList<AirportInfo> availableAirports, LonLat viewCenter, float radius)
		{
			var airport = availableAirports.PickRandom();
			var dir = -GetTakeoffDirection(airport);

			var startPoint = WorldCoordinateHelper.LonLatToWorld(viewCenter) +
			                 Random.insideUnitCircle.normalized * (SafeDistanceMultiplier * radius);
			var startPointSpherical = WorldCoordinateHelper.WorldToLonLat(startPoint);

			var endPoint = WorldCoordinateHelper.LonLatToWorld(airport.LonLat);
			var turnPoint = endPoint + dir * TakeoffDist;
			var turnPointSpherical = WorldCoordinateHelper.WorldToLonLat(turnPoint);

			return new FlightInfo(new[] {startPointSpherical, turnPointSpherical, airport.LonLat}, false, true);
		}

		private FlightInfo CreatePassingFlight(IReadOnlyList<AirportInfo> availableAirports, LonLat viewCenter, float radius)
		{
			var center = WorldCoordinateHelper.LonLatToWorld(viewCenter);
			var startPoint = center + Random.insideUnitCircle.normalized * (SafeDistanceMultiplier * radius);
			var dir = (center - startPoint).normalized;

			Vector2? turnPoint = null;
			Vector2 endPoint;


			if (Random.Range(0, 1f) < PassingTurnProbability)
			{
				turnPoint = center + Random.insideUnitCircle * (0.3f * radius);
				endPoint = center + dir * (2 * SafeDistanceMultiplier) +
				           (Vector2) (Vector3.Cross(dir, Vector3.up) * Random.Range(-3 * radius, 3 * radius));
			}
			else
			{
				endPoint = center + dir * (2 * SafeDistanceMultiplier) +
				           (Vector2) (Vector3.Cross(dir, Vector3.up) * Random.Range(-1.5f * radius, 1.5f * radius));
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
			var angle = (airportInfo.IATA.GetHashCode() % 360) * Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}
	}
}