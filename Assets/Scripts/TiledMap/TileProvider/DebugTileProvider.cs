using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tiles
{
	public class DebugTileProvider : ITileProvider
	{
		private readonly Dictionary<TileInfo, Texture2D> _usedTiles = new Dictionary<TileInfo, Texture2D>();

		private Texture2D GetTileTexture(TileInfo info)
		{
			var texture2D = new Texture2D(info.Size, info.Size);

			var pixels = texture2D.GetPixels();

			for (int i = 0; i < pixels.Length; i++)
			{
				var col = i % info.Size;
				var row = i / info.Size;
				var bound = info.Size / 10;
				pixels[i] = (col <= bound || row <= bound || col >= info.Size - bound || row >= info.Size - bound)
					? Color.black
					: Color.red;
			}

			texture2D.SetPixels(pixels);
			texture2D.Apply(true);

			_usedTiles.Add(info, texture2D);
			return texture2D;
		}

		public string Name => nameof(DebugTileProvider);

		public void RequestTileTexture(TileInfo info, Action<Texture2D> onThumbReady,
			Action<Texture2D> onMainTextureReady)
		{
			onMainTextureReady?.Invoke(GetTileTexture(info));
		}

		public void FreeTile(TileInfo info)
		{
			Object.Destroy(_usedTiles[info]);
			_usedTiles.Remove(info);
		}
	}
}