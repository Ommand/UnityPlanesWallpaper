using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Plane : MonoBehaviour
{
	private const float MinSpeed = 0.5f;
	private const float MaxSpeed = 3f;
	
	private const float MinSize = 0.05f;
	private const float MaxSize = 0.2f;
	
	public int count = 5;
	public float range = 5;
	public float speed = 1;

	[SerializeField] private TrailRenderer _trailRenderer;
	[SerializeField] private SpriteRenderer _spriteRenderer;

	public event Action OnFlightFinished;

	// Start is called before the first frame update
	void Start()
	{
		RandomizeValues();
//		Fly(new[] {ForwardPoint}.Concat(GetRandomWaypoints()).ToArray());
	}

	private void RandomizeValues()
	{
		_spriteRenderer.color = RandomColor;
		_trailRenderer.startColor = RandomColor;
		_trailRenderer.endColor = RandomColor;

		speed = Random.Range(MinSpeed, MaxSpeed);
		_spriteRenderer.transform.localScale = Vector3.one * Random.Range(MinSize, MaxSize);
	}

	private static Color RandomColor => new Color(Random.Range(0f, 1), Random.Range(0f, 1), Random.Range(0f, 1));

	public void Fly(Vector3[] waypoints)
	{
		transform
			.DOLocalPath(waypoints, speed,
				pathMode: PathMode.TopDown2D, pathType: PathType.CatmullRom)
			.SetSpeedBased()
			.SetEase(Ease.Linear)
			.SetLookAt(0.01f)
			.OnComplete(() => OnFlightFinished?.Invoke());
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