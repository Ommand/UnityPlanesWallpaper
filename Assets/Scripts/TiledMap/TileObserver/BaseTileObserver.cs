using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tiles
{
	public class BaseTileObserver : ITileObserver
	{
		private const float ZoomThreshold = 0.6f;

		private Dictionary<TileObserverViewport,int> _currentZoom = new Dictionary<TileObserverViewport, int>();
		private readonly HashSet<TileInfo> _visibleTiles = new HashSet<TileInfo>();
		private readonly List<TileObserverViewport> _viewports = new List<TileObserverViewport>();

		public event Action<IEnumerable<TileInfo>> OnNewTilesAppear;
		public event Action<IEnumerable<TileInfo>> OnNewTilesDisappear;

		public int TileSize { get; }
		public int BaseCols => 1;
		public int BaseRows => 1;

		public int GetCols(int zoom) => BaseCols << zoom;
		public int GetRows(int zoom) => BaseRows << zoom;

		public int PixelWidth => BaseCols * TileSize;
		public int PixelHeight => BaseRows * TileSize;

		public BaseTileObserver(int tileSize)
		{
			TileSize = tileSize;
		}

		public void AddViewport(TileObserverViewport viewport)
		{
			_viewports.Add(viewport);
			_currentZoom.Add(viewport, 0);
			viewport.OnViewportUpdated += ViewportOnOnViewportUpdated;
		}

		private void ViewportOnOnViewportUpdated()
		{
			UpdateVisibleTiles();
		}

		public void CheckZoom(TileObserverViewport viewport)
		{
			if (viewport.Size.x == 0 || viewport.Size.y == 0)
				return;

			var newZoom = GetZoom(viewport);

			if (newZoom != _currentZoom[viewport])
			{
				RemoveTiles(_visibleTiles.Where(tile => tile.Zoom > newZoom));
				_currentZoom[viewport] = newZoom;
			}
		}

		private void AddTiles(IEnumerable<TileInfo> tiles)
		{
			var tileInfos = tiles as TileInfo[] ?? tiles.Except(_visibleTiles).ToArray();

			if (!tileInfos.Any())
				return;

			OnNewTilesAppear?.Invoke(tileInfos);

			foreach (var tile in tileInfos)
				_visibleTiles.Add(tile);
		}

		private void RemoveTiles(IEnumerable<TileInfo> tiles)
		{
			var tileInfos = tiles as TileInfo[] ?? tiles.ToArray();

			if (!tileInfos.Any())
				return;

			OnNewTilesDisappear?.Invoke(tileInfos);

			foreach (var tile in tileInfos)
				_visibleTiles.Remove(tile);
		}

		public int GetZoom(TileObserverViewport viewport)
		{
			var zoom = _currentZoom[viewport];
			var viewportSize = Mathf.Max(viewport.Size.x, viewport.Size.y);
			var viewportPixelSize = Mathf.Max(viewport.PixelWidth, viewport.PixelHeight) / viewport.PixelScale;
			
			while (true)
			{
				var tilesPerViewport = viewportPixelSize / TileSize;
				var zoomValue = (viewportSize / tilesPerViewport) / TileInfo.GetTileWidth(TileSize, zoom);

				if (zoomValue < ZoomThreshold)
					zoom++;
				else if (zoomValue > 2 * ZoomThreshold && zoom > 0)
					zoom--;
				else
					break;
			}

			return zoom;
		}

		public void CheckViewport(TileObserverViewport viewport)
		{
			if (viewport.Size.x == 0 || viewport.Size.y == 0)
				return;
			
			var zoom = _currentZoom[viewport];
			var (xMin, yMin) = TileInfo.PosToMesh(viewport.Position - viewport.Size / 2, TileSize, zoom);
			var (xMax, yMax) = TileInfo.PosToMesh(viewport.Position + viewport.Size / 2, TileSize, zoom);

			if (xMin > xMax)
				(xMin, xMax) = (xMax, xMin);
			if (yMin > yMax)
				(yMin, yMax) = (yMax, yMin);

			var cols = GetCols(zoom);
			var rows = GetRows(zoom);
			if (!viewport.Repeat)
			{
				xMin = Mathf.Clamp(xMin, 0, cols - 1);
				xMax = Mathf.Clamp(xMax, 0, cols - 1);
				yMin = Mathf.Clamp(yMin, 0, rows - 1);
				yMax = Mathf.Clamp(yMax, 0, rows - 1);
			}

			var newTiles = new List<TileInfo>();

			for (var row = yMin; row <= yMax; row++)
			for (var col = xMin; col <= xMax; col++)
				newTiles.Add(new TileInfo((row + rows) % rows, (col + cols) % cols, TileSize, zoom));

			RemoveTiles(_visibleTiles.Except(newTiles).Where(tile => tile.Zoom == zoom));

			AddTiles(newTiles);
		}

		private void UpdateVisibleTiles()
		{
			foreach (var viewport in _viewports)
			{
				CheckZoom(viewport);
				CheckViewport(viewport);
			}
		}
	}
}