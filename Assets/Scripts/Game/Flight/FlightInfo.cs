using Coordinates;

namespace Game.Flight
{
	public struct FlightInfo
	{
		public FlightInfo(LonLat[] points, bool startsOnGround = false, bool endsOnGround = false)
		{
			StartsOnGround = startsOnGround;
			EndsOnGround = endsOnGround;
			Points = points;
		}

		public bool StartsOnGround { get; private set; }
		public bool EndsOnGround { get; private set; }

		public LonLat[] Points { get; private set; }
	}
}