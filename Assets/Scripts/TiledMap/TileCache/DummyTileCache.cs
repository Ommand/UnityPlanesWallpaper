using System;
using UnityEngine;

namespace Tiles
{
	public class DummyTileCache : ITileCache
	{
		public bool Provide(TileInfo info, Texture2D texture)
		{
			return false;
		}

		public void Retrieve(TileInfo info, Action<Texture2D> onTextureLoaded)
		{
			onTextureLoaded(null);
		}
	}
}