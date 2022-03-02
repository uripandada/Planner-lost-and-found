using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Planner.Application.Authorization.Commands.GenerateAuthorizationCode;
using Planner.Application.Authorization.Commands.GenerateToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Planner.WebUi.Controllers
{
	[ApiController]
	public class AuthorizationController : Controller
	{
		private IMediator _mediator;
		protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());

		private readonly SignInManager<Domain.Entities.User> _signInManager;
		private readonly UserManager<Domain.Entities.User> _userManager;

		public AuthorizationController(UserManager<Domain.Entities.User> userManager, SignInManager<Domain.Entities.User> signInManager)
		{ 
			this._userManager = userManager;
			this._signInManager = signInManager;
		}

		[AllowAnonymous]
		[HttpPost("~/connect/token"), Produces("application/json")]
		public async Task<IActionResult> Exchange()
		{
			var generateTokenCommand = new GenerateTokenCommand();
			var generateTokenResult = await this.Mediator.Send(generateTokenCommand);

			if(generateTokenResult.Status == GenerateTokenResponseStatus.SUCCESS)
			{
				// Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
				return SignIn(generateTokenResult.Principal, generateTokenResult.AuthenticationScheme);
			}
			else if(generateTokenResult.Status == GenerateTokenResponseStatus.FORBIDDEN)
			{
				return Forbid(generateTokenResult.AuthenticationProperties, generateTokenResult.AuthenticationScheme);
			}
			else
			{
				throw new Exception("Generating token failed.");
			}
		}

		[HttpGet("~/connect/authorize")]
		[HttpPost("~/connect/authorize")]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> Authorize()
		{
			var generateAuthorizationCodeCommand = new GenerateAuthorizationCodeCommand();
			var generateTokenResult = await this.Mediator.Send(generateAuthorizationCodeCommand);

			if (generateTokenResult.Status == GenerateAuthorizationCodeStatus.SUCCESS)
			{
				// Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
				return SignIn(generateTokenResult.Principal, generateTokenResult.AuthenticationScheme);
			}
			else if (generateTokenResult.Status == GenerateAuthorizationCodeStatus.CHALLENGE)
			{
				// If the user principal can't be extracted, redirect the user to the login page.
				return Challenge(generateTokenResult.AuthenticationProperties, generateTokenResult.AuthenticationScheme);
			}
			else
			{
				throw new Exception("Generating token failed.");
			}
		}

		[Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
		[HttpGet("~/connect/userinfo")]
		public async Task<IActionResult> Userinfo()
		{
			var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
			var user = await _userManager.GetUserAsync(User);
			if (user is null)
			{
				return Challenge(
					authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
					properties: new AuthenticationProperties(new Dictionary<string, string>
					{
						[OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
						[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
							"The specified access token is bound to an account that no longer exists."
					}));
			}

			var claims = new Dictionary<string, object>(StringComparer.Ordinal)
			{
				// Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
				[Claims.Subject] = await _userManager.GetUserIdAsync(user),
				[Claims.Name] = $"{user.FirstName} {user.LastName}",
			};

			if (User.HasScope(Scopes.Email))
			{
				claims[Claims.Email] = await _userManager.GetEmailAsync(user);
				claims[Claims.EmailVerified] = await _userManager.IsEmailConfirmedAsync(user);
			}

			if (User.HasScope(Scopes.Phone))
			{
				claims[Claims.PhoneNumber] = await _userManager.GetPhoneNumberAsync(user);
				claims[Claims.PhoneNumberVerified] = await _userManager.IsPhoneNumberConfirmedAsync(user);
			}

			if (User.HasScope(Scopes.Roles))
			{
				claims[Claims.Role] = await _userManager.GetRolesAsync(user);
			}

			if (User.Claims.Any(c => c.Type == "rooms"))
			{
				claims["rooms"] = "rooms";
			}
			if (User.Claims.Any(c => c.Type == "assets"))
			{
				claims["assets"] = "assets";
			}
			if (User.Claims.Any(c => c.Type == "users"))
			{
				claims["users"] = "users";
			}
			if (User.Claims.Any(c => c.Type == "role_management"))
			{
				claims["role_management"] = "role_management";
			}
			if (User.Claims.Any(c => c.Type == "room_categories"))
			{
				claims["room_categories"] = "room_categories";
			}
			if (User.Claims.Any(c => c.Type == "categories"))
			{
				claims["categories"] = "categories";
			}
			if (User.Claims.Any(c => c.Type == "hotel_settings"))
			{
				claims["hotel_settings"] = "hotel_settings";
			}
			if (User.Claims.Any(c => c.Type == "room_insights"))
			{
				claims["room_insights"] = "room_insights";
			}
			if (User.Claims.Any(c => c.Type == "user_insights"))
			{
				claims["user_insights"] = "user_insights";
			}
			if (User.Claims.Any(c => c.Type == "tasks"))
			{
				claims["tasks"] = "tasks";
			}
			if (User.Claims.Any(c => c.Type == "reservations"))
			{
				claims["reservations"] = "reservations";
			}
			if (User.Claims.Any(c => c.Type == "cleaning_planner"))
			{
				claims["cleaning_planner"] = "cleaning_planner";
			}
			if (User.Claims.Any(c => c.Type == "cleaning_calendar"))
			{
				claims["cleaning_calendar"] = "cleaning_calendar";
			}
			if (User.Claims.Any(c => c.Type == "reservation_calendar"))
			{
				claims["reservation_calendar"] = "reservation_calendar";
			}
			if (User.Claims.Any(c => c.Type == "lost_and_found"))
			{
				claims["lost_and_found"] = "lost_and_found";
			}
			if (User.Claims.Any(c => c.Type == "on_guard"))
			{
				claims["on_guard"] = "on_guard";
			}

			//claims[Domain.Values.ClaimsKeys.SettingsClaimKeys.Assets] = Domain.Values.ClaimsKeys.SettingsClaimKeys.Assets;
			//claims[Domain.Values.ClaimsKeys.SettingsClaimKeys.HotelSettings] = Domain.Values.ClaimsKeys.SettingsClaimKeys.HotelSettings;
			//claims[Domain.Values.ClaimsKeys.SettingsClaimKeys.RoleManagement] = Domain.Values.ClaimsKeys.SettingsClaimKeys.RoleManagement;
			//claims[Domain.Values.ClaimsKeys.SettingsClaimKeys.RoomCategories] = Domain.Values.ClaimsKeys.SettingsClaimKeys.RoomCategories;
			//claims[Domain.Values.ClaimsKeys.SettingsClaimKeys.Rooms] = Domain.Values.ClaimsKeys.SettingsClaimKeys.Rooms;
			//claims[Domain.Values.ClaimsKeys.SettingsClaimKeys.Users] = Domain.Values.ClaimsKeys.SettingsClaimKeys.Users;
			//claims[Domain.Values.ClaimsKeys.ManagementClaimKeys.CleaningCalendar] = Domain.Values.ClaimsKeys.ManagementClaimKeys.CleaningCalendar;
			//claims[Domain.Values.ClaimsKeys.ManagementClaimKeys.CleaningPlanner] = Domain.Values.ClaimsKeys.ManagementClaimKeys.CleaningPlanner;
			//claims[Domain.Values.ClaimsKeys.ManagementClaimKeys.LostAndFound] = Domain.Values.ClaimsKeys.ManagementClaimKeys.LostAndFound;
			//claims[Domain.Values.ClaimsKeys.ManagementClaimKeys.OnGuard] = Domain.Values.ClaimsKeys.ManagementClaimKeys.OnGuard;
			//claims[Domain.Values.ClaimsKeys.ManagementClaimKeys.ReservationCalendar] = Domain.Values.ClaimsKeys.ManagementClaimKeys.ReservationCalendar;
			//claims[Domain.Values.ClaimsKeys.ManagementClaimKeys.Reservations] = Domain.Values.ClaimsKeys.ManagementClaimKeys.Reservations;
			//claims[Domain.Values.ClaimsKeys.ManagementClaimKeys.RoomInsights] = Domain.Values.ClaimsKeys.ManagementClaimKeys.RoomInsights;
			//claims[Domain.Values.ClaimsKeys.ManagementClaimKeys.Tasks] = Domain.Values.ClaimsKeys.ManagementClaimKeys.Tasks;
			//claims[Domain.Values.ClaimsKeys.ManagementClaimKeys.UserInsights] = Domain.Values.ClaimsKeys.ManagementClaimKeys.UserInsights;

			// Note: the complete list of standard claims supported by the OpenID Connect specification
			// can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

			return Ok(claims);
		}

		[HttpGet("~/connect/logout")]
		[HttpPost("~/connect/logout")]
		public async Task<IActionResult> Logout()
		{
			// Ask ASP.NET Core Identity to delete the local and external cookies created
			// when the user agent is redirected from the external identity provider
			// after a successful authentication flow (e.g Google or Facebook).
			await _signInManager.SignOutAsync();

			// Returning a SignOutResult will ask OpenIddict to redirect the user agent
			// to the post_logout_redirect_uri specified by the client application or to
			// the RedirectUri specified in the authentication properties if none was set.
			return SignOut(
				authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
				properties: new AuthenticationProperties
				{
					RedirectUri = "/"
				});
		}
	}
}
