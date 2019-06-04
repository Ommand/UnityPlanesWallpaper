using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Tiles
{
	public class DefaultTileCache : ITileCache
	{
		private const float FileSystemCheckInterval = 5 * 60;
		
		private readonly string _providerName;
		private readonly int _limit;
		private readonly int _alwaysCachedMaxZoom;
		private string _cacheFolderPath;
		private int _fileCount;
		private const string CacheFolderName = "TileCache";

		private float _lastFileSystemCheckTimestamp;
		
		private string GetFileName(TileInfo info) => Path.Combine(_cacheFolderPath, $"{info}.png");

		public bool Provide(TileInfo info, Texture2D texture)
		{
			if (_fileCount >= _limit)
				CheckAndClean();

			if (_fileCount < _limit || info.Zoom <= _alwaysCachedMaxZoom)
			{
				SaveIfNotExists(info, texture);
				return true;
			}

			return false;
		}

		public void Retrieve(TileInfo info, Action<Texture2D> onTextureLoaded)
		{
//			return LoadImage(info);
			LoadImageWithCoroutine(info, onTextureLoaded);
		}

		private void SaveIfNotExists(TileInfo info, Texture2D texture)
		{
			if (File.Exists(GetFileName(info)))
				return;

			SaveImage(info, texture);
			_fileCount++;

			if (ItsTimeToCheckFolder)
				CheckAndClean();
		}

		private bool ItsTimeToCheckFolder => Time.realtimeSinceStartup - _lastFileSystemCheckTimestamp > FileSystemCheckInterval;

		private void SaveImage(TileInfo info, Texture2D texture)
		{
			try
			{
				if (!Directory.Exists(_cacheFolderPath))
					Directory.CreateDirectory(_cacheFolderPath);

				var path = GetFileName(info);
				if (File.Exists(path))
					File.Delete(path);
				File.WriteAllBytes(path, texture.EncodeToPNG());
			}
			catch (Exception e)
			{
				Debug.LogError($"Error saving cache: {e.Message}");
			}
		}

		private Texture2D LoadImage(TileInfo info)
		{
			try
			{
				if (!Directory.Exists(_cacheFolderPath))
					return null;

				var path = GetFileName(info);
				if (!File.Exists(path))return null;
				
				var tex = new Texture2D(info.Size, info.Size);
				tex.LoadImage(File.ReadAllBytes(path));
				tex.filterMode = FilterMode.Point;
				return tex;
			}
			catch (Exception e)
			{
				Debug.LogError($"Error loading cache: {e.Message}");
			}

			return null;
		}

		private IEnumerator LoadImageCoroutine(TileInfo info,Action<Texture2D> onTextureLoaded)
		{
			var www = UnityWebRequestTexture.GetTexture("file://"+GetFileName(info));
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

		public DefaultTileCache(string providerName, int limit = 1000, int alwaysCachedMaxZoom = 1)
		{
			_limit = limit;
			_alwaysCachedMaxZoom = alwaysCachedMaxZoom;

			_providerName = providerName ?? throw new ArgumentNullException(nameof(providerName));
			_cacheFolderPath = Path.Combine(Application.temporaryCachePath, CacheFolderName, _providerName);

			CheckAndClean();
		}

		private void CheckAndClean()
		{
			try
			{
				_lastFileSystemCheckTimestamp = Time.realtimeSinceStartup;
				
				if (!Directory.Exists(_cacheFolderPath))
				{
					_fileCount = 0;
					return;
				}

				_fileCount = Directory.GetFiles(_cacheFolderPath).Length;

				CleanCache();
			}
			catch (Exception e)
			{
				Debug.LogError($"Error updating cache: {e.Message}");
			}
		}

		private void CleanCache()
		{
			if (_fileCount < _limit || _limit <= 0) return;

			var fileCountToRemove = Mathf.Clamp(_limit / 10, 1, int.MaxValue);

			foreach (var file in Directory.GetFiles(_cacheFolderPath).OrderBy(x => new FileInfo(x).LastAccessTime)
				.Take(fileCountToRemove)) File.Delete(file);

			_fileCount = Mathf.Clamp(_fileCount - fileCountToRemove, 0, int.MaxValue);
		}
	}
}