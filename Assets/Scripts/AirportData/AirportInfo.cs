using Coordinates;

namespace AirportData
{
	public class AirportInfo
	{
		public string Name { get; }
		public string City { get; }
		public string Country { get; }

		// ReSharper disable once InconsistentNaming
		public string IATA { get; }

		// ReSharper disable once InconsistentNaming
		public string ICAO { get; }

		public LonLat LonLat { get; }

		/// <summary>
		/// Creates new AirportInfo object
		/// </summary>
		/// <param name="name"></param>
		/// <param name="city"></param>
		/// <param name="country"></param>
		/// <param name="iata"></param>
		/// <param name="icao"></param>
		/// <param name="lat">Deg value</param>
		/// <param name="lon">Deg value</param>
		public AirportInfo(string name, string city, string country, string iata, string icao, float lat, float lon)
		{
			Name = name;
			City = city;
			Country = country;
			IATA = iata;
			ICAO = icao;
			LonLat = new LonLat(Coordinate.FromDeg(lon), Coordinate.FromDeg(lat));
		}
	}
}