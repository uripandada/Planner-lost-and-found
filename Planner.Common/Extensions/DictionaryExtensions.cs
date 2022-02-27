using System.Collections.Generic;

namespace Planner.Common.Extensions
{
	public static class DictionaryExtensions
	{
		public static void AddEmptyValueIfDoesntExist<TKey, TValue>(this Dictionary<TKey, TValue> map, TKey key) where TValue : new()
		{
			if (map.ContainsKey(key))
				return;

			map.Add(key, new TValue());
		}
	}
}
