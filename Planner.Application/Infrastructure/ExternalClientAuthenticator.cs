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
		public async Task<ProcessResponse> AuthenticateUser(IMasterDatabaseContext masterDatabaseContext, IHeaderDictionary requestHeaders)
		{
			if (!requestHeaders.ContainsKey("ClientId"))
			{
				return new ProcessResponse
				{
					IsSuccess = false,
					HasError = true,
					Message = $"Unauthorized. Id missing.",
				};
			}
			if (!requestHeaders.ContainsKey("ClientKey"))
			{
				return new ProcessResponse
				{
					IsSuccess = false,
					HasError = true,
					Message = $"Unauthorized. Key missing.",
				};
			}

			var clientId = requestHeaders["ClientId"].FirstOrDefault();
			var clientKey = requestHeaders["ClientKey"].FirstOrDefault();
			if (clientId.IsNull())
			{
				return new ProcessResponse<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					HasError = true,
					Message = $"Unauthorized. Id missing.",
				};
			}
			if (clientKey.IsNull())
			{
				return new ProcessResponse<Guid>
				{
					Data = Guid.Empty,
					IsSuccess = false,
					HasError = true,
					Message = $"Unauthorized. Key missing.",
				};
			}

			var client = await masterDatabaseContext.ExternalClientSecretKeys.FirstOrDefaultAsync(c => c.ClientId == clientId && c.Key == clientKey && c.IsActive);

			if (client == null)
			{
				return new ProcessResponse
				{
					IsSuccess = false,
					HasError = true,
					Message = $"Unauthorized.",
				};
			}

			return new ProcessResponse
			{
				IsSuccess = true,
				HasError = false,
				Message = $"Authorized.",
			};
		}
	}

}
