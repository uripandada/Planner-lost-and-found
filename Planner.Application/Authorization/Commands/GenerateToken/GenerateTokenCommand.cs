using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Authorization.Commands.GenerateToken
{
	public enum GenerateTokenResponseStatus
	{
		FORBIDDEN,
		SUCCESS,
	}

	public class GenerateTokenResponse
	{
		public AuthenticationProperties AuthenticationProperties { get; set; }
		public string AuthenticationScheme { get; set; }
		public ClaimsPrincipal Principal { get; set; }


		public GenerateTokenResponseStatus Status { get; set; }
	}

	public class GenerateTokenCommand : IRequest<GenerateTokenResponse>
	{
	}

	public class GenerateTokenCommandHandler : IRequestHandler<GenerateTokenCommand, GenerateTokenResponse>, IAmWebApplicationHandler
	{
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GenerateTokenCommandHandler(SignInManager<User> signInManager, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
		{
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<GenerateTokenResponse> Handle(GenerateTokenCommand command, CancellationToken cancellationToken)
		{
			var request = this._httpContextAccessor.HttpContext.GetOpenIddictServerRequest();

			var response = new GenerateTokenResponse();

			if (request.IsPasswordGrantType())
			{
				try
				{
					var user = await _userManager.FindByNameAsync(request.Username);
					if (user == null)
					{
						var properties = new AuthenticationProperties(new Dictionary<string, string>
						{
							[OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
							[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "[AUTH-001] Invalid login credentials."
						});

						return new GenerateTokenResponse
						{
							AuthenticationProperties = properties,
							AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
							Status = GenerateTokenResponseStatus.FORBIDDEN,
						};
					}

					// Validate the username/password parameters and ensure the account is not locked out.
					var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
					
					if (result.Succeeded)
					{
						// Create a new ClaimsPrincipal containing the claims that
						// will be used to create an id_token, a token or a code.
						var principal = await _signInManager.CreateUserPrincipalAsync(user);

						// Set the list of scopes granted to the client application.
						// Note: the offline_access scope must be granted
						// to allow OpenIddict to return a refresh token.
						principal.SetScopes(new[]
						{
							OpenIddictConstants.Scopes.OpenId,
							OpenIddictConstants.Scopes.Email,
							OpenIddictConstants.Scopes.Profile,
							OpenIddictConstants.Scopes.OfflineAccess,
							OpenIddictConstants.Scopes.Roles
						}.Intersect(request.GetScopes()));

						foreach (var claim in principal.Claims)
						{
							claim.SetDestinations(GetDestinations(claim, principal));
						}

						return new GenerateTokenResponse
						{
							Principal = principal,
							AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
							Status = GenerateTokenResponseStatus.SUCCESS,
						};
					}

					var errorMessages = new Dictionary<string, string>();
					if (result.IsLockedOut)
					{
						errorMessages.Add(OpenIddictServerAspNetCoreConstants.Properties.Error, OpenIddictConstants.Errors.InvalidGrant);
						errorMessages.Add(OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription, "[AUTH-002] Locked out.");
					}
					else if (result.IsNotAllowed)
					{
						errorMessages.Add(OpenIddictServerAspNetCoreConstants.Properties.Error, OpenIddictConstants.Errors.InvalidGrant);
						errorMessages.Add(OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription, "[AUTH-003] Not allowed.");
					}
					else if (result.RequiresTwoFactor)
					{
						errorMessages.Add(OpenIddictServerAspNetCoreConstants.Properties.Error, OpenIddictConstants.Errors.InvalidGrant);
						errorMessages.Add(OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription, "[AUTH-004] Requirs two factor auth.");
					}
					else
					{
						errorMessages.Add(OpenIddictServerAspNetCoreConstants.Properties.Error, OpenIddictConstants.Errors.InvalidGrant);
						errorMessages.Add(OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription, "[AUTH-005] Invalid login credentials.");
					}

					return new GenerateTokenResponse
					{
						AuthenticationProperties = new AuthenticationProperties(errorMessages),
						AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
						Status = GenerateTokenResponseStatus.FORBIDDEN,
					};
				}
				catch(Exception ex)
				{
					var properties = new AuthenticationProperties(new Dictionary<string, string>
					{
						[OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
						[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "[AUTH-006] Invalid login credentials."
					});

					return new GenerateTokenResponse
					{
						AuthenticationProperties = properties,
						AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
						Status = GenerateTokenResponseStatus.FORBIDDEN,
					};
				}
			}
			else if (request.IsRefreshTokenGrantType())
			{
				// Retrieve the claims principal stored in the refresh token.
				var info = await this._httpContextAccessor.HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

				// Retrieve the user profile corresponding to the refresh token.
				// Note: if you want to automatically invalidate the refresh token
				// when the user password/roles change, use the following line instead:
				// var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
				var user = await _userManager.GetUserAsync(info.Principal);
				if (user == null)
				{
					var properties = new AuthenticationProperties(new Dictionary<string, string>
					{
						[OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
						[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The refresh token is no longer valid."
					});

					return new GenerateTokenResponse
					{
						AuthenticationProperties = properties,
						AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
						Status = GenerateTokenResponseStatus.FORBIDDEN,
					};

					//return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
				}

				// Ensure the user is still allowed to sign in.
				if (!await _signInManager.CanSignInAsync(user))
				{
					var properties = new AuthenticationProperties(new Dictionary<string, string>
					{
						[OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
						[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
					});

					return new GenerateTokenResponse
					{
						AuthenticationProperties = properties,
						AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
						Status = GenerateTokenResponseStatus.FORBIDDEN,
					};

					//return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
				}

				// Create a new ClaimsPrincipal containing the claims that
				// will be used to create an id_token, a token or a code.
				var principal = await _signInManager.CreateUserPrincipalAsync(user);

				foreach (var claim in principal.Claims)
				{
					claim.SetDestinations(GetDestinations(claim, principal));
				}

				return new GenerateTokenResponse
				{
					Principal = principal,
					AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
					Status = GenerateTokenResponseStatus.SUCCESS,
				};

				//return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
			}
			else if (request.IsClientCredentialsGrantType())
			{
				// Note: the client credentials are automatically validated by OpenIddict:
				// if client_id or client_secret are invalid, this action won't be invoked.

				var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

				// Subject (sub) is a required field, we use the client id as the subject identifier here.
				identity.AddClaim(OpenIddictConstants.Claims.Subject, request.ClientId ?? throw new InvalidOperationException());

				// Add some claim, don't forget to add destination otherwise it won't be added to the access token.
				identity.AddClaim("some-claim", "some-value", OpenIddictConstants.Destinations.AccessToken);

				var claimsPrincipal = new ClaimsPrincipal(identity);

				// We also grant all the requested scopes by calling claimsPrincipal.SetScopes(request.GetScopes());.
				// OpenIddict has already checked if the requested scopes are allowed (in general and for the current client).
				// The reason why we have to add the scopes manually here is that we are able to filter the scopes granted here if we want to.
				claimsPrincipal.SetScopes(request.GetScopes());

				return new GenerateTokenResponse
				{
					Principal = claimsPrincipal,
					AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
					Status = GenerateTokenResponseStatus.SUCCESS,
				};
			}
			else if (request.IsAuthorizationCodeGrantType())
			{
				// Retrieve the claims principal stored in the authorization code
				var info = await this._httpContextAccessor.HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
				var claimsPrincipal = info.Principal;

				// Retrieve the user profile corresponding to the refresh token.
				// Note: if you want to automatically invalidate the refresh token
				// when the user password/roles change, use the following line instead:
				// var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
				var user = await _userManager.GetUserAsync(info.Principal);

				if (user == null)
				{
					var properties = new AuthenticationProperties(new Dictionary<string, string>
					{
						[OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
						[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The authorization code is no longer valid."
					});

					return new GenerateTokenResponse
					{
						AuthenticationProperties = properties,
						AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
						Status = GenerateTokenResponseStatus.FORBIDDEN,
					};

					//return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
				}

				// Ensure the user is still allowed to sign in.
				if (!await _signInManager.CanSignInAsync(user))
				{
					var properties = new AuthenticationProperties(new Dictionary<string, string>
					{
						[OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
						[OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
					});

					return new GenerateTokenResponse
					{
						AuthenticationProperties = properties,
						AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
						Status = GenerateTokenResponseStatus.FORBIDDEN,
					};

					//return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
				}

				// Create a new ClaimsPrincipal containing the claims that
				// will be used to create an id_token, a token or a code.
				var principal = await _signInManager.CreateUserPrincipalAsync(user);

				foreach (var claim in principal.Claims)
				{
					claim.SetDestinations(GetDestinations(claim, principal));
				}

				return new GenerateTokenResponse
				{
					Principal = principal,
					AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
					Status = GenerateTokenResponseStatus.SUCCESS,
				};

			}
			//else if (request.IsRefreshTokenGrantType())
			//{
			//    // Retrieve the claims principal stored in the refresh token.
			//    var claimsPrincipal  = (await this._httpContextAccessor.HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
			//}

			throw new NotImplementedException("The specified grant type is not implemented.");
		}

		private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
		{
			// Note: by default, claims are NOT automatically included in the access and identity tokens.
			// To allow OpenIddict to serialize them, you must attach them a destination, that specifies
			// whether they should be included in access tokens, in identity tokens or in both.

			switch (claim.Type)
			{
				case OpenIddictConstants.Claims.Name:
					yield return OpenIddictConstants.Destinations.AccessToken;

					if (principal.HasScope(OpenIddictConstants.Scopes.Profile))
						yield return OpenIddictConstants.Destinations.IdentityToken;

					yield break;

				case OpenIddictConstants.Claims.Email:
					yield return OpenIddictConstants.Destinations.AccessToken;

					if (principal.HasScope(OpenIddictConstants.Scopes.Email))
						yield return OpenIddictConstants.Destinations.IdentityToken;

					yield break;

				case OpenIddictConstants.Claims.Role:
					yield return OpenIddictConstants.Destinations.AccessToken;

					if (principal.HasScope(OpenIddictConstants.Scopes.Roles))
						yield return OpenIddictConstants.Destinations.IdentityToken;

					yield break;

				// Never include the security stamp in the access and identity tokens, as it's a secret value.
				case "AspNet.Identity.SecurityStamp": yield break;

				default:
					yield return OpenIddictConstants.Destinations.AccessToken;
					yield break;
			}
		}
	}


}
