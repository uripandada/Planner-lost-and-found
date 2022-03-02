using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.ExternalApi.Infrastructure;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExternalApi.Users.Queries.ExternalGetListOfUsersQuery
{
	public class ExternalUser
	{
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ConnectionName { get; set; }
		public string RegistrationNumber { get; set; }
		public string Language { get; set; }
		public string OriginalHotel { get; set; }
		public Guid? UserSubGroupId { get; set; }
		public Guid? UserGroupId { get; set; }
		public bool IsGroupLeader { get; set; }
		public bool IsSubGroupLeader { get; set; }
		public bool IsActive { get; set; }
		public string DefaultAvatarColorHex { get; set; }
		public string AvatarUrl { get; set; }
		public bool IsOnDuty { get; set; }
	}

	public class ExternalGetListOfUsersQuery : IRequest<ProcessResponseSimple<IEnumerable<ExternalUser>>>
	{
		/// <summary>
		/// You can choose to set either hotelGroupId or hotelGroupKey
		/// </summary>
		public Guid? HotelGroupId { get; set; }
		/// <summary>
		/// You can choose to set either hotelGroupId or hotelGroupKey
		/// </summary>
		public string HotelGroupKey { get; set; }
	}

	public class ExternalGetListOfUsersQueryHandler : ExternalApiBaseHandler, IRequestHandler<ExternalGetListOfUsersQuery, ProcessResponseSimple<IEnumerable<ExternalUser>>>, IAmWebApplicationHandler
	{
		public ExternalGetListOfUsersQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._masterDatabaseContext = masterDatabaseContext;
			this._databaseContext = databaseContext;
			this._contextAccessor = contextAccessor;
		}

		public async Task<ProcessResponseSimple<IEnumerable<ExternalUser>>> Handle(ExternalGetListOfUsersQuery request, CancellationToken cancellationToken)
		{
			var initResult = await this._Initialize(request.HotelGroupId, request.HotelGroupKey);
			if (initResult != null)
			{
				return initResult;
			}

			var users = await this._databaseContext
				.Users
				.Select(u => new ExternalUser
				{
					Id = u.Id,
					FirstName = u.FirstName,
					AvatarUrl = u.Avatar == null ? null : u.Avatar.FileUrl,
					ConnectionName = u.ConnectionName,
					DefaultAvatarColorHex = u.DefaultAvatarColorHex,
					IsActive = u.IsActive,
					IsGroupLeader = u.UserGroupId != null && u.UserSubGroupId == null,
					IsOnDuty = u.IsOnDuty,
					IsSubGroupLeader = u.IsSubGroupLeader,
					Language = u.Language,
					LastName = u.LastName,
					RegistrationNumber = u.RegistrationNumber,
					UserGroupId = u.UserGroupId,
					UserSubGroupId = u.UserSubGroupId,
				})
				.ToListAsync();

			return new ProcessResponseSimple<IEnumerable<ExternalUser>>
			{
				Data = users,
				IsSuccess = true,
				Message = "Users loaded.",
			};
		}

		private async Task<ProcessResponseSimple<IEnumerable<ExternalUser>>> _Initialize(Guid? hotelGroupId, string hotelGroupKey)
		{
			var authResult = await this.AuthorizeExternalClient();
			if (!authResult.IsSuccess)
				return new ProcessResponseSimple<IEnumerable<ExternalUser>> { IsSuccess = false, Message = authResult.Message, Data = new ExternalUser[0] };

			var initResult = this.InitializeHotelGroupContext(hotelGroupId, hotelGroupKey);
			if (!initResult.IsSuccess)
				return new ProcessResponseSimple<IEnumerable<ExternalUser>> { IsSuccess = false, Message = initResult.Message, Data = new ExternalUser[0] };

			return null;
		}
	}
}
