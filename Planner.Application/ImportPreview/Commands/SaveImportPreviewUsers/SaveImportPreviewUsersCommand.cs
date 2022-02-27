using MediatR;
using Planner.Domain.Entities;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Planner.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Planner.Application.ImportPreview.Commands.UploadImportPreviewUsers;
using System.Security.Claims;
using Planner.Domain.Values;
using Planner.Common;

namespace Planner.Application.ImportPreview.Commands.SaveImportPreviewUsers
{
	public class SaveUserImportResult
	{
		public Guid? Id { get; set; }
		public string UserName { get; set; }
		public bool HasErrors { get; set; }
		public string Message { get; set; }
	}

	public class SaveImportPreviewUsersCommand : IRequest<ProcessResponse<IEnumerable<SaveUserImportResult>>>
	{
		public IEnumerable<ImportUserPreview> Users { get; set; }
	}
	
	public class SaveImportPreviewUsersCommandHandler : IRequestHandler<SaveImportPreviewUsersCommand, ProcessResponse<IEnumerable<SaveUserImportResult>>>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<Role> _roleManager;

		private class _UserInsertData
		{
			public List<IdentityUserClaim<Guid>> ClaimsToInsert { get; set; } = new List<IdentityUserClaim<Guid>>();
			public User User { get; set; }
			public Role Role { get; set; }
			public string Password { get; set; }
		}
		private class _UserUpdateData
		{
			public List<IdentityUserClaim<Guid>> ClaimsToInsert { get; set; } = new List<IdentityUserClaim<Guid>>();
			public List<IdentityUserClaim<Guid>> ClaimsToDelete { get; set; } = new List<IdentityUserClaim<Guid>>();
			public User User { get; set; }
			public Role NewRole { get; set; }
			public Role OldRole { get; set; }
			public bool RoleChanged { get; set; }
		}

		public SaveImportPreviewUsersCommandHandler(UserManager<User> userManager, RoleManager<Role> roleManager, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._userManager = userManager;
			this._roleManager = roleManager;
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<IEnumerable<SaveUserImportResult>>> Handle(SaveImportPreviewUsersCommand request, CancellationToken cancellationToken)
		{
			var roles = await this._databaseContext.Roles.ToDictionaryAsync(role => role.Name.Trim().ToLower(), role => role);

			var userGroups = await this._databaseContext.UserGroups.ToDictionaryAsync(userGroup => userGroup.Name.Trim().ToLower(), userGroup => userGroup);
			var userGroupsToInsert = new List<UserGroup>();

			var userSubGroups = await this._databaseContext.UserSubGroups.ToDictionaryAsync(userSubGroup => userSubGroup.Name.Trim().ToLower(), userSubGroup => userSubGroup);
			var userSubGroupsToInsert = new List<UserSubGroup>();

			var hotelsMap = await this._databaseContext.Hotels.Select(h => new { Name = h.Name.Trim().ToLower(), Id = h.Id }).ToDictionaryAsync(h => h.Name);

			var usersToInsert = new List<_UserInsertData>();
			var usersToUpdate = new List<_UserUpdateData>();

			var users = await this._userManager.Users
				.Include(u => u.UserClaims)
				.Include(u => u.UserRoles)
				.ToListAsync();

			foreach (ImportUserPreview u in request.Users.Where(user => user.HasError == false).ToArray())
			{
				UserGroup userGroup = null;
				if (u.UserGroup.IsNotNull())
				{
					string normalizedGroupName = u.UserGroup.Trim().ToLower();
					if (userGroups.ContainsKey(normalizedGroupName))
					{
						userGroup = userGroups[normalizedGroupName];
					}
					else
					{
						userGroup = new UserGroup
						{
							Id = Guid.NewGuid(),
							CreatedAt = DateTime.UtcNow,
							CreatedById = this._userId,
							ModifiedAt = DateTime.UtcNow,
							ModifiedById = this._userId,
							Name = u.UserGroup
						};
						userGroups.Add(normalizedGroupName, userGroup);
						userGroupsToInsert.Add(userGroup);
					}
				}

				UserSubGroup userSubGroup = null;
				if (u.UserSubGroup.IsNotNull())
				{
					string normalizedSubGroupName = u.UserSubGroup.Trim().ToLower();
					if (userSubGroups.ContainsKey(normalizedSubGroupName))
					{
						userSubGroup = userSubGroups[normalizedSubGroupName];
					}
					else
					{
						userSubGroup = new UserSubGroup
						{
							Id = Guid.NewGuid(),
							CreatedAt = DateTime.UtcNow,
							CreatedById = this._userId,
							ModifiedAt = DateTime.UtcNow,
							ModifiedById = this._userId,
							UserGroupId = userGroup.Id,
							Name = u.UserSubGroup
						};

						userSubGroups.Add(normalizedSubGroupName, userSubGroup);
						userSubGroupsToInsert.Add(userSubGroup);
					}
				}

				var normalizedUsername = u.UserName.Trim().ToLower();
				var existingUser = users.FirstOrDefault(usr => usr.UserName.Trim().ToLower() == normalizedUsername);

				if(existingUser == null)
				{
					var userId = Guid.NewGuid();
					var normalizedRoleName = u.RoleName.Trim().ToLower();
					var role = roles[normalizedRoleName];

					var claimsToInsert = new List<IdentityUserClaim<Guid>>();
					if(role.HotelAccessTypeKey == "ALL")
					{
						var claim = new IdentityUserClaim<Guid>();
						claim.ClaimType = ClaimsKeys.HotelId;
						claim.ClaimValue = "ALL";
						claim.UserId = userId;

						claimsToInsert.Add(claim);
					}
					else
					{
						var hotelNames = u.AccessibleHotels.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(h => h.Trim().ToLower()).ToArray();
						foreach(var hotelName in hotelNames.Where(hn => hn != "all").ToArray())
						{
							var hotel = hotelsMap[hotelName];

							var claim = new IdentityUserClaim<Guid>();
							claim.ClaimType = ClaimsKeys.HotelId;
							claim.ClaimValue = hotel.Id;
							claim.UserId = userId;

							claimsToInsert.Add(claim);
						}
					}

					var hotelGroupIdClaim = new IdentityUserClaim<Guid>();
					hotelGroupIdClaim.ClaimType = ClaimsKeys.HotelGroupId;
					hotelGroupIdClaim.ClaimValue = this._databaseContext.HotelGroupTenant.Id.ToString();
					hotelGroupIdClaim.UserId = userId;
					claimsToInsert.Add(hotelGroupIdClaim);

					usersToInsert.Add(new _UserInsertData
					{
						ClaimsToInsert = claimsToInsert,
						Password = u.Password,
						Role = role,
						User = new User
						{
							Id = userId,
							ConnectionName = null,
							Email = u.Email == "" ? null : u.Email,
							FirstName = u.FirstName,
							Language = "en",
							LastName = u.LastName,
							OriginalHotel = null,
							PhoneNumber = u.Phone == "" ? null : u.Phone,
							RegistrationNumber = null,
							UserName = u.UserName,
							UserSubGroupId = userSubGroup?.Id ?? null,
							UserGroupId = userGroup?.Id ?? null,
							IsSubGroupLeader = u.IsUserSubGroupLeader,
							IsActive = true,
							DefaultAvatarColorHex = u.DefaultAvatarColorHex.IsNull() ? "#EEEEEE" : u.DefaultAvatarColorHex,
						}
					});
				}
				else
				{
					var normalizedRoleName = u.RoleName.Trim().ToLower();
					var role = roles[normalizedRoleName];

					var isInNewRole = await this._userManager.IsInRoleAsync(existingUser, role.Name);
					var oldRoleId = existingUser.UserRoles?.FirstOrDefault()?.RoleId;
					var oldRole = oldRoleId == null ? null : roles.Values.FirstOrDefault(r => r.Id == oldRoleId);

					var claimsToInsert = new List<IdentityUserClaim<Guid>>();
					var claimsToDelete = new List<IdentityUserClaim<Guid>>();
					if(role.HotelAccessTypeKey == "ALL")
					{
						var existingHotelIdClaims = existingUser.UserClaims.Where(c => c.ClaimType == ClaimsKeys.HotelId).ToArray();
						var allClaimFound = false;

						foreach(var ec in existingHotelIdClaims)
						{
							if(ec.ClaimValue == "ALL")
							{
								allClaimFound = true;
							}
							else
							{
								claimsToDelete.Add(ec);
							}
						}

						if (!allClaimFound)
						{
							var claim = new IdentityUserClaim<Guid>();
							claim.ClaimType = ClaimsKeys.HotelId;
							claim.ClaimValue = "ALL";
							claim.UserId = existingUser.Id;

							claimsToInsert.Add(claim);
						}
					}
					else
					{
						var existingHotelIdClaims = existingUser.UserClaims.Where(c => c.ClaimType == ClaimsKeys.HotelId).ToArray();
						
						var hotelNames = u.AccessibleHotels.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(h => h.Trim().ToLower()).ToArray();
						var hotels = hotelNames.Where(hn => hn.Trim().ToLower() != "all").Select(hn => hotelsMap[hn]).ToArray();

						foreach(var existingHotelIdClaim in existingHotelIdClaims)
						{
							var existingHotel = hotels.FirstOrDefault(h => h.Id == existingHotelIdClaim.ClaimValue);
							if(existingHotel == null)
							{
								claimsToDelete.Add(existingHotelIdClaim);
							}
						}

						foreach(var hotel in hotels)
						{
							var existingHotelClaim = existingHotelIdClaims.FirstOrDefault(hc => hc.ClaimValue == hotel.Id);
							if(existingHotelClaim == null)
							{
								var claim = new IdentityUserClaim<Guid>();
								claim.ClaimType = ClaimsKeys.HotelId;
								claim.ClaimValue = hotel.Id;
								claim.UserId = existingUser.Id;

								claimsToInsert.Add(claim);
							}
						}
					}

					var hotelGroupIdClaim = existingUser.UserClaims.FirstOrDefault(uc => uc.ClaimType == ClaimsKeys.HotelGroupId);
					if(hotelGroupIdClaim == null)
					{
						hotelGroupIdClaim = new IdentityUserClaim<Guid>();
						hotelGroupIdClaim.ClaimType = ClaimsKeys.HotelGroupId;
						hotelGroupIdClaim.ClaimValue = this._databaseContext.HotelGroupTenant.Id.ToString();
						hotelGroupIdClaim.UserId = existingUser.Id;
						claimsToInsert.Add(hotelGroupIdClaim);
					}

					existingUser.UserGroupId = userGroup?.Id ?? null;
					existingUser.UserSubGroupId = userSubGroup?.Id ?? null;
					existingUser.FirstName = u.FirstName;
					existingUser.LastName = u.LastName;
					existingUser.Email = u.LastName;
					existingUser.PhoneNumber = u.LastName;
					existingUser.IsSubGroupLeader = u.IsUserSubGroupLeader;

					var userUpdateData = new _UserUpdateData
					{
						User = existingUser,
						OldRole = oldRole,
						NewRole = role,
						ClaimsToDelete = claimsToDelete,
						ClaimsToInsert = claimsToInsert,
						RoleChanged = !isInNewRole,
					};

					usersToUpdate.Add(userUpdateData);
				}
			}

			if(userGroupsToInsert.Any() || userSubGroupsToInsert.Any())
			{
				using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
				{
					await this._databaseContext.UserGroups.AddRangeAsync(userGroupsToInsert);
					await this._databaseContext.UserSubGroups.AddRangeAsync(userSubGroupsToInsert);

					await this._databaseContext.SaveChangesAsync(cancellationToken);

					await transaction.CommitAsync(cancellationToken);
				}
			}

			var saveDbChanges = false;
			var saveResults = new List<SaveUserImportResult>();

			foreach(var user in usersToUpdate)
			{
				var userUpdateResult = await this._userManager.UpdateAsync(user.User);
				if (!userUpdateResult.Succeeded)
				{
					saveResults.Add(new SaveUserImportResult
					{
						HasErrors = true,
						Id = user.User.Id,
						Message = "Failed to update the user.",
						UserName = user.User.UserName,
					});

					continue;
				}

				if (user.RoleChanged)
				{
					if(user.OldRole != null)
					{
						await this._userManager.RemoveFromRoleAsync(user.User, user.OldRole.Name);
					}

					await this._userManager.AddToRoleAsync(user.User, user.NewRole.Name);
				}

				if(user.ClaimsToDelete != null && user.ClaimsToDelete.Any())
				{
					this._databaseContext.UserClaims.RemoveRange(user.ClaimsToDelete);
					saveDbChanges = true;
				}

				if(user.ClaimsToInsert != null && user.ClaimsToInsert.Any())
				{
					await this._databaseContext.UserClaims.AddRangeAsync(user.ClaimsToInsert);
					saveDbChanges = true;
				}
			}

			foreach (var user in usersToInsert)
			{
				var createResult = await this._userManager.CreateAsync(user.User);
				if (!createResult.Succeeded)
				{
					saveResults.Add(new SaveUserImportResult
					{
						HasErrors = true,
						Id = null,
						Message = "Failed to create.",
						UserName = user.User.UserName,
					});
					continue;
				}

				var addToRoleResult = await this._userManager.AddToRoleAsync(user.User, user.Role.Name);
				if (!addToRoleResult.Succeeded)
				{
					saveResults.Add(new SaveUserImportResult
					{
						HasErrors = true,
						Id = user.User.Id,
						Message = "Failed to assign the role.",
						UserName = user.User.UserName,
					});
				}



				if (user.ClaimsToInsert != null && user.ClaimsToInsert.Any())
				{
					await this._databaseContext.UserClaims.AddRangeAsync(user.ClaimsToInsert);
					saveDbChanges = true;
				}

				var setPasswordResult = await this._userManager.AddPasswordAsync(user.User, user.Password);
				if (!setPasswordResult.Succeeded)
				{
					saveResults.Add(new SaveUserImportResult
					{
						HasErrors = true,
						Id = user.User.Id,
						Message = "Failed to set password.",
						UserName = user.User.UserName,
					});
				}
			}

			if (saveDbChanges)
			{
				await this._databaseContext.SaveChangesAsync(cancellationToken);
			}

			if (saveResults.Any(r => r.HasErrors))
			{
				return new ProcessResponse<IEnumerable<SaveUserImportResult>>()
				{
					Data = saveResults,
					HasError = true,
					IsSuccess = false,
					Message = "Users were partially imported."
				};
			}
			else
			{
				return new ProcessResponse<IEnumerable<SaveUserImportResult>>()
				{
					Data = saveResults,
					HasError = false,
					IsSuccess = true,
					Message = "Users imported."
				};
			}
		}
	}
}