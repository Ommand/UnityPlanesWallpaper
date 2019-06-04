namespace Tiles
{
	public class OpenStreetMapStandartUrlProvider : IOnlineMapUrlProvider
	{
		public string GetUrl(TileInfo info) => $"https://c.tile.openstreetmap.org/{info.Zoom}/{info.Col}/{info.Row}.png";
		public string Name => "OpenStreetMap-Standart";
	}
}