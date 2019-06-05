using System;
using UnityEngine;

namespace Coordinates
{
	public struct LonLat
	{
		private const float EarthRadius = 6371000f;
		
		public Coordinate Lon { get; }
		public Coordinate Lat { get; }

		public LonLat(Coordinate lon, Coordinate lat)
		{
			Lon = lon;
			Lat = lat;
		}

		public Vector2 DegValue => new Vector2(Lon.DegValue, Lat.DegValue);
		public Vector2 RadValue => new Vector2(Lon.RadValue, Lat.RadValue);

		public static LonLat Zero => new LonLat(Coordinate.FromRad(0), Coordinate.FromRad(0));

		/// <summary>
		/// Returns the distance between two coordinates
		/// </summary>
		/// <param name="to">Second point</param>
		/// <returns>Meters value</returns>
		public float DistanceTo(LonLat to)
		{
			var angle = Math.Sin(Lat.RadValue) * Math.Sin(to.Lat.RadValue) + Math.Cos(Lat.RadValue) *
			            Math.Cos(to.Lat.RadValue) * Math.Cos(Lon.RadValue - to.Lon.RadValue);
			angle = Math.Acos(angle);

			return EarthRadius * (float) angle;
		}

		public override string ToString()
		{
			return $"[{Lat.DegValue}, {Lon.DegValue}]";
		}
	}
}