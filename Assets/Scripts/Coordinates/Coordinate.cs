using UnityEngine;

namespace Coordinates
{
	public struct Coordinate
	{
		private Coordinate(float radValue)
		{
			RadValue = radValue;
		}

		public static Coordinate FromRad(float radValue) => new Coordinate(radValue);
		public static Coordinate FromDeg(float value) => new Coordinate(value * Mathf.Deg2Rad);

		public float RadValue { get; }

		public float DegValue => RadValue * Mathf.Rad2Deg;
	}
}