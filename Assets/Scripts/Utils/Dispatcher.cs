using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Dispatcher : Singleton<Dispatcher>
{
	private static Queue<Action> actions = new Queue<Action>();

	/// <summary>
	/// Call this method to use Add method
	/// </summary>
	[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
	public void Ensure()
	{
	}

	public static void Add(Action action)
	{
		if (!IsInstanceExist)
			return;

		lock (actions)
		{
			actions.Enqueue(action);
		}
	}

	public static void Add(IEnumerator routine)
	{
		if (!IsInstanceExist)
			return;

		lock (actions)
		{
			actions.Enqueue(() => { Instance.StartCoroutine(routine); });
		}
	}

	static bool IsInstanceExist
	{
		get
		{
			if (IsDestroyed)
				return false;
			var result = HasInstance;
			if (!HasInstance)
				Debug.LogError(
					"Instance of Dispatcher hasn't been created. Call Dispatcher.Instance.Ensure() in main thread to use Add method");
			return result;
		}
	}

	void Update()
	{
		if (null == actions)
			return;

		Action[] currentActions = null;
		lock (actions)
		{
			if (actions.Count > 0)
			{
				currentActions = actions.ToArray();
				actions.Clear();
			}
		}

		if (currentActions != null)
			foreach (var action in currentActions)
				action();
	}
}

