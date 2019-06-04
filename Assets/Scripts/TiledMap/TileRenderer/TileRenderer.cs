using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tiles
{
	public class TileRenderer : MonoBehaviour, ITileRenderer
	{
		private Dictionary<TileInfo, SpriteRenderer> _spriteRenderers = new Dictionary<TileInfo, SpriteRenderer>();
		public ITileProvider Provider { get; set; }

		public event Action OnViewUpdate;

		public void RenderTile(TileInfo info)
		{
			if (Provider == null)
				throw new NullReferenceException(nameof(Provider));

			if (_spriteRenderers.ContainsKey(info))
				return;

//			var texture = Provider.RequestTileTexture(info);
			var spriteRenderer =
				new GameObject($"Sprite {info.Zoom}-{info.Row}-{info.Col}").AddComponent<SpriteRenderer>();
			spriteRenderer.transform.SetParent(transform, false);
			spriteRenderer.transform.localPosition = (Vector3) info.BottomLeft + Vector3.back * info.Zoom * 0.01f;
			spriteRenderer.transform.localScale = Vector2.one * info.Scale;
			spriteRenderer.gameObject.layer = gameObject.layer;

			_spriteRenderers[info] = spriteRenderer;

			/*
			 * Perform texture request
			 */
			void OnTextureReady(Texture2D texture) => ApplyTexture(info, spriteRenderer, texture);
			Provider.RequestTileTexture(info, OnTextureReady, OnTextureReady);
		}

		public void ApplyTexture(TileInfo info, SpriteRenderer renderer, Texture2D texture)
		{
			if (!renderer)
			{
				Provider.FreeTile(info);
				return;
			}

			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Bilinear;
			texture.Apply(true);

			renderer.sprite = Sprite.Create(texture,
				new Rect(0, 0, texture.width, texture.width), Vector2.zero, texture.width);

			OnViewUpdate?.Invoke();
		}

		public void DestroyTile(TileInfo info)
		{
			Destroy(_spriteRenderers[info].gameObject);
			Provider.FreeTile(info);
			_spriteRenderers.Remove(info);
		}
	}
}