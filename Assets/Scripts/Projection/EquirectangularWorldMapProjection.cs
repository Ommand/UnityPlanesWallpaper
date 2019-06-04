using UnityEngine;

namespace WorldProjection
{
	public class EquirectangularWorldMapProjection : IWorldMapProjection
	{
		private Rect _world;

		private static Rect DefaultWorld { get; set; } = new Rect(-Mathf.PI, -1.56207F, Mathf.PI * 2, 1.56207F * 2);

		public Vector2 WorldToLatLon(Vector2 worldPos)
		{
			var (lat, lon) = WorldToLatLon(worldPos.x, worldPos.y);
			return new Vector2(lat, lon);
		}

		public (float, float) WorldToLatLon(float x, float y)
		{
			return (
				Mathf.Lerp(-Mathf.PI / 2, Mathf.PI / 2, Mathf.InverseLerp(_world.yMin, _world.yMax, y)),
				Mathf.Lerp(-Mathf.PI, Mathf.PI, Mathf.InverseLerp(_world.xMin, _world.xMax, x))
			);
		}

		public Vector2 LatLonToWorld(Vector2 latLon)
		{
			var (x, y) = LatLonToWorld(latLon.x, latLon.y);
			return new Vector2(x, y);
		}

		public (float, float) LatLonToWorld(float lat, float lon)
		{
			return (
				Mathf.Lerp(_world.xMin, _world.xMax, Mathf.InverseLerp(-Mathf.PI, Mathf.PI, lon)),
				Mathf.Lerp(_world.yMin, _world.yMax, Mathf.InverseLerp(-Mathf.PI / 2, Mathf.PI / 2, lat))
			);
		}

		public Rect LonLatLimits => DefaultWorld;
		public Rect WorldLimits => _world;

		public ProjectionType Type => ProjectionType.Equirectangular;

		public EquirectangularWorldMapProjection(Rect world)
		{
			_world = world;
		}

		public EquirectangularWorldMapProjection() : this(DefaultWorld)
		{
		}
	}
}