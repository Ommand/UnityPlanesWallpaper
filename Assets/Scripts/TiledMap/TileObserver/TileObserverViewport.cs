using System;
using UnityEngine;

namespace Tiles
{
	public class TileObserverViewport
	{
		private Vector2 _position;
		private Vector2 _size;
		private float _pixelScale;

		public event Action OnViewportUpdated;

		public Vector2 Position
		{
			get => _position;
			set
			{
				_position = value;
				OnViewportUpdated?.Invoke();
			}
		}

		public Vector2 Size
		{
			get => _size;
			set
			{
				_size = value;
				OnViewportUpdated?.Invoke();
			}
		}

		public float PixelScale
		{
			get => _pixelScale;
			set
			{
				_pixelScale = value;
				OnViewportUpdated?.Invoke();
			}
		}

		public int PixelWidth { get; }
		public int PixelHeight { get; }

		public bool Repeat { get; }

		public TileObserverViewport(Vector2 position, Vector2 size, int pixelWidth, int pixelHeight,
			float pixelScale = 1.0f, bool repeat = false)
		{
			_position = position;
			_size = size;
			PixelWidth = pixelWidth;
			PixelHeight = pixelHeight;
			Repeat = repeat;
			_pixelScale = pixelScale;
		}
	}
}