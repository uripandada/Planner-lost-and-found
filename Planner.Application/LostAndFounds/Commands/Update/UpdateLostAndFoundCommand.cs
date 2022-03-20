using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.LostAndFounds.Models;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.LostAndFounds.Commands.Update
{
    public class UpdateLostAndFoundCommand : IRequest<ProcessResponse>
    {
        public string HotelId { get; set; }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
        public Guid? RoomId { get; set; }
        public string ReservationId { get; set; }
        public DateTime? LostOn { get; set; }
        public FoundStatus FoundStatus { get; set; }
        public GuestStatus GuestStatus { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public OtherStatus OtherStatus { get; set; }
        public TypeOfLoss TypeOfLoss { get; set; }
        public SaveLostAndFoundWhereData WhereData { get; set; }
        public IEnumerable<LostAndFoundFilesUploadedData> Files { get; set; }
        public bool IsLostItem { get; set; }

        public string PlaceOfStorage { get; set; }
        public string TrackingNumber { get; set; }
        public string ClientName { get; set; }
        public string FounderName { get; set; }
        public string FounderEmail { get; set; }
        public string FounderPhoneNumber { get; set; }
        public Guid? LostAndFoundCategoryId { get; set; }
        public Guid? StorageRoomId { get; set; }
    }

    public class SaveLostAndFoundWhereData : TaskWhereData
    {
    }

    public class UpdateLostAndFoundCommandHandler : IRequestHandler<UpdateLostAndFoundCommand, ProcessResponse>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IFileService fileService;

        public UpdateLostAndFoundCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, IFileService fileService)
        {
            this.databaseContext = databaseContext;
            this.httpContextAccessor = httpContextAccessor;
            this.fileService = fileService;
        }

        public async Task<ProcessResponse> Handle(UpdateLostAndFoundCommand request, CancellationToken cancellationToken)
        {
            Guid? roomId = null;
            string reservationId = null;
            if (request.WhereData?.TypeKey == TaskWhereType.RESERVATION.ToString())
            {
                reservationId = request.WhereData.ReferenceId;
            }
            else if (request.WhereData?.TypeKey == TaskWhereType.ROOM.ToString())
            {
                if (Guid.TryParse(request.WhereData.ReferenceId, out Guid referenceId))
                {
                    roomId = referenceId;
                }
            }

            var item = await this.databaseContext.LostAndFounds.Include(x => x.Files).SingleAsync(x => x.Id == request.Id);
            item.Address = request.Address;
            item.City = request.City;
            item.PostalCode = request.PostalCode;
            item.Country = request.Country;
            item.Name = request.Name;
            item.LostOn = request.LostOn;
            item.ModifiedAt = DateTime.UtcNow;
            item.ModifiedById = httpContextAccessor.UserId();
            item.PhoneNumber = request.PhoneNumber;
            item.Email = request.Email;
            item.FoundStatus = request.FoundStatus;
            item.GuestStatus = request.GuestStatus;
            item.DeliveryStatus = request.DeliveryStatus;
            item.OtherStatus = request.OtherStatus;
            item.TypeOfLoss = request.TypeOfLoss;
            item.RoomId = roomId;
            item.ReservationId = reservationId;
            item.Description = request.Description;
            item.Notes = request.Notes;
            item.ClientName = request.ClientName;
            item.FounderName = request.FounderName;
            item.FounderEmail = request.FounderEmail;
            item.FounderPhoneNumber = request.FounderPhoneNumber;
            item.LostAndFoundCategoryId = request.LostAndFoundCategoryId;
            item.StorageRoomId = request.StorageRoomId;


            var existingLostAndFoundFilesMap = item.Files.ToDictionary(af => af.FileId);
            var filesToInsert = new List<Domain.Entities.File>();
            var lostAndFoundFilesToInsert = new List<Domain.Entities.LostAndFoundFile>();
            var lostAndFoundFilesToDelete = new List<Domain.Entities.LostAndFoundFile>();
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
                    FileTypeKey = "LOSTANDFOUND"
                };
                var lostAndFoundFile = new Domain.Entities.LostAndFoundFile
                {
                    LostAndFoundId = item.Id,
                    FileId = file.Id,
                };
                filesToInsert.Add(file);
                lostAndFoundFilesToInsert.Add(lostAndFoundFile);
            }
            foreach (var lostAndFoundFile in existingLostAndFoundFilesMap.Values)
            {
                if (!checkedFileIds.Contains(lostAndFoundFile.FileId))
                {
                    lostAndFoundFilesToDelete.Add(lostAndFoundFile);
                }
            }
            using (var transaction = await this.databaseContext.Database.BeginTransactionAsync())
            {
                if (filesToInsert.Any())
                {
                    await this.databaseContext.Files.AddRangeAsync(filesToInsert);
                }
                if (lostAndFoundFilesToInsert.Any())
                {
                    await this.databaseContext.LostAndFoundFiles.AddRangeAsync(lostAndFoundFilesToInsert);
                }
                if (lostAndFoundFilesToDelete.Any())
                {
                    this.databaseContext.LostAndFoundFiles.RemoveRange(lostAndFoundFilesToDelete);
                }
                await this.databaseContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }

            return new ProcessResponse
            {
                IsSuccess = true,
                Message = "Lost & Found item updated."
            };
        }
    }
}
