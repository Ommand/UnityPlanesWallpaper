using System;
using System.Linq;
using Coordinates;
using DG.Tweening;
using Game.Flight;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class Plane : MonoBehaviour
{
	private const float LandingDuration = 6;
	
	private const float MinSpeed = 0.5f;
	private const float MaxSpeed = 3f;
	
	private const float MinSize = 0.05f;
	private const float MaxSize = 0.2f;
	
	private float _speed = 1;
	private Vector3 _targetSize;

	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private ParticleSystem _particleSystem;

	public event Action OnFlightFinished;

	private void RandomizeValues()
	{
		var particleSystemMain = _particleSystem.colorOverLifetime;
		_spriteRenderer.color = RandomColor;

		var gradient = particleSystemMain.color.gradient;
		gradient.SetKeys(new[] {new GradientColorKey(RandomColor, 0)}, gradient.alphaKeys);
		particleSystemMain.color = gradient;

		_speed = Random.Range(MinSpeed, MaxSpeed);
		_targetSize = Vector3.one * Random.Range(MinSize, MaxSize);
		_spriteRenderer.transform.localScale = _targetSize;
	}

	private static Color RandomColor => new Color(Random.Range(0f, 1), Random.Range(0f, 1), Random.Range(0f, 1));

	public void Fly(FlightInfo flightInfo)
	{
		RandomizeValues();
		
		var waypoints = flightInfo.Points.Select(lat => (Vector3) WorldCoordinateHelper.LonLatToWorld(lat));

		transform.position = waypoints.First();

		var flyTween = transform
			.DOLocalPath(waypoints.Skip(1).ToArray(), _speed,
				pathMode: PathMode.TopDown2D, pathType: PathType.CatmullRom)
			.SetSpeedBased()
			.SetEase(Ease.Linear)
			.SetLookAt(0.01f)
			.OnComplete(() =>
				this.WaitAndFire(() => OnFlightFinished?.Invoke(), _particleSystem.main.startLifetime.constant));

		flyTween.OnStart(() =>
		{
			if (flightInfo.EndsOnGround)
			{
				var duration = flyTween.Duration();
			
				_spriteRenderer.transform
					.DOScale(_targetSize/20, LandingDuration)
					.SetEase(Ease.InQuart)
					.SetDelay(duration - LandingDuration);
			}
		});

		if (flightInfo.StartsOnGround)
		{
			_spriteRenderer.transform.localScale = Vector3.zero;
			_spriteRenderer.transform
				.DOScale(_targetSize, LandingDuration)
				.SetEase(Ease.OutCubic)
				;
		}
	}

	private Vector3 ForwardPoint
	{
		get
		{
			Debug.Log($"Pos: {transform.position}, Right: {transform.right}");
			Debug.DrawLine(transform.position, transform.position + transform.right);
			return transform.position + transform.right * 1.5f;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(transform.position, transform.right);
	}
}