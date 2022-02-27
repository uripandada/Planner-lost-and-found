using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.OnGuards.Models;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.OnGuards.Queries.GetById
{
    public class GetOnGuardByIdQuery : IRequest<ProcessResponse<Models.OnGuardModel>>
    {
        public string OnGuardId { get; set; }
    }

    public class GetOnGuardByIdQueryHandler : IRequestHandler<GetOnGuardByIdQuery, ProcessResponse<Models.OnGuardModel>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IFileService fileService;

        public GetOnGuardByIdQueryHandler(IDatabaseContext databaseContext, IFileService fileService)
        {
            this.databaseContext = databaseContext;
            this.fileService = fileService;
        }

        public async Task<ProcessResponse<Models.OnGuardModel>> Handle(GetOnGuardByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await this.databaseContext.OnGuards.Where(x => x.Id == Guid.Parse(request.OnGuardId)).SingleAsync();

            var fileIds = item.Files.Select(af => af.FileId).ToArray();

            var files = await this.databaseContext.Files.Where(f => fileIds.Contains(f.Id)).Select(f => new
            {
                Id = f.Id,
                Name = f.FileName
            }).ToListAsync();

            var filesData = new List<OnGuardFileModel>();
            foreach (var f in files)
            {
                var fileTypeData = this.fileService.DetermineFileType(f.Name);

                filesData.Add(new OnGuardFileModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Extension = fileTypeData.Extension,
                    IsImage = fileTypeData.FileType == FileTypes.IMAGE,
                    Url = this.fileService.GetAssetFileUrl(item.Id, f.Name)
                });
            }


            var result = new Models.OnGuardModel
            {
                Address = item.Address,
                FirstName = item.FirstName,
                Id = item.Id,
                LastName = item.LastName,
                PhoneNumber = item.PhoneNumber,
                ReferenceNumber = item.ReferenceNumber,
                Description = item.Description,
                Status = item.Status,
                Identification = item.Identification,
                CreatedAt = item.CreatedAt,
                Files = filesData
            };


            return new ProcessResponse<Models.OnGuardModel>
            {
                Data = result,
                IsSuccess = true
            };
        }
    }
}
