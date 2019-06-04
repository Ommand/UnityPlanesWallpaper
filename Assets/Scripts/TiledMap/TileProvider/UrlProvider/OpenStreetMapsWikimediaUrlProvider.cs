namespace Tiles
{
	public class OpenStreetMapsWikimediaUrlProvider : IOnlineMapUrlProvider
	{
		public string GetUrl(TileInfo info) => $"https://maps.wikimedia.org/osm-intl/{info.Zoom}/{info.Col}/{info.Row}.png";
		public string Name => "OpenStreetMap-Wikimedia";
	}
}