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

namespace Planner.Application.UserManagement.Commands.UpdateUser
{

	public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
	{
		private readonly IDatabaseContext _databaseContext;

		public UpdateUserCommandValidator(IDatabaseContext masterDatabaseContext)
		{
			this._databaseContext = masterDatabaseContext;

			RuleFor(command => command.FirstName).NotEmpty();
			RuleFor(command => command.LastName).NotEmpty();
			RuleFor(command => command.RoleId).NotEmpty();
			RuleFor(command => command.UserName).NotEmpty().MustAsync(async (command, value, propertyValidatorContext, cancellationToken) =>
			{
				var valueParam = value.ToLower();
				var user = await this._databaseContext.Users.Where(t => t.UserName.ToLower() == valueParam && t.Id != command.Id).FirstOrDefaultAsync();
				return user == null;
			}).WithMessage("USER_USERNAME_ALREADY_EXISTS");

			RuleFor(command => command.Password).Must((command, value, propertyvalidatorcontext) => {
				if (command.Password.IsNotNull() && command.PasswordConfirmation.IsNotNull())
				{
					return command.Password == command.PasswordConfirmation;
				}
				else
				{
					return true;
				}
			}).WithMessage("PASSWORDS_DONT_MATCH");

			RuleFor(command => command.Password).Must((command, value, propertyvalidatorcontext) => {
				if (command.Password.IsNotNull())
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
	public class UpdateUserCommand : IRequest<ProcessResponse<UserHierarchyData>>
	{
		public Guid Id { get; set; }
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
		public Guid? UserSubGroupId { get; set; }
		public Guid? UserGroupId { get; set; }
		public SaveAvatarData AvatarData { get; set; }
		public bool IsSubGroupLeader { get; set; }
		public bool IsActive { get; set; }
		public string DefaultAvatarColorHex { get; set; }
	}

	public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ProcessResponse<UserHierarchyData>>, IAmWebApplicationHandler
	{
		private readonly UserManager<User> userManager;
		private readonly RoleManager<Role> roleManager;
		private readonly IFileService _fileService;
		private readonly IDatabaseContext _databaseContext;

		public UpdateUserCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IFileService fileService, IDatabaseContext databaseContext)
		{
			this.userManager = userManager;
			this.roleManager = roleManager;
			this._fileService = fileService;
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse<UserHierarchyData>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
		{
			var user = await this.userManager.FindByIdAsync(request.Id.ToString());

			user.Email = request.Email;
			user.UserName = request.UserName;
			user.PhoneNumber = request.PhoneNumber;
			user.FirstName = request.FirstName;
			user.LastName = request.LastName;
			user.ConnectionName = request.ConnectionName;
			user.RegistrationNumber = request.RegistrationNumber;
			user.Language = request.Language;
			user.OriginalHotel = request.OriginalHotel;
			user.UserSubGroupId = request.UserSubGroupId;
			user.UserGroupId = request.UserGroupId;
			user.IsSubGroupLeader = request.IsSubGroupLeader;
			user.IsActive = request.IsActive;

			if (request.DefaultAvatarColorHex.IsNotNull())
			{
				user.DefaultAvatarColorHex = request.DefaultAvatarColorHex;
			}

			await this.userManager.UpdateAsync(user);
			var role = await roleManager.FindByIdAsync(request.RoleId);
			var roles =await this.userManager.GetRolesAsync(user);
			if (roles.Count > 1 || roles.Single() != role.Name)
			{
				await userManager.RemoveFromRolesAsync(user, roles);
				await userManager.AddToRoleAsync(user, role.Name);
			}

			var claims = await userManager.GetClaimsAsync(user);
			var hotelIdClaims = claims.Where(x => x.Type == ClaimsKeys.HotelId).ToArray();

			if (hotelIdClaims.Any())
			{
				await userManager.RemoveClaimsAsync(user, hotelIdClaims);
			}

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

			if (!string.IsNullOrWhiteSpace(request.Password) && request.Password == request.PasswordConfirmation)
			{
				var resetPasswordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
				await this.userManager.ResetPasswordAsync(user, resetPasswordToken, request.Password);
			}

			if (request.AvatarData != null && request.AvatarData.HasChanged)
			{
				var saveAvatarResult = await this._fileService.SaveAvatar(user.Id, request.AvatarData.File, request.AvatarData.FileName);

				var existingAvatar = await this._databaseContext.ApplicationUserAvatar.FindAsync(user.Id);
				if (existingAvatar == null)
				{
					var avatar = new ApplicationUserAvatar
					{
						File = request.AvatarData.File,
						FileName = request.AvatarData.FileName,
						FileUrl = saveAvatarResult.FileUrl,
						Id = user.Id
					};

					await this._databaseContext.ApplicationUserAvatar.AddAsync(avatar);
				}
				else
				{
					existingAvatar.File = request.AvatarData.File;
					existingAvatar.FileName = request.AvatarData.FileName;
					existingAvatar.FileUrl = saveAvatarResult.FileUrl;
				}
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}

			return new ProcessResponse<UserHierarchyData>
			{
				Data = UserHierarchyData.Create(user),
				IsSuccess = true,
				Message = "User updated"
			};
		}
	}
}
