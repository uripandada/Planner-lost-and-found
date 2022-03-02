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

namespace Planner.Application.WarehouseManagement.Queries.GetPageOfWarehouseHistory
{
	public class WarehouseHistoryItem
	{
		public Guid Id { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public string CreatedAtString { get; set; }
		public string CreatedByName { get; set; }
		public string CreatedByUsername { get; set; }
		public Guid? CreatedById { get; set; }

		public Guid WarehouseId { get; set; }
		public string TypeKey { get; set; }
		public string Note { get; set; }
		public Guid AssetId { get; set; }
		public string AssetName { get; set; }
		public int AvailableQuantityChange { get; set; }
		public int AvailableQuantityBeforeChange { get; set; }
		public int ReservedQuantityChange { get; set; }
		public int ReservedQuantityBeforeChange { get; set; }

	}

	public class GetPageOfWarehouseHistoryQuery : IRequest<PageOf<WarehouseHistoryItem>>
	{
		public Guid WarehouseId { get; set; }
		public int Skip { get; set; }
		public int Take { get; set; }
		public string SortKey { get; set; }
		public string Keywords { get; set; }
	}
	public class GetPageOfWarehouseHistoryQueryHandler : IRequestHandler<GetPageOfWarehouseHistoryQuery, PageOf<WarehouseHistoryItem>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetPageOfWarehouseHistoryQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<WarehouseHistoryItem>> Handle(GetPageOfWarehouseHistoryQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext
				   .WarehouseDocuments
				   .Where(i => i.WarehouseId == request.WarehouseId)
				   .AsQueryable();

			var hotel = await this._databaseContext.Warehouses.Where(w => w.Id == request.WarehouseId).Select(w => new { HotelId = w.Hotel.Id, WindowsTimeZoneId = w.Hotel.WindowsTimeZoneId, IanaTimeZoneId = w.Hotel.IanaTimeZoneId }).FirstOrDefaultAsync();

			if (request.Keywords.IsNotNull())
			{
				var keywordsVaue = request.Keywords.ToLower();
				query = query.Where(a => a.Asset.Name.ToLower().Contains(keywordsVaue) || a.CreatedBy.FirstName.ToLower().Contains(keywordsVaue) || a.CreatedBy.LastName.ToLower().Contains(keywordsVaue) || a.CreatedBy.UserName.ToLower().Contains(keywordsVaue));
			}

			switch (request.SortKey)
			{
				case "CREATED_BY_ASC":
					query = query.OrderBy(a => a.CreatedBy.FirstName).ThenBy(a => a.CreatedBy.LastName);
					break;
				case "CREATED_BY_DESC":
					query = query.OrderByDescending(a => a.CreatedBy.FirstName).ThenByDescending(a => a.CreatedBy.LastName);
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

			var historyItems = await query.Select(i => new WarehouseHistoryItem
			{
				CreatedAt = i.CreatedAt,
				CreatedAtString = "",
				CreatedById = i.CreatedById,
				CreatedByName = i.CreatedBy == null ? "-" : i.CreatedBy.FirstName + " " + i.CreatedBy.LastName,
				CreatedByUsername = i.CreatedBy == null ? "-" : i.CreatedBy.UserName,
				AssetId = i.AssetId,
				AssetName = i.Asset == null ? "-" : i.Asset.Name,
				AvailableQuantityBeforeChange = i.AvailableQuantityBeforeChange,
				AvailableQuantityChange = i.AvailableQuantityChange,
				Id = i.Id,
				Note = i.Note,
				ReservedQuantityBeforeChange = i.ReservedQuantityBeforeChange,
				ReservedQuantityChange = i.ReservedQuantityChange,
				TypeKey = i.TypeKey,
				WarehouseId	= i.WarehouseId,

			}).ToListAsync();

			if (request.Take == 0)
			{
				totalNumber = historyItems.Count;
			}

			var timeZoneId = HotelLocalDateProvider.GetAvailableTimeZoneId(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var timeZoneInfo = TimeZoneConverter.TZConvert.GetTimeZoneInfo(timeZoneId);
			foreach (var historyItem in historyItems)
			{
				historyItem.CreatedAt = TimeZoneInfo.ConvertTime(historyItem.CreatedAt, timeZoneInfo);
				historyItem.CreatedAtString = historyItem.CreatedAt.ToString("yyyy-MM-dd HH:mm");
			}

			return new PageOf<WarehouseHistoryItem>
			{
				Items = historyItems,
				TotalNumberOfItems = totalNumber
			};
		}
	}
}
