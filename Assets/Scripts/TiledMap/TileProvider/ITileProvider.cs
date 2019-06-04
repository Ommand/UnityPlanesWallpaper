using System;
using UnityEngine;

namespace Tiles
{
	public interface ITileProvider
	{
		string Name { get; }
		void RequestTileTexture(TileInfo info, Action<Texture2D> onThumbReady, Action<Texture2D> onMainTextureReady);
		void FreeTile(TileInfo info);
	}
}