using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Tiles
{
	public abstract class OnlineMapTileProvider : ITileProvider
	{
		private const int MaxConsequtiveDownloads = 2;
		
		private int _currentDownloadCount = 0;

		private readonly IOnlineMapUrlProvider _urlProvider;
		private readonly Dictionary<TileInfo, Texture2D> _usedTiles = new Dictionary<TileInfo, Texture2D>();

		private readonly HashSet<(TileInfo, Action<Texture2D>)>
			_downloadQueue = new HashSet<(TileInfo, Action<Texture2D>)>();

		public abstract void LoadThumb(TileInfo info, Action<Texture2D> onLoaded);
		public abstract void LoadTexture(TileInfo info, Action<Texture2D> onLoaded);

		public string Name => _urlProvider.Name;

		public void RequestTileTexture(TileInfo info, Action<Texture2D> onThumbReady,
			Action<Texture2D> onMainTextureReady)
		{
			LoadThumb(info, onThumbReady);
			LoadTexture(info, onMainTextureReady);
		}

		public void FreeTile(TileInfo info)
		{
			if (_usedTiles.ContainsKey(info))
			{
				Object.Destroy(_usedTiles[info]);
				_usedTiles.Remove(info);
			}

			_downloadQueue.RemoveWhere(el => el.Item1.Equals(info));
		}

		protected void LoadTextureFromWeb(TileInfo info, Action<Texture2D> onTextureDownloaded)
		{
			Dispatcher.Instance.StartCoroutine(LoadTextureFromWebCoroutine(info, onTextureDownloaded));
		}

		private IEnumerator LoadTextureFromWebCoroutine(TileInfo info, Action<Texture2D> onTextureDownloaded)
		{
			if (_currentDownloadCount >= MaxConsequtiveDownloads)
			{
				_downloadQueue.Add((info, onTextureDownloaded));
				yield break;
			}

			_currentDownloadCount++;

			var www = UnityWebRequestTexture.GetTexture(_urlProvider.GetUrl(info));
			yield return www.SendWebRequest();

			_currentDownloadCount--;
			TryToDequeueDownload();

			if (www.isNetworkError || www.isHttpError)
				Debug.LogError($"{www.error} [{www.url}] {www.responseCode}");
			else
			{
				var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
				texture.filterMode = FilterMode.Point;
				if (_usedTiles.ContainsKey(info))
				{
					Object.Destroy(_usedTiles[info]);
					_usedTiles.Remove(info);
				}

				_usedTiles.Add(info, texture);
				onTextureDownloaded(texture);
			}
		}

		private void TryToDequeueDownload()
		{
			if (_downloadQueue.Any())
			{
				var enqueuedElement = _downloadQueue.First();
				_downloadQueue.Remove(enqueuedElement);
				LoadTextureFromWeb(enqueuedElement.Item1, enqueuedElement.Item2);
			}
		}

		protected OnlineMapTileProvider(IOnlineMapUrlProvider urlProvider)
		{
			_urlProvider = urlProvider ?? throw new ArgumentNullException(nameof(urlProvider));
		}
	}
}