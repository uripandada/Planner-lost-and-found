using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.Infrastructure
{

	public class ExternalClientAuthenticator
	{
		public class ExternalClientAuthenticationResult: ProcessResponseSimple
		{
			public string ClientId { get; set; }
			public bool HasAccessToListOfHotelGroups { get; set; }
			public bool HasAccessToListOfHotels { get; set; }
		}

		public async Task<ExternalClientAuthenticationResult> AuthenticateExternalClient(IMasterDatabaseContext masterDatabaseContext, IHeaderDictionary requestHeaders)
		{
			if (!requestHeaders.ContainsKey("ClientId"))
			{
				return new ExternalClientAuthenticationResult
				{
					ClientId = null,
					IsSuccess = false,
					Message = $"Unauthorized. Id missing.",
					HasAccessToListOfHotelGroups = false,
					HasAccessToListOfHotels = false,
				};
			}
			if (!requestHeaders.ContainsKey("ClientKey"))
			{
				return new ExternalClientAuthenticationResult
				{
					ClientId = null,
					IsSuccess = false,
					Message = $"Unauthorized. Key missing.",
					HasAccessToListOfHotelGroups = false,
					HasAccessToListOfHotels = false,
				};
			}

			var clientId = requestHeaders["ClientId"].FirstOrDefault();
			var clientKey = requestHeaders["ClientKey"].FirstOrDefault();
			if (clientId.IsNull())
			{
				return new ExternalClientAuthenticationResult
				{
					ClientId = null,
					IsSuccess = false,
					Message = $"Unauthorized. Id missing.",
					HasAccessToListOfHotelGroups = false,
					HasAccessToListOfHotels = false,
				};
			}
			if (clientKey.IsNull())
			{
				return new ExternalClientAuthenticationResult
				{
					ClientId = null,
					IsSuccess = false,
					Message = $"Unauthorized. Key missing.",
					HasAccessToListOfHotelGroups = false,
					HasAccessToListOfHotels = false,
				};
			}

			var client = await masterDatabaseContext.ExternalClientSecretKeys.FirstOrDefaultAsync(c => c.ClientId == clientId && c.Key == clientKey && c.IsActive);

			if (client == null)
			{
				return new ExternalClientAuthenticationResult
				{
					ClientId = null,
					IsSuccess = false,
					Message = $"Unauthorized.",
					HasAccessToListOfHotelGroups = false,
					HasAccessToListOfHotels = false,
				};
			}

			return new ExternalClientAuthenticationResult
			{
				ClientId = null,
				IsSuccess = true,
				Message = $"Authorized.",
				HasAccessToListOfHotelGroups = client.HasAccessToListOfHotelGroups,
				HasAccessToListOfHotels = client.HasAccessToListOfHotels,
			};
		}
	}

}
