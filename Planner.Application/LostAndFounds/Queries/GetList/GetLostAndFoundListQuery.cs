using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.LostAndFounds.Models;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.LostAndFounds.Queries.GetList
{
	public class GetLostAndFoundListQuery : GetPageRequest, IRequest<ProcessResponse<PageOf<Models.LostAndFoundListItem>>>
	{
		public string Keyword { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public bool LoadLostItems { get; set; }
		public bool LoadFoundItems { get; set; }
		public string Filter { get; set; }
	}

	public class GetLostAndFoundListQueryHandler : IRequestHandler<GetLostAndFoundListQuery, ProcessResponse<PageOf<Models.LostAndFoundListItem>>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;

		public GetLostAndFoundListQueryHandler(IDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<ProcessResponse<PageOf<LostAndFoundListItem>>> Handle(GetLostAndFoundListQuery request, CancellationToken cancellationToken)
		{
			var query  = databaseContext.LostAndFounds.AsQueryable();

			if (!String.IsNullOrWhiteSpace(request.Keyword))
			{

				query = query.Where(x =>
				x.Name.ToLower().Contains(request.Keyword.ToLower())
				|| x.ReferenceNumber.ToLower().Contains(request.Keyword.ToLower())
				|| x.PhoneNumber.ToLower().Contains(request.Keyword.ToLower())
				|| x.Address.ToLower().Contains(request.Keyword.ToLower())
				|| x.Description.ToLower().Contains(request.Keyword.ToLower())
				|| x.Notes.ToLower().Contains(request.Keyword.ToLower())
				);
			}

			if (request.DateFrom.HasValue)
			{
				query = query.Where(x => x.LostOn >= request.DateFrom.Value || x.CreatedAt >= request.DateFrom.Value);
			}

			if (request.DateTo.HasValue)
			{
				query = query.Where(x => x.LostOn <= request.DateTo.Value || x.CreatedAt >= request.DateFrom.Value);
			}

			var typeValues = new List<Domain.Values.LostAndFoundRecordType>();
			if (request.LoadLostItems)
			{
				typeValues.Add(Domain.Values.LostAndFoundRecordType.Lost);
			}

			if (request.LoadFoundItems)
			{
				typeValues.Add(Domain.Values.LostAndFoundRecordType.Found);
			}

			if (typeValues.Any())
			{
				query = query.Where(x => typeValues.Contains(x.Type));
			}

			switch (request.Filter)
            {
				case "pending":
					query = query.Where(x => x.FoundStatus.Equals(Domain.Values.FoundStatus.WaitingRoomMaid));
					break;
				case "unclaimed":
					query = query.Where(x => x.GuestStatus.Equals(Domain.Values.GuestStatus.Unclaimed) && x.FoundStatus.Equals(Domain.Values.FoundStatus.Received));
					break;
				case "guest_follow_up":
					query = query.Where(x => x.FoundStatus.Equals(Domain.Values.FoundStatus.Received) && !x.GuestStatus.Equals(Domain.Values.GuestStatus.Unclaimed));
					break;
				case "returned":
					query = query.Where(x => x.DeliveryStatus.Equals(Domain.Values.DeliveryStatus.OTShipped) || x.DeliveryStatus.Equals(Domain.Values.DeliveryStatus.HandDelivered));
					break;
				case "canceled":
					query = query.Where(x => !x.OtherStatus.Equals(Domain.Values.OtherStatus.None));
					break;
				default:
					break;
					
            }


			var lostAndFound = await query.Select(x => new Models.LostAndFoundListItem
			{
				Id = x.Id,
				Description = x.Description,
				Address = x.Address,
				Name = x.Name,
				Notes = x.Notes,
				LostOn = x.LostOn,
				PhoneNumber = x.PhoneNumber,
				ReferenceNumber = x.ReferenceNumber,
				FoundStatus = x.FoundStatus,
				GuestStatus = x.GuestStatus,
				DeliveryStatus = x.DeliveryStatus,
				OtherStatus = x.OtherStatus,
				TypeOfLoss = x.TypeOfLoss.HasValue ? x.TypeOfLoss.Value : Domain.Values.TypeOfLoss.Unknown,
				StorageRoomId = x.StorageRoomId,
				FounderName = x.FounderName,
				LostAndFoundCategoryId = x.LostAndFoundCategoryId,
				ClientName = x.ClientName,
				LostAndFoundCategory = x.LostAndFoundCategory,
				RoomId = x.RoomId,
				Room = x.Room,
				Reservation = x.Reservation,
				ReservationId = x.ReservationId
			}).Skip(request.Skip).Take(request.Take).ToListAsync();

			var count = await query.CountAsync();
			return new ProcessResponse<PageOf<LostAndFoundListItem>>
			{
				Data = new PageOf<LostAndFoundListItem>
				{
					Items  = lostAndFound,
					TotalNumberOfItems = count
				},
				IsSuccess = true
			};
		}
	}
}
