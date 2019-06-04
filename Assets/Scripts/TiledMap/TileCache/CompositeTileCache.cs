using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tiles
{
	public class CompositeTileCache : ITileCache
	{
		private readonly List<ITileCache> _caches;

		public CompositeTileCache(IEnumerable<ITileCache> caches)
		{
			_caches = new List<ITileCache>(caches ?? Enumerable.Empty<ITileCache>());
		}

		public CompositeTileCache(params ITileCache[] caches)
		{
			_caches = caches?.ToList() ?? new List<ITileCache>();
		}

		public bool Provide(TileInfo info, Texture2D texture)
		{
			return _caches.Any(cache => cache.Provide(info, texture));
		}

		public void Retrieve(TileInfo info, Action<Texture2D> onTextureLoaded)
		{
			var cachesChecked = 0;

			void CheckNext()
			{
				_caches[cachesChecked++].Retrieve(info,texture =>
				{
					if (texture)
						onTextureLoaded(texture);
					else if (cachesChecked >= _caches.Count)
						onTextureLoaded(null);
					else
						CheckNext();
				});
			}
			
			CheckNext();
		}
	}
}