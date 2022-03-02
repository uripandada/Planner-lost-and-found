using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.WarehouseManagement.Queries.GetPageOfWarehouseInventoryArchives
{
	public class WarehouseInventoryArchiveItem
	{
		public Guid Id { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public string CreatedAtString { get; set; }
		public string CreatedByName { get; set; }
		public string CreatedByUsername { get; set; }
		public Guid? CreatedById { get; set; }
	}
	public class GetPageOfWarehouseInventoryArchivesQuery : IRequest<PageOf<WarehouseInventoryArchiveItem>>
	{
		public Guid WarehouseId { get; set; }
		public int Skip { get; set; }
		public int Take { get; set; }
		public string SortKey { get; set; }
		public string Keywords { get; set; }
	}
	public class GetPageOfWarehouseInventoryArchivesQueryHandler : IRequestHandler<GetPageOfWarehouseInventoryArchivesQuery, PageOf<WarehouseInventoryArchiveItem>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfWarehouseInventoryArchivesQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<WarehouseInventoryArchiveItem>> Handle(GetPageOfWarehouseInventoryArchivesQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext
				   .Inventories
				   .Where(i => i.WarehouseId == request.WarehouseId)
				   .AsQueryable();

			var hotel = await this._databaseContext.Warehouses.Where(w => w.Id == request.WarehouseId).Select(w => new { HotelId = w.Hotel.Id, WindowsTimeZoneId = w.Hotel.WindowsTimeZoneId, IanaTimeZoneId = w.Hotel.IanaTimeZoneId }).FirstOrDefaultAsync();

			if (request.Keywords.IsNotNull())
			{
				var keywordsVaue = request.Keywords.ToLower();
				query = query.Where(a => a.CreatedBy.FirstName.ToLower().Contains(keywordsVaue) || a.CreatedBy.LastName.ToLower().Contains(keywordsVaue) || a.CreatedBy.UserName.ToLower().Contains(keywordsVaue));
			}

			switch (request.SortKey)
			{
				case "NAME_ASC":
					query = query.OrderBy(a => a.CreatedBy.FirstName).ThenBy(a => a.CreatedBy.LastName);
					break;
				case "NAME_DESC":
					query = query.OrderByDescending(a => a.CreatedBy.FirstName).ThenByDescending(a => a.CreatedBy.LastName);
					break;
				case "USERNAME_ASC":
					query = query.OrderBy(a => a.CreatedBy.UserName);
					break;
				case "USERNAME_DESC":
					query = query.OrderByDescending(a => a.CreatedBy.UserName);
					break;
				case "CREATED_AT_ASC":
					query = query.OrderBy(a => a.CreatedAt);
					break;
				case "CREATED_AT_DESC":
					query = query.OrderByDescending(a => a.CreatedAt);
					break;
				default:
					query = query.OrderByDescending(a => a.CreatedAt);
					break;
			}

			var totalNumber = 0;

			if (request.Take > 0)
			{
				totalNumber = await query.CountAsync();
			}

			if (request.Skip > 0)
			{
				query = query.Skip(request.Skip);
			}

			if (request.Take > 0)
			{
				query = query.Take(request.Take);
			}

			var inventories = await query.Select(i => new WarehouseInventoryArchiveItem 
			{
				CreatedAt = i.CreatedAt,
				CreatedAtString = "",
				CreatedById = i.CreatedById,
				CreatedByName = i.CreatedBy == null ? "-" : i.CreatedBy.FirstName + " " + i.CreatedBy.LastName,
				CreatedByUsername = i.CreatedBy == null ? "-" : i.CreatedBy.UserName,
			}).ToListAsync();

			if (request.Take == 0)
			{
				totalNumber = inventories.Count;
			}

			var timeZoneId = HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var timeZoneInfo = TimeZoneConverter.TZConvert.GetTimeZoneInfo(timeZoneId);
			foreach (var inventory in inventories)
			{
				inventory.CreatedAt = TimeZoneInfo.ConvertTime(inventory.CreatedAt, timeZoneInfo);
				inventory.CreatedAtString = inventory.CreatedAt.ToString("yyyy-MM-dd HH:mm");
			}

			return new PageOf<WarehouseInventoryArchiveItem>
			{
				Items = inventories,
				TotalNumberOfItems = totalNumber
			};
		}
	}
}
