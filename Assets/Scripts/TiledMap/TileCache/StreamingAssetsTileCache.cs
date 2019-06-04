using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Tiles
{
	public class StreamingAssetsTileCache : ITileCache
	{
		private const string CacheFolderName = "TileCache";
		private readonly string _cacheFolderPath;

		private string _pathPrefix =
#if UNITY_ANDROID && !UNITY_EDITOR
			"";
#else
			"file://";
#endif

		public StreamingAssetsTileCache(string providerName)
		{
			providerName = providerName ?? throw new ArgumentNullException(nameof(providerName));
			_cacheFolderPath = Path.Combine(Application.streamingAssetsPath, CacheFolderName, providerName);
		}

		private string GetFileName(TileInfo info) => Path.Combine(_cacheFolderPath, $"{info}.png");

		public bool Provide(TileInfo info, Texture2D texture)
		{
			return File.Exists(GetFileName(info));
		}

		public void Retrieve(TileInfo info, Action<Texture2D> onTextureLoaded)
		{
			LoadImageWithCoroutine(info, onTextureLoaded);
		}

		private IEnumerator LoadImageCoroutine(TileInfo info, Action<Texture2D> onTextureLoaded)
		{
			var uri = _pathPrefix + GetFileName(info);
			var www = UnityWebRequestTexture.GetTexture(uri);
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError)
			{
				onTextureLoaded(null);
			}
			else
			{
				var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
				texture.filterMode = FilterMode.Point;

				onTextureLoaded(texture);
			}
		}

		private void LoadImageWithCoroutine(TileInfo info, Action<Texture2D> onTextureLoaded)
		{
			Dispatcher.Instance.StartCoroutine(LoadImageCoroutine(info, onTextureLoaded));
		}
	}
}