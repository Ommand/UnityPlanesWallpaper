namespace Tiles
{
	public interface IOnlineMapUrlProvider
	{
		string GetUrl(TileInfo info);
		string Name { get; }
	}
}