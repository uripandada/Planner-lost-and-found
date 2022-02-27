using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.UserManagement.Models;
using Planner.Common;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.UserManagement.Commands.InsertUser
{

	public class InsertUserCommandValidator : AbstractValidator<InsertUserCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public InsertUserCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.FirstName).NotEmpty();
			RuleFor(command => command.LastName).NotEmpty();
			RuleFor(command => command.RoleId).NotEmpty();
			RuleFor(command => command.UserName).NotEmpty().MustAsync(async (command, value, propertyValidatorContext, cancellationToken) =>
			{
				var valueParam = value.ToLower();
				var user = await this._databaseContext.Users.Where(t => t.UserName.ToLower() == valueParam).FirstOrDefaultAsync();
				return user == null;
			}).WithMessage("USER_USERNAME_ALREADY_EXISTS");

			RuleFor(command => command.Password).Must((command, value, propertyvalidatorcontext) => {
				if(command.Password.IsNotNull() && command.PasswordConfirmation.IsNotNull())
				{
					return command.Password == command.PasswordConfirmation;
				}
				else
				{
					return true;
				}
			}).WithMessage("PASSWORDS_DONT_MATCH");

			RuleFor(command => command.Password).Must((command, value, propertyvalidatorcontext) => {
				if(command.Password.IsNotNull())
				{
					return command.Password.Length >= 4;
				}
				else
				{
					return true;
				}
			}).WithMessage("PASSWORD_TOO_SHORT");
		}
	}

	public class InsertUserCommand : IRequest<ProcessResponse<UserHierarchyData>>
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
		public string OriginalHotel { get; set; }
		public string[] HotelIds { get; set; }
		public string RoleId { get; set; }
		public bool IsSubGroupLeader { get; set; }
		public bool IsActive { get; set; }
		public Guid? UserSubGroupId { get; set; }
		public Guid? UserGroupId { get; set; }
		public SaveAvatarData AvatarData { get; set; }
		public string DefaultAvatarColorHex { get; set; }
	}


	public class InsertUserCommandHandler : IRequestHandler<InsertUserCommand, ProcessResponse<UserHierarchyData>>, IAmWebApplicationHandler
	{
		private readonly UserManager<User> userManager;
		private readonly RoleManager<Role> roleManager;
		private readonly IFileService _fileService;
		private readonly IDatabaseContext _databaseContext;


		public InsertUserCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IFileService fileService, IDatabaseContext databaseContext)
		{
			this.userManager = userManager;
			this.roleManager = roleManager;
			this._fileService = fileService;
			this._databaseContext = databaseContext;
		}
		public async Task<ProcessResponse<UserHierarchyData>> Handle(InsertUserCommand request, CancellationToken cancellationToken)
		{
			var newUser = new User
			{
				Id = Guid.NewGuid(),
				ConnectionName = request.ConnectionName,
				Email = request.Email,
				FirstName = request.FirstName,
				Language = request.Language,
				LastName = request.LastName,
				OriginalHotel = request.OriginalHotel,
				PhoneNumber = request.PhoneNumber,
				RegistrationNumber = request.RegistrationNumber,
				UserName = request.UserName,
				UserSubGroupId = request.UserSubGroupId,
				UserGroupId = request.UserGroupId,
				IsSubGroupLeader = request.IsSubGroupLeader,
				IsActive = request.IsActive,
				DefaultAvatarColorHex = request.DefaultAvatarColorHex.IsNull() ? "#EEEEEE" : request.DefaultAvatarColorHex,
			};

			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var createResult = await this.userManager.CreateAsync(newUser);


				if (createResult.Succeeded)
				{
					var user = await userManager.FindByIdAsync(newUser.Id.ToString());

					var role = await roleManager.FindByIdAsync(request.RoleId);
					await userManager.AddToRoleAsync(user, role.Name);

					await this.userManager.AddClaimAsync(user, new Claim(ClaimsKeys.HotelGroupId, this._databaseContext.HotelGroupTenant.Id.ToString()));
					if (role.NormalizedName == SystemDefaults.Roles.Administrator.NormalizedName)
					{
						await this.userManager.AddClaimAsync(user, new Claim(ClaimsKeys.HotelId, "ALL"));
					}
					else
					{
						var claimsToAdd = new List<Claim>();
						foreach (var item in request.HotelIds)
						{
							claimsToAdd.Add(new Claim(ClaimsKeys.HotelId, item));
						}
						await this.userManager.AddClaimsAsync(user, claimsToAdd);
					}

					var setPasswordResult = await this.userManager.AddPasswordAsync(user, request.Password);
					if (!setPasswordResult.Succeeded)
					{
						return new ProcessResponse<UserHierarchyData>
						{
							IsSuccess = false,
							HasError = true,
							Message = "Setting user password failed."
						};
					}

					if (request.AvatarData != null)
					{
						var saveAvatarResult = await this._fileService.SaveAvatar(user.Id, request.AvatarData.File, request.AvatarData.FileName);

						var avatar = new ApplicationUserAvatar
						{
							File = request.AvatarData.File,
							FileName = request.AvatarData.FileName,
							FileUrl = saveAvatarResult.FileUrl,
							Id = user.Id
						};

						await this._databaseContext.ApplicationUserAvatar.AddAsync(avatar);
						await this._databaseContext.SaveChangesAsync(cancellationToken);
					}

					await transaction.CommitAsync(cancellationToken);

					return new ProcessResponse<UserHierarchyData>
					{
						Data = UserHierarchyData.Create(newUser),
						IsSuccess = true,
						Message = "User created."
					};
				}

				return new ProcessResponse<UserHierarchyData>
				{
					IsSuccess = false,
					HasError = true,
					Message = "Creating user failed."
				};
			}

			
		}
	}
}

