using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.Users.Commands.InsertUserForMobile
{
	public class InsertUserForMobileCommand : IRequest<ProcessResponseSimple<Guid>>
	{
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
		public string RoleId { get; set; }
		public bool IsSubGroupLeader { get; set; }
		public bool IsActive { get; set; }
		public Guid? UserSubGroupId { get; set; }
		public Guid? UserGroupId { get; set; }
		public string AvatarImageUrl { get; set; }
	}

	public class InsertUserForMobileCommandHandler : IRequestHandler<InsertUserForMobileCommand, ProcessResponseSimple<Guid>>, IAmWebApplicationHandler
	{
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ISystemEventsService _eventsService;
		private readonly Guid _hotelGroupId;

		public InsertUserForMobileCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, ISystemEventsService eventsService)
		{
			this._userManager = userManager;
			this._roleManager = roleManager;
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._eventsService = eventsService;
			this._hotelGroupId = this._httpContextAccessor.HttpContext.User.HotelGroupId();
		}

		public async Task<ProcessResponseSimple<Guid>> Handle(InsertUserForMobileCommand request, CancellationToken cancellationToken)
		{
			var validationResult = await this._ValidateRequest(request);
			if(validationResult != null)
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
			};

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var createResult = await this._userManager.CreateAsync(newUser);


				if (createResult.Succeeded)
				{
					var user = await this._userManager.FindByIdAsync(newUser.Id.ToString());

					var role = await this._roleManager.FindByIdAsync(request.RoleId);
					await this._userManager.AddToRoleAsync(user, role.Name);

					await this._userManager.AddClaimAsync(user, new Claim(ClaimsKeys.HotelGroupId, this._databaseContext.HotelGroupTenant.Id.ToString()));
					if (role.NormalizedName == SystemDefaults.Roles.Administrator.NormalizedName)
					{
						await this._userManager.AddClaimAsync(user, new Claim(ClaimsKeys.HotelId, "ALL"));
					}
					else
					{
						var claimsToAdd = new List<Claim>();
						foreach (var item in request.HotelIds)
						{
							claimsToAdd.Add(new Claim(ClaimsKeys.HotelId, item));
						}
						await this._userManager.AddClaimsAsync(user, claimsToAdd);
					}

					var setPasswordResult = await this._userManager.AddPasswordAsync(user, request.Password);
					if (!setPasswordResult.Succeeded)
					{
						return new ProcessResponseSimple<Guid>
						{
							IsSuccess = false,
							Message = "Setting user password failed."
						};
					}

					if (request.AvatarImageUrl.IsNotNull())
					{
						var avatar = new ApplicationUserAvatar
						{
							File = new byte[0],
							FileName = "filestack-avatar.jpg",
							FileUrl = request.AvatarImageUrl,
							Id = user.Id
						};

						await this._databaseContext.ApplicationUserAvatar.AddAsync(avatar);
						await this._databaseContext.SaveChangesAsync(cancellationToken);
					}

					await transaction.CommitAsync(cancellationToken);

					return new ProcessResponseSimple<Guid>
					{
						Data = newUser.Id,
						IsSuccess = true,
						Message = "User created."
					};
				}

				return new ProcessResponseSimple<Guid>
				{
					IsSuccess = false,
					Data = Guid.Empty,
					Message = "Creating user failed."
				};
			}
		}

		private async Task<ProcessResponseSimple<Guid>> _ValidateRequest(InsertUserForMobileCommand request)
		{
			if (request.Email.IsNotNull())
			{
				var emailValue = request.Email.ToLower().Trim();
				var user = await this._databaseContext.Users.Where(u => u.Email.ToLower() == emailValue).FirstOrDefaultAsync();

				if(user != null)
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

				if(user != null)
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
			else if(request.Password != request.PasswordConfirmation)
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "Password and password confirmation don't match.",
				};
			}
			else if(request.Password.Length < 4)
			{
				return new ProcessResponseSimple<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					Message = "Minimum password length is 4.",
				};
			}

			if (request.RoleId.IsNull())
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
				if(role == null)
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
