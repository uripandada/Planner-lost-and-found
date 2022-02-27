using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.OnGuards.Commands.Update
{
    public class UpdateOnGuardCommand : IRequest<ProcessResponse>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Identification { get; set; }
        public string ReferenceNumber { get; set; }
        public string Description { get; set; }
        public OnGuardStatus Status { get; set; }
        public IEnumerable<Models.OnGuardFilesUploadedData> Files { get; set; }

    }

    public class UpdateOnGuardCommandHandler : IRequestHandler<UpdateOnGuardCommand, ProcessResponse>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IFileService fileService;

        public UpdateOnGuardCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, IFileService fileService)
        {
            this.databaseContext = databaseContext;
            this.httpContextAccessor = httpContextAccessor;
            this.fileService = fileService;
        }

        public async Task<ProcessResponse> Handle(UpdateOnGuardCommand request, CancellationToken cancellationToken)
        {
            var item = await this.databaseContext.OnGuards.SingleAsync(x => x.Id == request.Id);
            item.Address = request.Address;
            item.FirstName = request.FirstName;
            item.LastName = request.LastName;
            item.ModifiedAt = DateTime.UtcNow;
            item.ModifiedById = httpContextAccessor.UserId();
            item.PhoneNumber = request.PhoneNumber;
            item.ReferenceNumber = request.ReferenceNumber;
            item.Status = request.Status;
            item.Identification = request.Identification;
            item.Description = request.Description;

            var existingOnGuardFilesMap = item.Files.ToDictionary(af => af.FileId);
            var filesToInsert = new List<Domain.Entities.File>();
            var onGuardFilesToInsert = new List<Domain.Entities.OnGuardFile>();
            var onGuardFilesToDelete = new List<Domain.Entities.OnGuardFile>();
            var checkedFileIds = new HashSet<Guid>();

            foreach (var f in request.Files)
            {
                if (f.Id.HasValue)
                {
                    checkedFileIds.Add(f.Id.Value);
                    continue;
                }

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

                filesToInsert.Add(file);
                onGuardFilesToInsert.Add(lostAndFoundFile);
            }

            foreach (var lostAndFoundFile in existingOnGuardFilesMap.Values)
            {
                if (!checkedFileIds.Contains(lostAndFoundFile.FileId))
                {
                    onGuardFilesToDelete.Add(lostAndFoundFile);
                }
            }

            using (var transaction = await this.databaseContext.Database.BeginTransactionAsync())
            {
                if (filesToInsert.Any())
                {
                    await this.databaseContext.Files.AddRangeAsync(filesToInsert);
                }
                if (onGuardFilesToInsert.Any())
                {
                    await this.databaseContext.OnGuardFiles.AddRangeAsync(onGuardFilesToInsert);
                }
                if (onGuardFilesToDelete.Any())
                {
                    this.databaseContext.OnGuardFiles.RemoveRange(onGuardFilesToDelete);
                }
                await this.databaseContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            
            return new ProcessResponse
            {
                IsSuccess = true,
                Message = "OnGuard item updated."
            };
        }
    }
}
