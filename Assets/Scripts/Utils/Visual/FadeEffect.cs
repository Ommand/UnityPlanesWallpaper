using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
	[SerializeField] private SpriteRenderer _renderer;

	public void FadeIn()
	{
		_renderer.DOColor(Color.black, 2);
	}

	public void FadeOut()
	{
		_renderer.DOColor(Color.clear, 2);
	}
}