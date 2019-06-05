using UnityEngine;
using WorldProjection;

namespace Coordinates
{
	public static class WorldCoordinateHelper
	{
		private const float WorldSize = 256;

		private static readonly IWorldMapProjection Projection = new WebMercatorWorldMapProjectionBase();

		public static LonLat WorldToLonLat(Vector2 worldPos)
		{
			var latLonVector = Projection.WorldToLatLon(worldPos / WorldSize);
			return new LonLat(Coordinate.FromRad(latLonVector.y), Coordinate.FromRad(latLonVector.x));
		}

		public static Vector2 LonLatToWorld(LonLat lonLat)
		{
			return Projection.LatLonToWorld(new Vector2(lonLat.Lat.RadValue, lonLat.Lon.RadValue)) * WorldSize;
		}

		public static bool IsLonLatAllowed(LonLat lonLat)
		{
			return Projection.LonLatLimits.Contains(new Vector2(lonLat.Lon.RadValue, lonLat.Lat.RadValue));
		}
	}
}