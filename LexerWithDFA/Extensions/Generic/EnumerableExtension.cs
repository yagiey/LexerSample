using System;
using System.Collections.Generic;

namespace LexerWithDFA.Extensions.Generic
{
	internal static class EnumerableExtension
	{
		public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
		{
			foreach (var item in e)
			{
				action(item);
			}
		}
	}
}
