namespace Tiles
{
	public class OpenStreetMapCartocdnUrlProvider : IOnlineMapUrlProvider
	{
		public string GetUrl(TileInfo info) =>
			$"https://a.basemaps.cartocdn.com/{Id}/{info.Zoom}/{info.Col}/{info.Row}@{Scale}.png";

		private string Id => "dark_all";

		private int Scale { get; set; } = 1;

		public string Name => $"OpenStreetMap-Cartocdn-{Id}-{Scale}";
	}
}