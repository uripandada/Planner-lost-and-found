using Microsoft.EntityFrameworkCore;
using System;

namespace Planner.Domain.Entities
{
	[Keyless]
	public class NumberOfTasksPerUser
	{
		public int NumberOfTasks { get; set; }
		public Guid UserId { get; set; }
		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
	}
}
