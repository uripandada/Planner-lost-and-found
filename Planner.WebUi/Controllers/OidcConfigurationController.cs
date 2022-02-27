using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class OidcConfigurationController : Controller
	{
		private readonly ILogger<OidcConfigurationController> logger;
        private readonly IOpenIddictApplicationManager _openIddictApplicationManager;

        public OidcConfigurationController(IOpenIddictApplicationManager openIddictApplicationManager, ILogger<OidcConfigurationController> _logger)
        {
            this._openIddictApplicationManager = openIddictApplicationManager;
            ClientRequestParametersProvider = new DefaultClientRequestParametersProvider(this._openIddictApplicationManager);
			logger = _logger;
		}

		private DefaultClientRequestParametersProvider ClientRequestParametersProvider { get; }

		[HttpGet("_configuration/{clientId}")]
		public async Task<IActionResult> GetClientRequestParameters([FromRoute] string clientId)
		{
            var parameters = await this.ClientRequestParametersProvider.GetClientParameters(this.HttpContext, clientId);
			return Ok(parameters);
		}
	}

    internal class DefaultClientRequestParametersProvider
    {
        private readonly IOpenIddictApplicationManager _openIddictApplicationManager;
        public DefaultClientRequestParametersProvider(
            IOpenIddictApplicationManager openIddictApplicationManager)
        {
            this._openIddictApplicationManager = openIddictApplicationManager;
        }

        public async Task<IDictionary<string, string>> GetClientParameters(HttpContext context, string clientId)
        {
            OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication client = (await this._openIddictApplicationManager.FindByClientIdAsync(clientId)) as OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication;

            var authority = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.PathBase.Value}";

            var redirectUris = JsonConvert.DeserializeObject<string[]>(client.RedirectUris);
            var permissions = JsonConvert.DeserializeObject<string[]>(client.Permissions);
            var postLogoutRedirectUris = new string[0];

			if (client.PostLogoutRedirectUris.IsNotNull())
			{
                postLogoutRedirectUris = JsonConvert.DeserializeObject<string[]>(client.PostLogoutRedirectUris);
			}
            
            var scopes = new List<string>();
            var responseTypes = new List<string>();
            foreach(var permission in permissions)
			{
                var parts = permission.Split(":", StringSplitOptions.RemoveEmptyEntries);
                if(parts.Length >= 2)
				{
					if (parts[0] == "scp")
					{
                        scopes.Add(parts[1]);
					}
                    else if (parts[0] == "rst")
                    {
                        responseTypes.Add(parts[1]);
                    }
				}
			}

			if (!responseTypes.Any())
			{
                responseTypes.Add("code");
			}

            var redirectUri = (string) null;
            if(redirectUris.Any())
			{
                redirectUri = redirectUris.FirstOrDefault(ru => ru.StartsWith(authority));

				if (redirectUri.IsNull())
				{
                    redirectUri = redirectUris.First();
				}
			}

            var postLogoutRedirectUri = (string)null;
            if (postLogoutRedirectUris.Any())
            {
                postLogoutRedirectUri = postLogoutRedirectUris.FirstOrDefault(ru => ru.StartsWith(authority));

                if (postLogoutRedirectUri.IsNull())
                {
                    postLogoutRedirectUri = postLogoutRedirectUris.First();
                }
            }

            scopes.Add("openid");
            scopes.Add("hotelgroupid");
            scopes.Add("hotelid");

            scopes = scopes.Distinct().ToList();
            
            var options = new Dictionary<string, string>
            {
                ["authority"] = authority,
                ["client_id"] = clientId,
                ["redirect_uri"] = redirectUri,
                ["post_logout_redirect_uri"] = postLogoutRedirectUri,
                ["response_type"] = string.Join(" ", responseTypes),
                ["scope"] = string.Join(" ", scopes)
            };

            return options;
        }
	}

}
