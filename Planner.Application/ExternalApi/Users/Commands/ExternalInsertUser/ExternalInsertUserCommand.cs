using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.ExternalApi.Infrastructure;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExternalApi.Users.Commands.ExternalInsertUser
{
	public class ExternalInsertUserCommand: IRequest<ProcessResponseSimple<Guid>>
	{
		/// <summary>
		/// You can choose to set either hotelGroupId or hotelGroupKey
		/// </summary>
		public Guid? HotelGroupId { get; set; }
		/// <summary>
		/// You can choose to set either hotelGroupId or hotelGroupKey
		/// </summary>
		public string HotelGroupKey { get; set; }



		public string Email { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string PasswordConfirmation { get; set; }
		public string PhoneNumber { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string ConnectionName { get; set; }
		public string RegistrationNumber { get; set; }
		public string Language { get; set; }
		public string[] HotelIds { get; set; }
		public Guid RoleId { get; set; }
		public bool IsSubGroupLeader { get; set; }
		public bool IsActive { get; set; }
		public Guid? UserSubGroupId { get; set; }
		public Guid? UserGroupId { get; set; }
		public string AvatarImageUrl { get; set; }

	}

	public class ExternalInsertUserCommandHandler: ExternalApiBaseHandler, IRequestHandler<ExternalInsertUserCommand, ProcessResponseSimple<Guid>>, IAmWebApplicationHandler
	{
		private readonly IPasswordHasher<User> _passwordHasher;

		public ExternalInsertUserCommandHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IPasswordHasher<User> passwordHasher)
		{
			this._masterDatabaseContext = masterDatabaseContext;
			this._databaseContext = databaseContext;
			this._contextAccessor = contextAccessor;

			this._passwordHasher = passwordHasher;
		}

		public async Task<ProcessResponseSimple<Guid>> Handle(ExternalInsertUserCommand request, CancellationToken cancellationToken)
		{
			var initResult = await this._Initialize(request.HotelGroupId, request.HotelGroupKey);
			if (initResult != null)
			{
				return initResult;
			}

			var validationResult = await this._ValidateRequest(request);
			if (validationResult != null)
			{
				return validationResult;
			}

			var newUser = new User
			{
				Id = Guid.NewGuid(),
				ConnectionName = request.ConnectionName,
				Email = request.Email,
				FirstName = request.FirstName,
				Language = request.Language,
				LastName = request.LastName,
				OriginalHotel = null,
				PhoneNumber = request.PhoneNumber,
				RegistrationNumber = request.RegistrationNumber,
				UserName = request.UserName,
				UserSubGroupId = request.UserSubGroupId,
				UserGroupId = request.UserGroupId,
				IsSubGroupLeader = request.IsSubGroupLeader,
				IsActive = request.IsActive,
				DefaultAvatarColorHex = "#EEEEEE",
				AccessFailedCount = 0,
				ConcurrencyStamp = Guid.NewGuid().ToString(),
				EmailConfirmed = false,
				IsOnDuty = false,
				LockoutEnabled = true,
				LockoutEnd = DateTime.UtcNow.AddDays(-1),
				PhoneNumberConfirmed = false,
				SecurityStamp = Guid.NewGuid().ToString(),
				TwoFactorEnabled = false,
				NormalizedEmail = request.Email?.Trim()?.ToUpper(),
				NormalizedUserName = request.UserName?.Trim()?.ToUpper(),
			};
			newUser.PasswordHash = this._passwordHasher.HashPassword(newUser, request.Password);

			var role = await this._databaseContext.Roles.Where(r => r.Id == request.RoleId).FirstOrDefaultAsync();

			var userRoles = new List<IdentityUserRole<Guid>>();
			userRoles.Add(new IdentityUserRole<Guid> { RoleId = role.Id, UserId = newUser.Id });
				
			var userClaims = new List<IdentityUserClaim<Guid>>();
			userClaims.Add(new IdentityUserClaim<Guid>() { ClaimType = ClaimsKeys.HotelGroupId, ClaimValue = this._hotelGroupId.ToString(), UserId = newUser.Id });
			if (role.NormalizedName == SystemDefaults.Roles.Administrator.NormalizedName)
			{
				userClaims.Add(new IdentityUserClaim<Guid>() { ClaimType = ClaimsKeys.HotelId, ClaimValue = "ALL", UserId = newUser.Id });
			}
			else
			{
				foreach (var hid in request.HotelIds)
				{
					userClaims.Add(new IdentityUserClaim<Guid>() { ClaimType = ClaimsKeys.HotelId, ClaimValue = hid, UserId = newUser.Id });
				}
			}
			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				await this._databaseContext.Users.AddAsync(newUser);
				await this._databaseContext.UserRoles.AddRangeAsync(userRoles);
				await this._databaseContext.UserClaims.AddRangeAsync(userClaims);

				await this._databaseContext.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new ProcessResponseSimple<Guid>
				{
					Data = newUser.Id,
					IsSuccess = true,
					Message = "User created."
				};
			}
		}

		private async Task<ProcessResponseSimple<Guid>> _Initialize(Guid? hotelGroupId, string hotelGroupKey)
		{
			var authResult = await this.AuthorizeExternalClient();
			if (!authResult.IsSuccess)
				return new ProcessResponseSimple<Guid> { IsSuccess = false, Message = authResult.Message, Data = Guid.Empty };

			var initResult = this.InitializeHotelGroupContext(hotelGroupId, hotelGroupKey);
			if (!initResult.IsSuccess)
				return new ProcessResponseSimple<Guid> { IsSuccess = false, Message = initResult.Message, Data = Guid.Empty };

			return null;
		}

		private async Task<ProcessResponseSimple<Guid>> _ValidateRequest(ExternalInsertUserCommand request)
		{
			if (request.Email.IsNotNull())
			{
				var emailValue = request.Email.ToLower().Trim();
				var user = await this._databaseContext.Users.Where(u => u.Email.ToLower() == emailValue).FirstOrDefaultAsync();

				if (user != null)
				{
					return new ProcessResponseSimple<Guid>
					{
						Data = Guid.Empty,
						IsSuccess = false,
						Message = $"Email {request.Email} already in use.",
					};
				}
			}

			if (request.UserName.IsNull())
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "User name is required.",
				};
			}
			else
			{
				var usernameValue = request.UserName.ToLower().Trim();
				var user = await this._databaseContext.Users.Where(u => u.UserName.ToLower() == usernameValue).FirstOrDefaultAsync();

				if (user != null)
				{
					return new ProcessResponseSimple<Guid>
					{
						Data = Guid.Empty,
						IsSuccess = false,
						Message = $"User name {request.UserName} already exists.",
					};
				}
			}

			if (request.FirstName.IsNull())
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "First name is required.",
				};
			}

			if (request.LastName.IsNull())
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "Last name is required.",
				};
			}

			if (request.Password.IsNull())
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "Password is required.",
				};
			}
			else if (request.PasswordConfirmation.IsNull())
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "Password confirmation is required.",
				};
			}
			else if (request.Password != request.PasswordConfirmation)
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "Password and password confirmation don't match.",
				};
			}
			else if (request.Password.Length < 4)
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "Minimum password length is 4.",
				};
			}

			if (request.RoleId == Guid.Empty)
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "Role is required.",
				};
			}
			else
			{
				var role = await this._databaseContext.Roles.FindAsync(request.RoleId);
				if (role == null)
				{
					return new ProcessResponseSimple<Guid>
					{
						Data = Guid.Empty,
						IsSuccess = false,
						Message = $"Role ID {request.RoleId} doesn't exist.",
					};
				}
			}

			return null;
		}
	}


}
