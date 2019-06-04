using System.Collections;
using System.Collections.Generic;
using Tiles;
using UnityEngine;

public class DefaultTileMap : MonoBehaviour
{
	[SerializeField] private Camera observationCamera;
	
	private ITileObserver _tileObserver;
	private ITileRenderer _tileRenderer;
	private TileObserverViewport _viewport;

	private int DefaultTileSize = 256;

	// Start is called before the first frame update
	void Start()
	{
		_tileObserver = new BaseTileObserver(DefaultTileSize);
		
		_tileRenderer = gameObject.AddComponent<TileRenderer>();
		var provider = new OpenStreetMapCartocdnUrlProvider();
		_tileRenderer.Provider =
			new CachedOnlineTileMapProvider(provider, new DefaultTileCache(provider.Name, alwaysCachedMaxZoom: 4));
		
		_tileObserver.OnNewTilesAppear += TileObserverOnOnNewTilesAppear;
		_tileObserver.OnNewTilesDisappear += TileObserverOnOnNewTilesDisappear;

		_viewport = new TileObserverViewport(observationCamera.transform.position, CameraSize,
			observationCamera.pixelWidth, observationCamera.pixelHeight);
		_tileObserver.AddViewport(_viewport);
	}

	private Vector2 CameraSize =>  new Vector2(observationCamera.orthographicSize *2 * observationCamera.pixelWidth / observationCamera.pixelHeight,observationCamera.orthographicSize*2);

	private void TileObserverOnOnNewTilesAppear(IEnumerable<TileInfo> newTiles)
	{
		foreach (var tile in newTiles)
			_tileRenderer.RenderTile(tile);
	}

	private void TileObserverOnOnNewTilesDisappear(IEnumerable<TileInfo> newTiles)
	{
		foreach (var tile in newTiles)
			_tileRenderer.DestroyTile(tile);
	}

	// Update is called once per frame
	void Update()
	{
		_viewport.Size = CameraSize;
		_viewport.Position = observationCamera.transform.position;
	}
}