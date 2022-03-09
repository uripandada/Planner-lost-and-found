using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.LostAndFounds.Models;
using Planner.Application.TaskManagement.Queries.GetTasksData;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.LostAndFounds.Commands.Insert
{
	public class InsertLostAndFoundCommand : IRequest<ProcessResponse<string>>
	{
		public string HotelId { get; set; }
		public bool IsLostItem { get; set; }
		public string Description { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
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

		public string PlaceOfStorage { get; set; }
		public string TrackingNumber { get; set; }

		public string ClientName { get; set; }
		public string FounderName { get; set; }
		public string FounderEmail { get; set; }
		public string FounderPhoneNumber { get; set; }
		public Guid? LostAndFoundCategoryId { get; set; }
		public Guid? StorageRoomId { get; set; }

		public SaveLostAndFoundWhereData WhereData { get; set; }

		public IEnumerable<LostAndFoundFilesUploadedData> Files { get; set; }
	}

	public class SaveLostAndFoundWhereData : TaskWhereData
	{
	}

	public class InsertLostAndFoundCommandHandler : IRequestHandler<InsertLostAndFoundCommand, ProcessResponse<string>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IFileService _fileService;
		private readonly Guid _userId;

		public InsertLostAndFoundCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor, IFileService fileService)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._fileService = fileService;
			this._userId = httpContextAccessor.UserId();
		}

		public async Task<ProcessResponse<string>> Handle(InsertLostAndFoundCommand request, CancellationToken cancellationToken)
		{
			var hotel = await this._databaseContext.Hotels.FindAsync(request.HotelId);
			var timeZoneInfo = HotelLocalDateProvider.GetAvailableTimeZoneInfo(hotel.WindowsTimeZoneId, hotel.IanaTimeZoneId);
			var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
			var dateFrom = dateTime.Date;
			var dateTo = dateTime.Date.AddDays(1);

			Guid? roomId = null;
			string reservationId = null;
			if (request?.WhereData?.TypeKey == TaskWhereType.RESERVATION.ToString())
			{
				reservationId = request.WhereData.ReferenceId;
			}
			else if (request?.WhereData?.TypeKey == TaskWhereType.ROOM.ToString())
			{
				if (Guid.TryParse(request.WhereData.ReferenceId, out Guid referenceId))
				{
					roomId = referenceId;
				}
			}

			var countOfRecordsInsertedToday = await this._databaseContext
				.LostAndFounds
				//.Select(x => new { CreatedAt = x.CreatedAt.Date) })
				.Where(dts => dts.CreatedAt >= dateFrom && dts.CreatedAt < dateTo)
				.CountAsync();

			var referenceNumber = $"{dateTime.ToString("yyyy-MM-dd")}-{countOfRecordsInsertedToday + 1}";

			var item = new Domain.Entities.LostAndFound
			{
				Address = request.Address,
				City = request.City,
				PostalCode = request.PostalCode,
				Country = request.Country,
				CreatedAt = dateTime,
				CreatedById = this._userId,
				FirstName = request.FirstName,
				Id = Guid.NewGuid(),
				LastName = request.LastName,
				LostOn = request.LostOn,
				ModifiedAt = dateTime,
				ModifiedById = this._userId,
				PhoneNumber = request.PhoneNumber,
				Email = request.Email,
				ReferenceNumber = referenceNumber,
				FoundStatus = request.FoundStatus,
				GuestStatus = request.GuestStatus,
				DeliveryStatus = request.DeliveryStatus,
				OtherStatus = request.OtherStatus,
				TypeOfLoss = request.TypeOfLoss,
				RoomId = roomId,
				ReservationId = reservationId,
				Description = request.Description,
				Notes = request.Notes,
				HotelId = request.HotelId,
				IsClosed = false,
				IsDeleted = false,
				Type = request.IsLostItem ? LostAndFoundRecordType.Lost : LostAndFoundRecordType.Found,
				RccStatus = RccLostAndFoundStatus.OPEN,
				ClientName = request.ClientName,
				FounderName = request.FounderName,
				FounderEmail = request.FounderEmail,
				FounderPhoneNumber = request.FounderPhoneNumber,
				LostAndFoundCategoryId = request.LostAndFoundCategoryId,
				StorageRoomId = request.StorageRoomId
			};

			//var storageDirectory = this._fileService.GetLostAndFoundStoragePath(item.Id);
			//if (!Directory.Exists(storageDirectory)) Directory.CreateDirectory(storageDirectory);

			//foreach (var f in request.Files)
			//{
			//	// 1. read file from temporary upload directory
			//	// 2. if file is image, resize it first
			//	// 3. copy the (resized) file to the destination directory
			//	// 4. save the (resized) file backup to the database
			//	// 5. link the file with the asset

			//	var processResult = await this._fileService.ProcessNewAssetFile(item.Id, f.FileName);

			//	var file = new Domain.Entities.File
			//	{
			//		CreatedAt = dateTime,
			//		CreatedById = this._userId,
			//		FileData = await File.ReadAllBytesAsync(processResult.FilePath),
			//		FileName = f.FileName,
			//		Id = Guid.NewGuid(),
			//		ModifiedAt = dateTime,
			//		ModifiedById = this._userId,
			//		FileTypeKey = "LOSTANDFOUND"
			//	};

			//	var lostAndFoundFile = new Domain.Entities.LostAndFoundFile
			//	{
			//		LostAndFoundId = item.Id,
			//		FileId = file.Id,
			//	};
			//	lostAndFoundFile.File = file;

			//	item.Files.Add(lostAndFoundFile);
			//}

			await this._databaseContext.LostAndFounds.AddAsync(item);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<string>
			{
				Data = item.Id.ToString(),
				IsSuccess = true,
				Message = "Lost & Found item saved."
			};
		}
	}
}