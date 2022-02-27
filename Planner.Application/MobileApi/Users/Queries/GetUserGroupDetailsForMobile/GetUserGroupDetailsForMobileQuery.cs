using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Application.MobileApi.Users.Queries.GetListOfUserGroupsForMobile;
using Planner.Common;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Users.Queries.GetUserGroupDetailsForMobile
{
	public class MobileUserGroupDetails: MobileUserGroup
	{

	}

	public class GetUserGroupDetailsForMobileQuery: IRequest<MobileUserGroupDetails>
	{
		public string HotelId { get; set; }
		public Guid Id { get; set; }
		public bool IsSubGroup { get; set; }
	}

	public class GetUserGroupDetailsForMobileQueryHandler : IRequestHandler<GetUserGroupDetailsForMobileQuery, MobileUserGroupDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetUserGroupDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileUserGroupDetails> Handle(GetUserGroupDetailsForMobileQuery request, CancellationToken cancellationToken)
		{

			if (request.IsSubGroup)
			{
				var userSubGroup = await this._databaseContext.UserSubGroups
					   .Include(ug => ug.Users)
					   .Where(ug => ug.Id == request.Id)
					   .FirstOrDefaultAsync();

				return new MobileUserGroupDetails
				{
					Id = userSubGroup.Id,
					HotelId = request.HotelId,
					Name = userSubGroup.Name,
					UserIds = userSubGroup.Users == null ? new Guid[0] : userSubGroup.Users.Select(u => u.Id).ToArray(),
					Emails = new string[0],

					EscalationCompletedTime = 0,
					EscalationAcceptedTime = 0,

					IsAssetsViewable = false,
					IsCatalogViewable = false,
					IsConciergeViewable = false,
					IsDashboardAttendantViewable = false,
					IsDashboardAuditViewable = false,
					IsDashboardExperiencesViewable = false,
					IsDashboardInventoryViewable = false,
					IsDashboardTasksViewable = false,
					IsDashboardViewable = false,
					IsGroupsViewable = false,
					IsHistoryViewable = false,
					IsHousekeepingViewable = false,
					IsInventoryViewable = false,
					IsLostFoundViewable = false,
					IsMaintenanceViewable = false,
					IsPlanningViewable = false,
					IsQuestionnairesViewable = false,
					IsRoomsViewable = false,
					IsSettingsViewable = false,
					IsUsersViewable = false,

					IsEnableEscalationAccepted = false,
					IsEnableEscalationCompleted = false,
					IsEnableExperienceEndEmail = false,
					IsEnableExperienceStartEmail = false,
					IsEnableExperienceUpdateEmail = false,
					IsEscalationAcceptedGuests = false,
					IsEscalationCompletedGuests = false,

					IsSubGroup = true,
				};
			}
			else
			{
				var userGroup = await this._databaseContext.UserGroups
					   .Include(ug => ug.Users)
					   .Where(ug => ug.Id == request.Id)
					   .FirstOrDefaultAsync();

				return new MobileUserGroupDetails
				{
					Id = userGroup.Id,
					HotelId = request.HotelId,
					Name = userGroup.Name,
					UserIds = userGroup.Users == null ? new Guid[0] : userGroup.Users.Select(u => u.Id).ToArray(),
					Emails = new string[0],

					EscalationCompletedTime = 0,
					EscalationAcceptedTime = 0,

					IsAssetsViewable = false,
					IsCatalogViewable = false,
					IsConciergeViewable = false,
					IsDashboardAttendantViewable = false,
					IsDashboardAuditViewable = false,
					IsDashboardExperiencesViewable = false,
					IsDashboardInventoryViewable = false,
					IsDashboardTasksViewable = false,
					IsDashboardViewable = false,
					IsGroupsViewable = false,
					IsHistoryViewable = false,
					IsHousekeepingViewable = false,
					IsInventoryViewable = false,
					IsLostFoundViewable = false,
					IsMaintenanceViewable = false,
					IsPlanningViewable = false,
					IsQuestionnairesViewable = false,
					IsRoomsViewable = false,
					IsSettingsViewable = false,
					IsUsersViewable = false,

					IsEnableEscalationAccepted = false,
					IsEnableEscalationCompleted = false,
					IsEnableExperienceEndEmail = false,
					IsEnableExperienceStartEmail = false,
					IsEnableExperienceUpdateEmail = false,
					IsEscalationAcceptedGuests = false,
					IsEscalationCompletedGuests = false,

					IsSubGroup = false,
				};
			}
		}
	}
}
