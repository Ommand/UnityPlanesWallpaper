using System;
using UnityEngine;

namespace WorldProjection
{
	public interface IWorldMapProjection
	{
		Vector2 WorldToLatLon(Vector2 worldPos);
		(float, float) WorldToLatLon(float x, float y);

		/// <summary>
		/// (Latitude, Longitude) in radians
		/// </summary>
		/// <param name="latLon"></param>
		/// <returns></returns>
		Vector2 LatLonToWorld(Vector2 latLon);
		(float, float) LatLonToWorld(float lat, float lon);

		Rect LonLatLimits { get; }
		Rect WorldLimits { get; }
		
		ProjectionType Type { get; }
	}
}