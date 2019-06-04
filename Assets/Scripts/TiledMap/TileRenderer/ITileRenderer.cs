using System;

namespace Tiles
{
	public interface ITileRenderer
	{
		ITileProvider Provider { get; set; }
		void RenderTile(TileInfo info);

		void DestroyTile(TileInfo info);
		event Action OnViewUpdate;
	}
}