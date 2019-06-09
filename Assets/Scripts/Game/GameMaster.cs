using System;
using Game;
using UnityEngine;

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

		public CameraEvents MainCamera => mainCamera;

		public void CreatePlane()
		{
			planes.CreatePlane(airports.VisibleAirports);
		}
	}
}