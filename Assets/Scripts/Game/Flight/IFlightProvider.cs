using System.Collections.Generic;
using AirportData;
using Coordinates;

namespace Game.Flight
{
	public interface IFlightProvider
	{
		FlightInfo CreateFlight(IReadOnlyList<AirportInfo> availableAirports, LonLat viewCenter, float radius);
	}
}