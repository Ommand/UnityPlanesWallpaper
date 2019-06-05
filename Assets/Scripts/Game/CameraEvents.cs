using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraEvents : MonoBehaviour
{
	public event Action<Camera, float> OnOrthoSizeChanged;

	private Camera _camera;
	private float _lastOrthoSize;
	public Camera Camera => _camera ? _camera : _camera = GetComponent<Camera>(); 

	private void Update()
	{
		if (Camera.orthographicSize != _lastOrthoSize)
		{
			_lastOrthoSize = Camera.orthographicSize;

			OnOrthoSizeChanged?.Invoke(Camera, _lastOrthoSize);
		}
	}
}