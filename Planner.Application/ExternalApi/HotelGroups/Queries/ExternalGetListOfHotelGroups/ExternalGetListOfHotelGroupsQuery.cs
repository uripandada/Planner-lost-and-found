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

namespace Planner.Application.ExternalApi.HotelGroups.Queries.ExternalGetListOfHotelGroups
{
    public class ExternalHotelGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class ExternalGetListOfHotelGroupsQuery: IRequest<ProcessResponseSimple<IEnumerable<ExternalHotelGroup>>>
    {

    }

    public class ExternalGetListOfHotelGroupsQueryHandler : ExternalApiBaseHandler, IRequestHandler<ExternalGetListOfHotelGroupsQuery, ProcessResponseSimple<IEnumerable<ExternalHotelGroup>>>, IAmWebApplicationHandler
    {
        public ExternalGetListOfHotelGroupsQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
        {
            this._masterDatabaseContext = masterDatabaseContext;
            this._databaseContext = databaseContext;
            this._contextAccessor = contextAccessor;
        }

        public async Task<ProcessResponseSimple<IEnumerable<ExternalHotelGroup>>> Handle(ExternalGetListOfHotelGroupsQuery request, CancellationToken cancellationToken)
        {
            var authResult = await this.AuthorizeExternalClient();
            if (!authResult.IsSuccess)
                return new ProcessResponseSimple<IEnumerable<ExternalHotelGroup>> { IsSuccess = false, Message = authResult.Message, Data = new ExternalHotelGroup[0] };

            if (!this._canAccessListOfHotelGroups)
                return new ProcessResponseSimple<IEnumerable<ExternalHotelGroup>> { IsSuccess = false, Message = "Access denied.", Data = new ExternalHotelGroup[0] };

            var hotelGroups = await this._masterDatabaseContext
                .HotelGroupTenants
                .Select(hg => new ExternalHotelGroup
                {
                    Id = hg.Id,
                    Key = hg.Key,
                    Name = hg.Name,
                })
                .ToListAsync();

            return new ProcessResponseSimple<IEnumerable<ExternalHotelGroup>>
            {
                Data = hotelGroups,
                IsSuccess = true,
                Message = "Hotel groups loaded."
            };
        }
    }
}
