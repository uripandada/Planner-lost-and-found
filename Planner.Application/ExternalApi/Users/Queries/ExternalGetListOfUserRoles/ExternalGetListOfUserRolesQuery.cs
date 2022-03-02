using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.ExternalApi.Infrastructure;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExternalApi.Users.Queries.ExternalGetListOfUserRoles
{
    public class ExternalUserRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string HotelAccessTypeKey { get; set; }
    }

    public class ExternalGetListOfUserRolesQuery : IRequest<ProcessResponseSimple<IEnumerable<ExternalUserRole>>>
    {
        /// <summary>
        /// You can choose to set either hotelGroupId or hotelGroupKey
        /// </summary>
        public Guid? HotelGroupId { get; set; }
        /// <summary>
        /// You can choose to set either hotelGroupId or hotelGroupKey
        /// </summary>
        public string HotelGroupKey { get; set; }
    }

    public class ExternalGetListOfUserRolesQueryHandler : ExternalApiBaseHandler, IRequestHandler<ExternalGetListOfUserRolesQuery, ProcessResponseSimple<IEnumerable<ExternalUserRole>>>, IAmWebApplicationHandler
    {
        public ExternalGetListOfUserRolesQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
        {
            this._masterDatabaseContext = masterDatabaseContext;
            this._databaseContext = databaseContext;
            this._contextAccessor = contextAccessor;
        }

        public async Task<ProcessResponseSimple<IEnumerable<ExternalUserRole>>> Handle(ExternalGetListOfUserRolesQuery request, CancellationToken cancellationToken)
        {
            var initResult = await this._Initialize(request.HotelGroupId, request.HotelGroupKey);
            if (initResult != null)
            {
                return initResult;
            }

            var roles = await this._databaseContext
                .Roles
                .Select(r => new ExternalUserRole 
                {
                    Id = r.Id,
                    Name = r.Name,
                    HotelAccessTypeKey = r.HotelAccessTypeKey,
                })
                .ToListAsync();

            return new ProcessResponseSimple<IEnumerable<ExternalUserRole>>
            {
                Data = roles,
                IsSuccess = true,
                Message = "Users roles loaded.",
            };
        }

        private async Task<ProcessResponseSimple<IEnumerable<ExternalUserRole>>> _Initialize(Guid? hotelGroupId, string hotelGroupKey)
        {
            var authResult = await this.AuthorizeExternalClient();
            if (!authResult.IsSuccess)
                return new ProcessResponseSimple<IEnumerable<ExternalUserRole>> { IsSuccess = false, Message = authResult.Message, Data = new ExternalUserRole[0] };

            var initResult = this.InitializeHotelGroupContext(hotelGroupId, hotelGroupKey);
            if (!initResult.IsSuccess)
                return new ProcessResponseSimple<IEnumerable<ExternalUserRole>> { IsSuccess = false, Message = initResult.Message, Data = new ExternalUserRole[0] };

            return null;
        }
    }
}
