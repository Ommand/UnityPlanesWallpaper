using System;
using System.Collections.Generic;

namespace Tiles
{
	public interface ITileObserver
	{
		int TileSize { get; }
		int BaseCols { get; }
		int BaseRows { get; }
		
		void AddViewport(TileObserverViewport viewport);

		event Action<IEnumerable<TileInfo>> OnNewTilesAppear;
		event Action<IEnumerable<TileInfo>> OnNewTilesDisappear;
	}
}