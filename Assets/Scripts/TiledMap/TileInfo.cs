using System;
using UnityEngine;

namespace Tiles
{
	public struct TileInfo : IEquatable<TileInfo>
	{
		public TileInfo(int row, int col, int size, int zoom)
		{
			Row = row;
			Col = col;
			Size = size;
			Zoom = zoom;
			Width = GetTileWidth(Size, Zoom);
		}

		public int Row { get; }
		public int Col { get; }
		public int Size { get; }
		public int Zoom { get; }
		public float Width { get; }

		public Vector2 BottomLeft => new Vector2(Width * Col, Size - Width * (Row + 1));
		public Vector2 Center => BottomLeft + Vector2.one * Width;

		public float Scale => Width;

		public static (int, int) PosToMesh(Vector2 pos, int pixelSize, int zoom)
		{
			var width = GetTileWidth(pixelSize, zoom);
			var i = Mathf.FloorToInt(pos.x / width);
			var j = (1 << zoom) - Mathf.FloorToInt(pos.y / width) - 1;

			return (i, j);
		}

		public static float GetTileWidth(int pixelSize, int zoom)
		{
			return pixelSize / (float) (1 << zoom);
		}

		#region IEquatable

		public bool Equals(TileInfo other)
		{
			return Row == other.Row && Col == other.Col && Size == other.Size && Zoom == other.Zoom;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is TileInfo other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Row;
				hashCode = (hashCode * 397) ^ Col;
				hashCode = (hashCode * 397) ^ Size;
				hashCode = (hashCode * 397) ^ Zoom;
				return hashCode;
			}
		}

		#endregion

		#region ToString

		public override string ToString()
		{
			return $"TileInfo {Zoom}-{Col}-{Row}";
		}

		#endregion
	}
}