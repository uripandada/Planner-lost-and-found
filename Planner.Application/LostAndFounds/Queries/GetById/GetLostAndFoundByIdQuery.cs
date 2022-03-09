using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Application.LostAndFounds.Models;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.LostAndFounds.Queries.GetById
{
    public class GetLostAndFoundByIdQuery : IRequest<ProcessResponse<Models.LostAndFoundModel>>
    {
        public string LostAndFoundId { get; set; }
    }

    public class GetLostAndFoundByIdQueryHandler : IRequestHandler<GetLostAndFoundByIdQuery, ProcessResponse<Models.LostAndFoundModel>>, IAmWebApplicationHandler
    {
        private readonly IDatabaseContext databaseContext;
        private readonly IFileService fileService;

        public GetLostAndFoundByIdQueryHandler(IDatabaseContext databaseContext, IFileService fileService)
        {
            this.databaseContext = databaseContext;
            this.fileService = fileService;
        }

        public async Task<ProcessResponse<LostAndFoundModel>> Handle(GetLostAndFoundByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await this.databaseContext.LostAndFounds.Include(x=>x.Files).Where(x => x.Id == Guid.Parse(request.LostAndFoundId)).SingleAsync();
          
            var fileIds = item.Files.Select(af => af.FileId).ToArray();

            var files = await this.databaseContext.Files.Where(f => fileIds.Contains(f.Id)).Select(f => new
            {
                Id = f.Id,
                Name = f.FileName
            }).ToListAsync();

            var filesData = new List<LostAndFoundFileModel>();
            foreach (var f in files)
            {
                var fileTypeData = this.fileService.DetermineFileType(f.Name);

                filesData.Add(new LostAndFoundFileModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Extension = fileTypeData.Extension,
                    IsImage = fileTypeData.FileType == FileTypes.IMAGE,
                    Url = this.fileService.GetAssetFileUrl(item.Id, f.Name)
                });
            }

            var result = new Models.LostAndFoundModel
            {
                Address = item.Address,
                //Street = item.Street,
                City = item.City,
                PostalCode = item.PostalCode,
                Country = item.Country,
                FirstName = item.FirstName,
                Id = item.Id,
                Description = item.Description,
                LastName = item.LastName,
                LostOn = item.LostOn,
                PhoneNumber = item.PhoneNumber,
                Email = item.Email,
                ReferenceNumber = item.ReferenceNumber,
                FoundStatus = item.FoundStatus,
                GuestStatus = item.GuestStatus,
                DeliveryStatus = item.DeliveryStatus,
                OtherStatus = item.OtherStatus,
                TypeOfLoss = item.TypeOfLoss.HasValue ? item.TypeOfLoss.Value : Domain.Values.TypeOfLoss.Unknown,
                Files = filesData,
                Notes = item.Notes,
                RoomId = item.RoomId,
                ReservationId = item.ReservationId,
                HotelId = item.HotelId,
                PlaceOfStorage = "",
                TrackingNumber = "",
                ClientName = item.ClientName,
                FounderName = item.FounderName,
                FounderEmail = item.FounderEmail,
                FounderPhoneNumber = item.FounderPhoneNumber,
                LostAndFoundCategoryId = item.LostAndFoundCategoryId,
                StorageRoomId = item.StorageRoomId
            };
            
            return new ProcessResponse<LostAndFoundModel>
            {
                Data = result,
                IsSuccess = true
            };
        }
    }
}
