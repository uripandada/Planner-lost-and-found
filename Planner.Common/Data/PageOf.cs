using System.Collections.Generic;

namespace Planner.Common.Data
{
	public class PageOf<TData>
	{
		public IEnumerable<TData> Items { get; set; }

		public int TotalNumberOfItems { get; set; }
	}
}
