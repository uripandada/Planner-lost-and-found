﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExperienceManagement.Queries.GetList
{
	public class ExperienceGridItemViewModel
	{
		public Guid Id { get; set; }
		public string RoomName { get; set; }
		public string GuestName { get; set; }
		public DateTime? CheckIn { get; set; }
		public DateTime? CheckOut { get; set; }
		public string ReservationId { get; set; }
		public string VIP { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public int Type { get; set; }
		public string Description { get; set; }
		public string Actions { get; set; }
		public string InternalFollowUp { get; set; }
		public Guid ExperienceCategoryId { get; set; }
		public ExperienceCategory ExperienceCategory { get; set; }
		public Guid ExperienceCompensationId { get; set; }
		public ExperienceCompensation ExperienceCompensation { get; set; }
		public string Group { get; set; }
		public ExperienceTicketStatus ExperienceTicketStatus { get; set; }
		public ExperienceClientRelationStatus ExperienceClientRelationStatus { get; set; }
		public ExperienceResolutionStatus ExperienceResolutionStatus { get; set; }
	}

	public class GetExperienceListQuery : GetPageRequest, IRequest<PageOf<ExperienceGridItemViewModel>>
	{
		public string Keywords { get; set; }
		public DateTime? DateFrom { get; set; }
		public DateTime? DateTo { get; set; }
		public string SortKey { get; set; }
	}

	public class GetExperienceListQueryHandler : GetPageRequest, IRequestHandler<GetExperienceListQuery, PageOf<ExperienceGridItemViewModel>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetExperienceListQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<PageOf<ExperienceGridItemViewModel>> Handle(GetExperienceListQuery request, CancellationToken cancellationToken)
		{
			var query = this._databaseContext.Experiences.AsQueryable();

			if (request.Keywords.IsNotNull())
			{
				var keywordsValue = request.Keywords.ToLower();
				query = query.Where(c => c.GuestName.ToLower().Contains(keywordsValue));
			}

			var count = 0;
			if (request.Skip > 0 || request.Take > 0)
			{
				count = await query.CountAsync();
			}

			if (request.SortKey.IsNotNull())
			{
				switch (request.SortKey)
				{
					case "CREATED_AT_DESC":
						query = query.OrderBy(q => q.CheckIn);
						break;
					case "CREATED_AT_ASC":
						query = query.OrderByDescending(q => q.CheckIn);
						break;
					default:
						break;
				}
			}

			if (request.DateFrom.HasValue)
			{
				query = query.Where(x => x.CreatedAt >= request.DateFrom.Value);
			}

			if (request.DateTo.HasValue)
			{
				query = query.Where(x => x.CreatedAt <= request.DateTo.Value);
			}

			if (request.Skip > 0)
			{
				query = query.Skip(request.Skip);
			}

			if (request.Take > 0)
			{
				query = query.Take(request.Take);
			}

			var experiences = await query.ToArrayAsync();

			if (request.Skip == 0 && request.Take == 0)
			{
				count = experiences.Length;
			}

			var response = new PageOf<ExperienceGridItemViewModel>
			{
				TotalNumberOfItems = count,
				Items = experiences.Select(d => new ExperienceGridItemViewModel
				{
					Id = d.Id,
					RoomName = d.RoomName,
					GuestName = d.GuestName,
					CheckIn = d.CheckIn,
					CheckOut = d.CheckOut,
					ReservationId = d.ReservationId,
					VIP = d.VIP,
					Email = d.Email,
					PhoneNumber = d.PhoneNumber,
					Type = d.Type,
					Description = d.Description,
					Actions = d.Actions,
					Group = d.Group,
					InternalFollowUp = d.InternalFollowUp,
					ExperienceCategoryId = d.ExperienceCategoryId,
					ExperienceCategory = this._databaseContext.ExperienceCategories.Where(x => x.Id == d.ExperienceCategoryId).Single(),
					ExperienceCompensationId = d.ExperienceCompensationId,
					ExperienceCompensation = this._databaseContext.ExperienceCompensations.Where(x => x.Id == d.ExperienceCompensationId).Single(),
					ExperienceTicketStatus = d.ExperienceTicketStatus,
					ExperienceClientRelationStatus = d.ExperienceClientRelationStatus,
					ExperienceResolutionStatus = d.ExperienceResolutionStatus
				}).ToArray()
			};

			return response;
		}
	}
}
