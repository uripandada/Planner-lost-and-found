using System;
using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Domain.Entities;
using Planner.Application.AutomaticHousekeepingUpdateSettingss.Models;
using Planner.Application.Admin;

namespace Planner.Application.AutomaticHousekeepingUpdateSettingss.Queries.GetPageOfAutomaticHousekeepingUpdateSettings
{
	public class SortData
	{
		public string Key { get; set; }
		public string Value { get; set; }
	}
	public class SortByData
	{
		public string ColumnName { get; set; }
		public string SortDirection { get; set; }
	}
	public class PageOf<TData>
	{
		public IEnumerable<TData> Items { get; set; }
		public int Total { get; set; }
	}
	public class FilterByData
	{
		public string ColumnName { get; set; }
		public string Keywords { get; set; }
	}
	public class GetPageOfAutomaticHousekeepingUpdateSettingsQuery: IRequest<PageOf<AutomaticHousekeepingUpdateSettingsListItem>>
	{
		public int? Skip { get; set; }
		public int? Take { get; set; }
		public IEnumerable<SortByData> Sorts { get; set; }
		public IEnumerable<FilterByData> Filters { get; set; }
		public string Keywords { get; set; }
	}

	public class GetPageOfAutomaticHousekeepingUpdateSettingsQueryHandler: IRequestHandler<GetPageOfAutomaticHousekeepingUpdateSettingsQuery, PageOf<AutomaticHousekeepingUpdateSettingsListItem>>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GetPageOfAutomaticHousekeepingUpdateSettingsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<PageOf<AutomaticHousekeepingUpdateSettingsListItem>> Handle(GetPageOfAutomaticHousekeepingUpdateSettingsQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.AutomaticHousekeepingUpdateSettingss.AsQueryable();

			if (!string.IsNullOrWhiteSpace(request.Keywords))
			{
				// 
				// var keywordsValue = request.Keywords.ToLower().Trim();
				// query = query.Where(cp => cp.Name.ToLower().Contains(keywordsValue));
				// Uncomment the line below to filter by all string properties
				// query = query.Where(st => (RoomNameRegex||UpdateStatusTo||UpdateStatusWhen||UpdateStatusAtTime));
			}

			if (request.Filters != null && request.Filters.Any())
			{
				foreach(var f in request.Filters)
				{
					var keywordsValue = f.Keywords.ToLower();
					switch (f.ColumnName)
					{
						case nameof(AutomaticHousekeepingUpdateSettings.Id):
							if(Guid.TryParse(f.Keywords, out var temp_Id))
							{
								query = query.Where(cp => cp.Id == temp_Id);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Dirty):
							if(bool.TryParse(f.Keywords, out var temp_Dirty))
							{
								query = query.Where(cp => cp.Dirty == temp_Dirty);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Clean):
							if(bool.TryParse(f.Keywords, out var temp_Clean))
							{
								query = query.Where(cp => cp.Clean == temp_Clean);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.CleanNeedsInspection):
							if(bool.TryParse(f.Keywords, out var temp_CleanNeedsInspection))
							{
								query = query.Where(cp => cp.CleanNeedsInspection == temp_CleanNeedsInspection);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Inspected):
							if(bool.TryParse(f.Keywords, out var temp_Inspected))
							{
								query = query.Where(cp => cp.Inspected == temp_Inspected);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Vacant):
							if(bool.TryParse(f.Keywords, out var temp_Vacant))
							{
								query = query.Where(cp => cp.Vacant == temp_Vacant);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Occupied):
							if(bool.TryParse(f.Keywords, out var temp_Occupied))
							{
								query = query.Where(cp => cp.Occupied == temp_Occupied);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.DoNotDisturb):
							if(bool.TryParse(f.Keywords, out var temp_DoNotDisturb))
							{
								query = query.Where(cp => cp.DoNotDisturb == temp_DoNotDisturb);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.DoDisturb):
							if(bool.TryParse(f.Keywords, out var temp_DoDisturb))
							{
								query = query.Where(cp => cp.DoDisturb == temp_DoDisturb);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.OutOfService):
							if(bool.TryParse(f.Keywords, out var temp_OutOfService))
							{
								query = query.Where(cp => cp.OutOfService == temp_OutOfService);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.InService):
							if(bool.TryParse(f.Keywords, out var temp_InService))
							{
								query = query.Where(cp => cp.InService == temp_InService);
							}
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.RoomNameRegex):
							query = query.Where(cp => cp.RoomNameRegex.Contains(keywordsValue));
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.UpdateStatusAtTime):
							query = query.Where(cp => cp.UpdateStatusAtTime.Contains(keywordsValue));
							break;
					}
				}
			}

			var count = await query.CountAsync();

			if (request.Sorts != null && request.Sorts.Any())
			{
				foreach(var sort in request.Sorts)
				{
					switch (sort.ColumnName)
					{
						case nameof(AutomaticHousekeepingUpdateSettings.Id):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.Id);
							else query = query.OrderBy(q => q.Id);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Dirty):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.Dirty);
							else query = query.OrderBy(q => q.Dirty);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Clean):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.Clean);
							else query = query.OrderBy(q => q.Clean);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.CleanNeedsInspection):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.CleanNeedsInspection);
							else query = query.OrderBy(q => q.CleanNeedsInspection);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Inspected):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.Inspected);
							else query = query.OrderBy(q => q.Inspected);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Vacant):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.Vacant);
							else query = query.OrderBy(q => q.Vacant);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.Occupied):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.Occupied);
							else query = query.OrderBy(q => q.Occupied);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.DoNotDisturb):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.DoNotDisturb);
							else query = query.OrderBy(q => q.DoNotDisturb);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.DoDisturb):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.DoDisturb);
							else query = query.OrderBy(q => q.DoDisturb);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.OutOfService):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.OutOfService);
							else query = query.OrderBy(q => q.OutOfService);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.InService):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.InService);
							else query = query.OrderBy(q => q.InService);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.RoomNameRegex):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.RoomNameRegex);
							else query = query.OrderBy(q => q.RoomNameRegex);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.UpdateStatusTo):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.UpdateStatusTo);
							else query = query.OrderBy(q => q.UpdateStatusTo);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.UpdateStatusWhen):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.UpdateStatusWhen);
							else query = query.OrderBy(q => q.UpdateStatusWhen);
							break;
						case nameof(AutomaticHousekeepingUpdateSettings.UpdateStatusAtTime):
							if(sort.SortDirection == "descending") query = query.OrderByDescending(q => q.UpdateStatusAtTime);
							else query = query.OrderBy(q => q.UpdateStatusAtTime);
							break;
						default:
							query = query.OrderBy(q => q.Id);
							break;
					}
				}
			}
			else
			{
				query = query.OrderBy(q => q.Id);
			}

			if (request.Skip.HasValue && request.Skip.Value > 0)
			{
				query = query.Skip(request.Skip.Value);
			}

			if (request.Take.HasValue && request.Take.Value > 0)
			{
				query = query.Take(request.Take.Value);
			}

			var result = await query.ToArrayAsync();

			var list = new List<AutomaticHousekeepingUpdateSettingsListItem>();
			foreach(var x in result)
			{
				var item = new AutomaticHousekeepingUpdateSettingsListItem
				{
					Id = x.Id,
					Dirty = x.Dirty,
					Clean = x.Clean,
					CleanNeedsInspection = x.CleanNeedsInspection,
					Inspected = x.Inspected,
					Vacant = x.Vacant,
					Occupied = x.Occupied,
					DoNotDisturb = x.DoNotDisturb,
					DoDisturb = x.DoDisturb,
					OutOfService = x.OutOfService,
					InService = x.InService,
					RoomNameRegex = x.RoomNameRegex,
					UpdateStatusTo = x.UpdateStatusTo,
					UpdateStatusWhen = x.UpdateStatusWhen,
					UpdateStatusAtTime = x.UpdateStatusAtTime,
				};

				list.Add(item);
			}

			return new PageOf<AutomaticHousekeepingUpdateSettingsListItem>
			{
				Total = count,
				Items = list
			};
		}
	}
}
