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

namespace Planner.Application.ExternalApi.UserGroups.Queries.ExternalGetListOfUserGroupsQuery
{
    public class ExternalUserGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ExternalUserSubGroup> SubGroups { get; set;}
    }

    public class ExternalUserSubGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class ExternalGetListOfUserGroupsQuery: IRequest<ProcessResponseSimple<IEnumerable<ExternalUserGroup>>>
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

    public class ExternalGetListOfUserGroupsQueryHandler : ExternalApiBaseHandler, IRequestHandler<ExternalGetListOfUserGroupsQuery, ProcessResponseSimple<IEnumerable<ExternalUserGroup>>>, IAmWebApplicationHandler
    {
        public ExternalGetListOfUserGroupsQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
        {
            this._masterDatabaseContext = masterDatabaseContext;
            this._databaseContext = databaseContext;
            this._contextAccessor = contextAccessor;
        }

        public async Task<ProcessResponseSimple<IEnumerable<ExternalUserGroup>>> Handle(ExternalGetListOfUserGroupsQuery request, CancellationToken cancellationToken)
        {
            var initResult = await this._Initialize(request.HotelGroupId, request.HotelGroupKey);
            if(initResult != null)
            {
                return initResult;
            }

            var userGroups = await this._databaseContext
                .UserGroups
                .Include(ug => ug.UserSubGroups)
                .ToListAsync();

            return new ProcessResponseSimple<IEnumerable<ExternalUserGroup>>
            {
                Data = userGroups.Select(ug => new ExternalUserGroup
                { 
                    Id = ug.Id,
                    Name = ug.Name,
                    SubGroups = ug.UserSubGroups.Select(sg => new ExternalUserSubGroup
                    {
                        Id = sg.Id,
                        Name = sg.Name,
                    }),
                }),
                IsSuccess = true,
                Message = "User groups and sub groups loaded.",
            };
        }
        
        private async Task<ProcessResponseSimple<IEnumerable<ExternalUserGroup>>> _Initialize(Guid? hotelGroupId, string hotelGroupKey)
        {
            var authResult = await this.AuthorizeExternalClient();
            if (!authResult.IsSuccess)
                return new ProcessResponseSimple<IEnumerable<ExternalUserGroup>> { IsSuccess = false, Message = authResult.Message, Data = new ExternalUserGroup[0] };

            var initResult = this.InitializeHotelGroupContext(hotelGroupId, hotelGroupKey);
            if (!initResult.IsSuccess)
                return new ProcessResponseSimple<IEnumerable<ExternalUserGroup>> { IsSuccess = false, Message = initResult.Message, Data = new ExternalUserGroup[0] };

            return null;
        }
    }
}
