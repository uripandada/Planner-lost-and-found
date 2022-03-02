using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Common.Shared;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Authentication.Queries.Login
{
	public class LoginQuery : IRequest<ProcessResponse<LoginModel>>
	{
		public string HotelGroup { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool RememberMe { get; set; }
	}

	public class LoginModel
	{
		public CookieOptions CookieOptions { get;set;}
		public string HotelGroupId { get; set; }
	}

	public class LoginQueryHandler : IRequestHandler<LoginQuery, ProcessResponse<LoginModel>>, IAmWebApplicationHandler
	{
		private readonly IHotelGroupTenantProvider groupTenantProvider;
		private readonly UserManager<User> userManager;
		private readonly SignInManager<User> signInManager;

		public LoginQueryHandler(IHotelGroupTenantProvider groupTenantProvider, UserManager<User> userManager, SignInManager<User> signInManager)
		{
			this.groupTenantProvider = groupTenantProvider;
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<ProcessResponse<LoginModel>> Handle(LoginQuery request, CancellationToken cancellationToken)
		{
			if(request.HotelGroup.IsNull() || request.Username.IsNull() || request.Password.IsNull())
			{
				return new ProcessResponse<LoginModel>
				{
					IsSuccess = false,
					HasError = true,
					Message = "Invalid login request."
				};
			}

			if (!this.groupTenantProvider.CheckIfTenantKeyExists(request.HotelGroup))
			{
				return new ProcessResponse<LoginModel>
				{
					IsSuccess = false,
					HasError = true,
					Message = "Hotel Group not found."
				};
			}

			var usernameValue = request.Username.Trim();
			var user = await this.userManager.FindByEmailAsync(request.Username);

			if (user == null)
			{
				user = await this.userManager.FindByNameAsync(request.Username);
			}

			if (user != null)
			{
				var result = await this.signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
				if (result.Succeeded)
				{
					var userClaims = await this.userManager.GetClaimsAsync(user);
					var hotelIdClaim = userClaims.Where(x => x.Type == "hotel_group_id").FirstOrDefault();
					
					CookieOptions option = new CookieOptions();
					option.Expires = DateTime.Now.AddDays(60);
					option.IsEssential = true;
					//Response.Cookies.Append("hotel_group_id", hotelIdClaim.Value, option);

					return new ProcessResponse<LoginModel>
					{
						Data = new LoginModel
						{
							CookieOptions = option,
							HotelGroupId = hotelIdClaim.Value
						},
						IsSuccess = true,
						HasError = false,
					};
				}
				else
				{
					if (result.IsLockedOut)
					{
						return new ProcessResponse<LoginModel>
						{
							IsSuccess = false,
							HasError = true,
							Message = "Locked out."
						};
					}
					else if (result.IsNotAllowed)
					{
						return new ProcessResponse<LoginModel>
						{
							IsSuccess = false,
							HasError = true,
							Message = "Not allowed to login."
						};
					}
					else if (result.RequiresTwoFactor)
					{
						return new ProcessResponse<LoginModel>
						{
							IsSuccess = false,
							HasError = true,
							Message = "Requires two factor auth."
						};
					}
					else
					{
						return new ProcessResponse<LoginModel>
						{
							IsSuccess = false,
							HasError = true,
							Message = "Login failed."
						};
					}
				}
			}
			else
			{
				return new ProcessResponse<LoginModel>
				{
					IsSuccess = false,
					HasError = true,
					Message = "Login failed, unknown user."
				};
			}
		}
	}
}
