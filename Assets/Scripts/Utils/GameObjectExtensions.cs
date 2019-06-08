using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
	public static class GameObjectExtensions
	{
		public static void WaitAndFire(this MonoBehaviour gameObject, Action action, float delay)
		{
			IEnumerator WaitAndFireCoroutine()
			{
				yield return new WaitForSeconds(delay);
				action.Invoke();
			}

			gameObject.StartCoroutine(WaitAndFireCoroutine());
		}
	}
}