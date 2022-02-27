using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.OnGuards.Queries.GetList
{
    public class GetOnGuardListQuery : GetPageRequest, IRequest<ProcessResponse<PageOf<Models.OnGuardListItem>>>
    {
        public string Keyword { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    public class GetOnGuardListQueryHandler : IRequestHandler<GetOnGuardListQuery, ProcessResponse<PageOf<Models.OnGuardListItem>>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;

        public GetOnGuardListQueryHandler(IDatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<ProcessResponse<PageOf<Models.OnGuardListItem>>> Handle(GetOnGuardListQuery request, CancellationToken cancellationToken)
        {
            var query = databaseContext.OnGuards.AsQueryable();

            if (!String.IsNullOrWhiteSpace(request.Keyword))
            {

                query = query.Where(x =>
                x.FirstName.ToLower().Contains(request.Keyword.ToLower())
                || x.LastName.ToLower().Contains(request.Keyword.ToLower())
                || x.ReferenceNumber.ToLower().Contains(request.Keyword.ToLower())
                || x.Identification.ToLower().Contains(request.Keyword.ToLower())
                || x.PhoneNumber.ToLower().Contains(request.Keyword.ToLower())
                || x.Address.ToLower().Contains(request.Keyword.ToLower())
                || x.LastName.ToLower().Contains(request.Keyword.ToLower())
                || x.Description.ToLower().Contains(request.Keyword.ToLower())
                );
            }

            if (request.DateFrom.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= request.DateFrom.Value);
            }

            if (request.DateTo.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= request.DateTo.Value);
            }

            var lostAndFound = await query.Select(x => new Models.OnGuardListItem
            {
                Id = x.Id,
                Address = x.Address,
                FirstName = x.FirstName,
                LastName = x.LastName,
                PhoneNumber = x.PhoneNumber,
                ReferenceNumber = x.ReferenceNumber,
                Description = x.Description,
                Status = x.Status,
                Identification = x.Identification,
                CreatedAt = x.CreatedAt
            }).Skip(request.Skip).Take(request.Take).ToListAsync();

            var count = await query.CountAsync();

            return new ProcessResponse<PageOf<Models.OnGuardListItem>>
            {
                Data = new PageOf<Models.OnGuardListItem>
                {
                    Items = lostAndFound,
                    TotalNumberOfItems = count
                },
                IsSuccess = true
            };

        }
    }

}
