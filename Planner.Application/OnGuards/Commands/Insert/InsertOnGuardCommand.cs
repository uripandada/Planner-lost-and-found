using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Application.OnGuards.Models;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.OnGuards.Commands.Insert
{
    public class InsertOnGuardCommand : IRequest<ProcessResponse<string>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Identification { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public OnGuardStatus Status { get; set; }
        public IEnumerable<OnGuardFilesUploadedData> Files { get; set; }

    }

    public class InsertOnGuardCommandHandler : IRequestHandler<InsertOnGuardCommand, ProcessResponse<string>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IFileService fileService;

        public InsertOnGuardCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, IFileService fileService)
        {
            this.databaseContext = databaseContext;
            this.httpContextAccessor = httpContextAccessor;
            this.fileService = fileService;
        }

        public async Task<ProcessResponse<string>> Handle(InsertOnGuardCommand request, CancellationToken cancellationToken)
        {
            var item = new Domain.Entities.OnGuard
            {
                Address = request.Address,
                CreatedAt = DateTime.UtcNow,
                CreatedById = httpContextAccessor.UserId(),
                FirstName = request.FirstName,
                Id = Guid.NewGuid(),
                LastName = request.LastName,
                ModifiedAt = DateTime.UtcNow,
                ModifiedById = httpContextAccessor.UserId(),
                PhoneNumber = request.PhoneNumber,
                ReferenceNumber = request.ReferenceNumber,
                Description = request.Description,
                Status = request.Status,
                Identification = request.Identification
            };


            var storageDirectory = this.fileService.GetOnGuardStoragePath(item.Id);
            if (!Directory.Exists(storageDirectory)) Directory.CreateDirectory(storageDirectory);


            foreach (var f in request.Files)
            {
                // 1. read file from temporary upload directory
                // 2. if file is image, resize it first
                // 3. copy the (resized) file to the destination directory
                // 4. save the (resized) file backup to the database
                // 5. link the file with the asset

                var processResult = await this.fileService.ProcessNewAssetFile(item.Id, f.FileName);

                var file = new Domain.Entities.File
                {
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = this.httpContextAccessor.UserId(),
                    FileData = await System.IO.File.ReadAllBytesAsync(processResult.FilePath),
                    FileName = f.FileName,
                    Id = Guid.NewGuid(),
                    ModifiedAt = DateTime.UtcNow,
                    ModifiedById = this.httpContextAccessor.UserId(),
                    FileTypeKey = "ONGUARD"
                };

                var lostAndFoundFile = new Domain.Entities.OnGuardFile
                {
                    OnGuardId = item.Id,
                    FileId = file.Id,
                };
                lostAndFoundFile.File = file;

                item.Files.Add(lostAndFoundFile);
            }
            
            this.databaseContext.OnGuards.Add(item);
            await this.databaseContext.SaveChangesAsync(cancellationToken);

            return new ProcessResponse<string>
            {
                Data = item.Id.ToString(),
                IsSuccess = true,
                Message = "OnGuard item saved."
            };
        }
    }
}
