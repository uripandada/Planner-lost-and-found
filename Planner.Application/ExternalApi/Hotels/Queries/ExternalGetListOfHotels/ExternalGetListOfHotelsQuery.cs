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

namespace Planner.Application.ExternalApi.Hotels.Queries.ExternalGetListOfHotels
{
    public class ExternalHotel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IanaTimeZoneId { get; set; }
        public string WindowsTimeZoneId { get; set; }
    }

    public class ExternalGetListOfHotelsQuery : IRequest<ProcessResponseSimple<IEnumerable<ExternalHotel>>>
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

    public class ExternalGetListOfHotelsQueryHandler : ExternalApiBaseHandler, IRequestHandler<ExternalGetListOfHotelsQuery, ProcessResponseSimple<IEnumerable<ExternalHotel>>>, IAmWebApplicationHandler
    {
        public ExternalGetListOfHotelsQueryHandler(IMasterDatabaseContext masterDatabaseContext, IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
        {
            this._masterDatabaseContext = masterDatabaseContext;
            this._databaseContext = databaseContext;
            this._contextAccessor = contextAccessor;
        }

        public async Task<ProcessResponseSimple<IEnumerable<ExternalHotel>>> Handle(ExternalGetListOfHotelsQuery request, CancellationToken cancellationToken)
        {
            var initResult = await this._Initialize(request.HotelGroupId, request.HotelGroupKey);
            if (initResult != null)
            {
                return initResult;
            }

            var hotels = await this._databaseContext
                .Hotels
                .Select(h => new ExternalHotel 
                { 
                    Id = h.Id,
                    Name = h.Name,
                    IanaTimeZoneId = h.IanaTimeZoneId,
                    WindowsTimeZoneId = h.WindowsTimeZoneId,
                })
                .ToListAsync();

            return new ProcessResponseSimple<IEnumerable<ExternalHotel>>
            {
                Data = hotels,
                IsSuccess = true,
                Message = "Hotels loaded.",
            };
        }

        private async Task<ProcessResponseSimple<IEnumerable<ExternalHotel>>> _Initialize(Guid? hotelGroupId, string hotelGroupKey)
        {
            var authResult = await this.AuthorizeExternalClient();
            if (!authResult.IsSuccess)
                return new ProcessResponseSimple<IEnumerable<ExternalHotel>> { IsSuccess = false, Message = authResult.Message, Data = new ExternalHotel[0] };

            var initResult = this.InitializeHotelGroupContext(hotelGroupId, hotelGroupKey);
            if (!initResult.IsSuccess)
                return new ProcessResponseSimple<IEnumerable<ExternalHotel>> { IsSuccess = false, Message = initResult.Message, Data = new ExternalHotel[0] };

            if(!this._canAccessListOfHotels)
                return new ProcessResponseSimple<IEnumerable<ExternalHotel>> { IsSuccess = false, Message = "Access denied.", Data = new ExternalHotel[0] };

            return null;
        }
    }
}
