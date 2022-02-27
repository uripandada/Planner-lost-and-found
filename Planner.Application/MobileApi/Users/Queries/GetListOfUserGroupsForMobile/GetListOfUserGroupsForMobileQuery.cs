using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Users.Queries.GetListOfUserGroupsForMobile
{
	public class MobileUserGroup
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string HotelId { get; set; }
		public IEnumerable<Guid> UserIds { get; set; }
		public bool IsMaintenanceViewable { get; set; }
		public bool IsHousekeepingViewable { get; set; }
		public bool IsConciergeViewable { get; set; }
		public bool IsPlanningViewable { get; set; }
		public bool IsDashboardViewable { get; set; }
		public bool IsQuestionnairesViewable { get; set; }
		public bool IsInventoryViewable { get; set; }
		public bool IsLostFoundViewable { get; set; }
		public bool IsHistoryViewable { get; set; }
		public bool IsAssetsViewable { get; set; }
		public bool IsCatalogViewable { get; set; }
		public bool IsRoomsViewable { get; set; }
		public bool IsUsersViewable { get; set; }
		public bool IsGroupsViewable { get; set; }
		public bool IsSettingsViewable { get; set; }
		public bool IsDashboardTasksViewable { get; set; }
		public bool IsDashboardInventoryViewable { get; set; }
		public bool IsDashboardAttendantViewable { get; set; }
		public bool IsDashboardAuditViewable { get; set; }
		public bool IsDashboardExperiencesViewable { get; set; }
		public string[] Emails { get; set; }
		public bool IsEnableEscalationAccepted { get; set; }
		public bool IsEscalationAcceptedGuests { get; set; }
		public int EscalationAcceptedTime { get; set; }
		public bool IsEnableEscalationCompleted { get; set; }
		public bool IsEscalationCompletedGuests { get; set; }
		public int EscalationCompletedTime { get; set; }
		public bool IsEnableExperienceStartEmail { get; set; }
		public bool IsEnableExperienceUpdateEmail { get; set; }
		public bool IsEnableExperienceEndEmail { get; set; }

		public bool IsSubGroup { get; set; }
		public Guid UserGroupId { get; set; }
		public Guid? UserSubGroupId { get; set; }
		public string TypeKey { get; set; }
	}

	public class GetListOfUserGroupsForMobileQuery : IRequest<IEnumerable<MobileUserGroup>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfUserGroupsForMobileQueryHandler : IRequestHandler<GetListOfUserGroupsForMobileQuery, IEnumerable<MobileUserGroup>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetListOfUserGroupsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<IEnumerable<MobileUserGroup>> Handle(GetListOfUserGroupsForMobileQuery request, CancellationToken cancellationToken)
		{
			var userGroups = await this._databaseContext.UserGroups
				.Include(ug => ug.Users)
				.Include(ug => ug.UserSubGroups)
				.ThenInclude(usg => usg.Users)
				.ToArrayAsync();

			var groups = new List<MobileUserGroup>();
			foreach(var userGroup in userGroups)
			{
				groups.Add(new MobileUserGroup
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

					UserGroupId = userGroup.Id,
					UserSubGroupId = null,
					TypeKey = "USER_GROUP"
				});

				foreach(var userSubGroup in userGroup.UserSubGroups)
				{
					groups.Add(new MobileUserGroup 
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

						UserGroupId = userGroup.Id,
						UserSubGroupId = userSubGroup.Id,
						TypeKey = "USER_SUB_GROUP",
					});
				}
			}

			return groups;
		}
	}
}
