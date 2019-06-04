using System;
using UnityEngine;

namespace Tiles
{
	public interface ITileCache
	{
		bool Provide(TileInfo info, Texture2D texture);
		void Retrieve(TileInfo info, Action<Texture2D> onTextureLoaded);
	}
}