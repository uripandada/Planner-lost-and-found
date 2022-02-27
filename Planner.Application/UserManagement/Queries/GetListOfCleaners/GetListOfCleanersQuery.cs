using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Queries.GetListOfCleaners
{
	public class CleanerListItemData
	{
		public Guid Id { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullNameInitials { get; set; }
		public string AvatarImageUrl { get; set; }
		public string GroupName { get; set; }
		public string SubGroupName { get; set; }
		public bool IsSubGroupLeader { get; set; }
		public bool IsActive { get; set; }
	}

	public class GetListOfCleanersQuery : IRequest<IEnumerable<CleanerListItemData>>
	{
		public string HotelId { get; set; }
	}

	public class GetListOfCleanersQueryHandler : IRequestHandler<GetListOfCleanersQuery, IEnumerable<CleanerListItemData>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly UserManager<User> _userManager;

		public GetListOfCleanersQueryHandler(IDatabaseContext databaseContext, UserManager<User> userManager)
		{
			this._databaseContext = databaseContext;
			this._userManager = userManager;
		}

		public async Task<IEnumerable<CleanerListItemData>> Handle(GetListOfCleanersQuery request, CancellationToken cancellationToken)
		{
			var users = (await this._databaseContext
				.Users
				.FromSqlInterpolated(@$"
					SELECT 
						u.*
					FROM 
						public.asp_net_users u
						LEFT JOIN public.asp_net_user_claims uc ON u.id = uc.user_id
						LEFT JOIN public.asp_net_user_roles ur ON u.id = ur.user_id
						LEFT JOIN public.asp_net_roles r ON r.id = ur.role_id
					WHERE 
						uc.claim_type = 'hotel_id' 
						AND uc.claim_value = {request.HotelId}
						AND r.normalized_name = 'ATTENDANT'
				")
				.Select(u => new CleanerListItemData
				{
					AvatarImageUrl = u.Avatar.FileUrl,
					FirstName = u.FirstName,
					LastName = u.LastName,
					FullNameInitials = "",
					Id = u.Id,
					UserName = u.UserName,
					GroupName = u.UserSubGroup.UserGroup.Name,
					SubGroupName = u.UserSubGroup.Name,
					IsActive = u.IsActive,
					IsSubGroupLeader = u.IsSubGroupLeader,
				})
				.ToArrayAsync())
				.Select(u => {
					u.FullNameInitials = $"{(u.FirstName.IsNotNull() ? u.FirstName[0] : "")}{(u.LastName.IsNotNull() ? u.LastName[0] : "")}";
					return u;
				})
				.ToArray();

			return users;
		}
	}
}
