using System.Linq;

namespace Planner.Common.Extensions
{
	public static class IQueryableExtensions
	{
		public static IQueryable<TData> SkipTake<TData>(this IQueryable<TData> query, int? skip, int? take)
		{
			if (skip.HasValue && skip > 0)
			{
				query = query.Skip(skip.Value);
			}

			if (take.HasValue && take > 0)
			{
				query = query.Take(take.Value);
			}

			return query;
		}

		public static IQueryable<TData> SkipTake<TData>(IQueryable<TData> query, int skip, int take)
		{
			if (skip > 0)
			{
				query.Skip(skip);
			}

			if (take > 0)
			{
				query.Take(take);
			}

			return query;
		}
	}
}
