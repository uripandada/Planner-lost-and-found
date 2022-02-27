using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ImportPreview.Commands.UploadImportPreviewRooms
{
	public class ImportRoomsPreview
	{
		public string FileName { get; set; }
		public IEnumerable<ImportRoomPreview> Rooms { get; set; }
		public bool HasError { get; set; }
		public string Message { get; set; }
	}

	public class ImportRoomPreview
	{
		public string RoomName { get; set; }
		public string RoomType { get; set; }
		public string RoomCategory { get; set; }
		public string Beds { get; set; }
		public string Order { get; set; }
		public string FloorSubSection { get; set; }
		public string FloorSection { get; set; }
		public string Floor { get; set; }
		public string Building { get; set; }
		public string Area { get; set; }
		public string Hotel { get; set; }
		public bool HasError { get; set; }
		public string Message { get; set; }
		public bool AlreadyExists { get; set; }
	}

	public class UploadImportPreviewRoomsCommand : IRequest<ImportRoomsPreview>
	{
		public IFormFile File { get; set; }
	}

	public class UploadImportPreviewRoomsCommandHandler : IRequestHandler<UploadImportPreviewRoomsCommand, ImportRoomsPreview>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public UploadImportPreviewRoomsCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ImportRoomsPreview> Handle(UploadImportPreviewRoomsCommand request, CancellationToken cancellationToken)
		{
			var importPreview = new ImportRoomsPreview();
			importPreview.FileName = request.File.FileName;
			importPreview.HasError = false;
			importPreview.Message = "Data extracted.";
			importPreview.Rooms = new List<ImportRoomPreview>();

			try
			{
				using (ExcelEngine excelEngine = new ExcelEngine())
				{
					IApplication application = excelEngine.Excel;

					application.DefaultVersion = ExcelVersion.Excel2016;
					application.CSVSeparator = ";";

					using (var ms = new MemoryStream())
					{
						request.File.CopyTo(ms);

						ms.Position = 0;

						IWorkbook workbook = application.Workbooks.Open(ms, ExcelOpenType.Automatic);

						IWorksheet worksheet = workbook.Worksheets[0];

						if (worksheet.Rows.Length > 0)
						{
							importPreview.Rooms = this._ExtractRoomsFromTheWorksheet(worksheet);
						}
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

			await this._ValidateExtractedData(importPreview);

			importPreview.Rooms = importPreview.Rooms
				.OrderBy(r => r.HasError ? 0 : 1)
				.ThenBy(r => r.Hotel)
				.ThenBy(r => r.Order)
				.ThenBy(r => r.Building)
				.ThenBy(r => r.Floor)
				.ThenBy(r => r.FloorSection)
				.ThenBy(r => r.FloorSubSection)
				.ThenBy(r => r.RoomName)
				.ToArray();

			return importPreview;
		}

		private IEnumerable<ImportRoomPreview> _ExtractRoomsFromTheWorksheet(IWorksheet worksheet)
		{
			var rooms = new List<ImportRoomPreview>();

			var cells = this._GetColumnsFromWorksheet(worksheet.Rows[0]);

			foreach (IRange row in worksheet.Rows.Skip(1))
			{
				ImportRoomPreview ipr = new ImportRoomPreview();
				ipr.RoomName = Convert.ToString(row.Cells[cells["Room Name"]].Value);
				ipr.RoomType = Convert.ToString(row.Cells[cells["Room Type"]].Value);
				ipr.RoomCategory = Convert.ToString(row.Cells[cells["Room Category"]].Value);
				ipr.Beds = Convert.ToString(row.Cells[cells["Beds"]].Value);
				ipr.Order = Convert.ToString(row.Cells[cells["Order"]].Value);
				ipr.FloorSubSection = Convert.ToString(row.Cells[cells["Floor SubSection"]].Value);
				ipr.FloorSection = Convert.ToString(row.Cells[cells["Floor Section"]].Value);
				ipr.Floor = Convert.ToString(row.Cells[cells["Floor"]].Value);
				ipr.Building = Convert.ToString(row.Cells[cells["Building"]].Value);
				ipr.Area = Convert.ToString(row.Cells[cells["Area"]].Value);
				ipr.Hotel = Convert.ToString(row.Cells[cells["Hotel"]].Value);

				rooms.Add(ipr);
			}

			return rooms;
		}

		private Dictionary<string, int> _GetColumnsFromWorksheet(IRange range)
		{
			var columns = new Dictionary<string, int>();

			foreach (var col in range.Columns)
			{
				var _columnName = col.Value;
				var _columnIndex = col.Column - 1;
				columns.Add(_columnName, _columnIndex);
			}

			return columns;
		}

		private async Task _ValidateExtractedData(ImportRoomsPreview roomsPreview)
		{
			var hotels = await this._databaseContext.Hotels.ToArrayAsync();
			var roomTypeSet = new HashSet<string>
			{
				Common.Enums.RoomTypeEnum.APPARTMENT.ToString(),
				Common.Enums.RoomTypeEnum.HOTEL.ToString(),
				Common.Enums.RoomTypeEnum.HOSTEL.ToString(),
				Common.Enums.RoomTypeEnum.UNKNOWN.ToString(),
			};


			var duplicatesCheck = roomsPreview.Rooms.GroupBy(r => r.RoomName).ToDictionary(r => r.Key, r => r.ToArray());

			foreach (ImportRoomPreview item in roomsPreview.Rooms)
			{
				var hasError = false;
				var messages = new List<string>();

				if (item.RoomName.IsNull())
				{
					hasError = true;
					messages.Add("RoomName is required.");
				}
				
				if (item.RoomType.IsNull())
				{
					hasError = true;
					messages.Add("RoomType is required.");
				}
				else if (!roomTypeSet.Contains(item.RoomType))
				{
					hasError = true;
					messages.Add($"Valid RoomType values: {string.Join(", ", roomTypeSet)}");
				}
				
				if (item.RoomCategory.IsNull())
				{
					hasError = true;
					messages.Add("RoomCategory is required.");
				}
				
				if (item.Floor.IsNull())
				{
					hasError = true;
					messages.Add("Floor is required.");
				}
				
				if (item.Building.IsNull())
				{
					hasError = true;
					messages.Add("Building is required.");
				}
				
				if (item.Hotel.IsNull())
				{
					hasError = true;
					messages.Add("Hotel is required.");
				}
				else
				{
					var hotelKey = item.Hotel.Trim().ToLower();
					var existingHotel = hotels.Where(hotel => hotel.Name.Trim().ToLower() == hotelKey).FirstOrDefault();
					if (existingHotel == null)
					{
						hasError = true;
						messages.Add("Unknown hotel.");
					}
				}

				if(duplicatesCheck[item.RoomName].Length > 1)
				{
					hasError = true;
					messages.Add("Duplicate room name.");
				}

				item.HasError = hasError;
				item.Message = string.Join(" ", messages);
			}
		}
	}
}