using System;
using System.Linq;
using Coordinates;
using DG.Tweening;
using Game.Flight;
using UnityEngine;
using UnityPlanes;
using Utils;
using Utils.Visual;
using Random = UnityEngine.Random;

public class Plane : MonoBehaviour
{
	private float _speed = 1;
	private Vector3 _targetSize;
	private float _baseEmission;
	private float _baseSpriteScale;

	[SerializeField] private SpriteRenderer _spriteRenderer;
	[SerializeField] private ParticleSystem _particleSystem;

	public event Action OnFlightFinished;

	private float LandingDuration => Settings.PlaneLandingAnimationDuration;

	private float BaseEmission => _baseEmission == 0
		? (_baseEmission = _particleSystem.emission.rateOverDistance.constant)
		: _baseEmission;

	private void Awake()
	{
		_baseSpriteScale = _spriteRenderer.transform.localScale.x;
	}

	private void RandomizeValues()
	{
		var (primaryColor, secondaryColor) = ColorScheme.GetRandomPair();
		
		_spriteRenderer.color = primaryColor;

		var particleSystemEmission = _particleSystem.emission;
		particleSystemEmission.rateOverDistance = BaseEmission / Settings.CameraSizeMultiplier;

		var particleSystemColor = _particleSystem.colorOverLifetime;
		var gradient = particleSystemColor.color.gradient;
		gradient.SetKeys(new[] {new GradientColorKey(secondaryColor, 0)}, gradient.alphaKeys);
		particleSystemColor.color = gradient;

		_speed = Random.Range(Settings.MinPlaneSpeed, Settings.MaxPlaneSpeed);
		_targetSize = Vector3.one * Random.Range(Settings.MinPlaneSize * _baseSpriteScale,
			              Settings.MaxPlaneSize * _baseSpriteScale);
		_spriteRenderer.transform.localScale = _targetSize;
	}

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