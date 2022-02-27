using MediatR;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Authorization.Commands.GenerateAuthorizationCode
{
	public enum GenerateAuthorizationCodeStatus
    {
        CHALLENGE,
        SUCCESS,
	}

    public class GenerateAuthorizationCodeResponse
    {
        public AuthenticationProperties AuthenticationProperties { get; set; }
        public string AuthenticationScheme { get; set; }
        public ClaimsPrincipal Principal { get; set; }
        public GenerateAuthorizationCodeStatus Status { get; set; }

    }

    public class GenerateAuthorizationCodeCommand : IRequest<GenerateAuthorizationCodeResponse>
    {
    }

	public class GenerateAuthorizationCodeCommandHandler : IRequestHandler<GenerateAuthorizationCodeCommand, GenerateAuthorizationCodeResponse>, IAmWebApplicationHandler
	{
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GenerateAuthorizationCodeCommandHandler(SignInManager<User> signInManager, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
		{
			this._signInManager = signInManager;
			this._userManager = userManager;
			this._httpContextAccessor = httpContextAccessor;
		}

		public async Task<GenerateAuthorizationCodeResponse> Handle(GenerateAuthorizationCodeCommand command, CancellationToken cancellationToken)
		{
            var request = this._httpContextAccessor.HttpContext.GetOpenIddictServerRequest() ??
                   throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Retrieve the user principal stored in the authentication cookie.
            //var result = await this._httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // If the user principal can't be extracted, redirect the user to the login page.
            if (this._httpContextAccessor.HttpContext.User == null || this._httpContextAccessor.HttpContext.User.Identity == null || !this._httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return new GenerateAuthorizationCodeResponse
                {
                    Status = GenerateAuthorizationCodeStatus.CHALLENGE,
                    AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                    AuthenticationProperties = new AuthenticationProperties
                    {
                        RedirectUri = this._httpContextAccessor.HttpContext.Request.PathBase + this._httpContextAccessor.HttpContext.Request.Path + QueryString.Create(
                            this._httpContextAccessor.HttpContext.Request.HasFormContentType ? this._httpContextAccessor.HttpContext.Request.Form.ToList() : this._httpContextAccessor.HttpContext.Request.Query.ToList())
                    }
                };
            }

            // Create a new claims principal
            //var claims = new List<Claim>
            //{
            //    // 'subject' claim which is required
            //    new Claim(OpenIddictConstants.Claims.Subject, result.Principal.Identity.Name),
            //    new Claim("some claim", "some value 2").SetDestinations(OpenIddictConstants.Destinations.AccessToken)
            //};

            //var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            //var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Set requested scopes (this is not done automatically)
            var claimsPrincipal = this._httpContextAccessor.HttpContext.User;
            claimsPrincipal.SetScopes(request.GetScopes());

            //claimsPrincipal.SetClaim(OpenIddictConstants.Claims.Subject, claimsPrincipal.Identity.Name);
            //claimsPrincipal.SetClaim("TEST-CLAIM", "Test claim value");

            return new GenerateAuthorizationCodeResponse
            {
                Principal = claimsPrincipal,
                AuthenticationScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                Status = GenerateAuthorizationCodeStatus.SUCCESS,
            };
        }
    }
}
