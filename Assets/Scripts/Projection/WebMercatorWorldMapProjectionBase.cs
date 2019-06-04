using System;
using UnityEngine;

namespace WorldProjection
{
	public class WebMercatorWorldMapProjectionBase : IWorldMapProjection
	{
		private const float LatMax = 1.484422F;

		public Vector2 WorldToLatLon(Vector2 worldPos)
		{
			var (lat, lon) = WorldToLatLon(worldPos.x, worldPos.y);
			return new Vector2(lat, lon);
		}

		public (float, float) WorldToLatLon(float x, float y)
		{
			if (Mathf.Clamp01(x) != x)
				throw new ArgumentOutOfRangeException(nameof(x), $"{nameof(x)} must be in range [0,1] (Value: {x})");
			if (Mathf.Clamp01(y) != y)
				throw new ArgumentOutOfRangeException(nameof(y), $"{nameof(y)} must be in range [0,1] (Value: {y})");

			var lat = -1 / 2.0f * (-Mathf.PI + 4 * Mathf.Atan(Mathf.Exp(Mathf.PI - 2 * Mathf.PI * y)));
			var lon = Mathf.PI * (2 * x - 1);
			return (lat, lon);
		}

		/// <summary>
		/// World is (0,0) -> (1,1) rect
		/// </summary>
		/// <param name="latLon"></param>
		/// <returns></returns>
		public Vector2 LatLonToWorld(Vector2 latLon)
		{
			var (x, y) = LatLonToWorld(latLon.x, latLon.y);
			return new Vector2(x, y);
		}

		public (float, float) LatLonToWorld(float lat, float lon)
		{
			if (Mathf.Clamp(lat,LonLatLimits.yMin,LonLatLimits.yMax) != lat || Mathf.Clamp(lon,LonLatLimits.xMin,LonLatLimits.xMax) != lon)
				throw new ArgumentOutOfRangeException($"Lat lon must be in {LonLatLimits}");

			var x = 1 / (2 * Mathf.PI) * (lon + Mathf.PI);
			var y = 1 / (2 * Mathf.PI) * (Mathf.PI - Mathf.Log(Mathf.Tan(Mathf.PI / 4 - lat / 2)));
			return (x, y);
		}

		public Rect LonLatLimits { get; } = new Rect(-Mathf.PI, -LatMax, Mathf.PI * 2, LatMax * 2);
		public Rect WorldLimits { get; } = new Rect(0, 0, 1, 1);

		public ProjectionType Type => ProjectionType.Merkator;
	}
}