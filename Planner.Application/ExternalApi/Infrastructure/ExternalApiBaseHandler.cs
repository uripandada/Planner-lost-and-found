using Microsoft.AspNetCore.Http;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.ExternalApi.Infrastructure
{
    public class ExternalApiBaseHandler
    {
        protected Guid _hotelGroupId;
        protected string _clientId;
        protected bool _canAccessListOfHotelGroups;
        protected bool _canAccessListOfHotels;
        protected IMasterDatabaseContext _masterDatabaseContext;
        protected IDatabaseContext _databaseContext;
        protected IHttpContextAccessor _contextAccessor;

        protected readonly ExternalClientAuthenticator ExternalClientAuthenticator = new ExternalClientAuthenticator();

        protected async Task<ProcessResponseSimple> AuthorizeExternalClient()
        {
            var authResult = await this.ExternalClientAuthenticator.AuthenticateExternalClient(this._masterDatabaseContext, this._contextAccessor.HttpContext.Request.Headers);

            if (authResult.IsSuccess)
            {
                this._clientId = authResult.ClientId;
                this._canAccessListOfHotelGroups = authResult.HasAccessToListOfHotelGroups;
                this._canAccessListOfHotels = authResult.HasAccessToListOfHotels;
            }
            else
            {
                this._clientId = null;
                this._canAccessListOfHotelGroups = false;
                this._canAccessListOfHotels = false;
            }

            return new ProcessResponseSimple
            {
                IsSuccess = authResult.IsSuccess,
                Message = authResult.Message,
            };
        }

        protected ProcessResponseSimple InitializeHotelGroupContext(Guid? hotelGroupId, string hotelGroupKey)
        {
            if (hotelGroupId.HasValue && this._databaseContext.DoesHotelGroupExist(hotelGroupId.Value))
            {
                this._databaseContext.SetTenantId(hotelGroupId.Value);
                this._hotelGroupId = this._databaseContext.HotelGroupTenant.Id;

                return new ProcessResponseSimple
                {
                    IsSuccess = true,
                    Message = "Tenant set.",
                };
            }
            else if (hotelGroupKey.IsNotNull() && this._databaseContext.DoesHotelGroupExist(hotelGroupKey))
            {
                this._databaseContext.SetTenantKey(hotelGroupKey);
                this._hotelGroupId = this._databaseContext.HotelGroupTenant.Id;

                return new ProcessResponseSimple
                {
                    IsSuccess = true,
                    Message = "Tenant set.",
                };
            }
            else
            {
                return new ProcessResponseSimple
                {
                    IsSuccess = false,
                    Message = $"Hotel group doesn't exist.",
                };
            }
        }
    }
}
