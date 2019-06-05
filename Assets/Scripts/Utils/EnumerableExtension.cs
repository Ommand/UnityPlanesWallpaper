using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtension
{
	public static T PickRandom<T>(this IEnumerable<T> source)
	{
		return source.ElementAt(UnityEngine.Random.Range(0, source.Count()));
	}
}