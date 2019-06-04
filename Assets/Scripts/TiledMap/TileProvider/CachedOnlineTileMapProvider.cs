using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tiles
{
	public class CachedOnlineTileMapProvider : OnlineMapTileProvider
	{
		private readonly ITileCache _cache;
//		private readonly Texture2D _thumb;

		public CachedOnlineTileMapProvider(IOnlineMapUrlProvider urlProvider, ITileCache cache = null) : base(
			urlProvider)
		{
			_cache = cache ?? new DummyTileCache();

//			_thumb = new Texture2D(TileSize, TileSize);
//			_thumb.SetPixels(Enumerable.Repeat(Color.clear, TileSize * TileSize).ToArray());
//			_thumb.Apply(true);
		}

		public override void LoadThumb(TileInfo info, Action<Texture2D> onLoaded)
		{
//			onLoaded(_thumb);
		}

		public override void LoadTexture(TileInfo info, Action<Texture2D> onLoaded)
		{
			_cache.Retrieve(info, cached =>
			{
				if (cached != null)
					onLoaded(cached);
				else
					LoadTextureFromWeb(info, texture =>
					{
						_cache.Provide(info,texture);
						onLoaded(texture);
					});
			});
		}

		~CachedOnlineTileMapProvider()
		{
//			if (_thumb) Object.Destroy(_thumb);
		}
	}
}