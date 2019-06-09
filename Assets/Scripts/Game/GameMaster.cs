using System;
using System.Collections.Generic;
using System.Linq;
using Coordinates;
using Game;
using Game.Flight;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityPlanes
{
	public class GameMaster : MonoBehaviour
	{
		#region Singleton

		public static GameMaster Instance { get; private set; }

		private void Awake()
		{
			if (Instance)
				throw new Exception($"There are two instances of {nameof(GameMaster)} in scene");

			Instance = this;
		}

		#endregion
		
		[SerializeField] private CameraEvents mainCamera;
		[SerializeField] private Airports airports;
		[SerializeField] private Planes planes;

		private float _lastCameraSize = 0;

		public CameraEvents MainCamera => mainCamera;

		public void CreatePlane()
		{
			planes.CreatePlane(airports.VisibleAirports);
		}
	}
}