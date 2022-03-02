using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Domain.Entities;
using Syncfusion.XlsIO;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Export.Commands.ExportRooms
{
	public class ExcelRoom
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
		public string Hotel { get; set; }
		public string Area { get; set; }
	}

	public class ExportRoomsCommand : IRequest<byte[]>
	{
	}

	public class ExportRoomsCommandHandler : IRequestHandler<ExportRoomsCommand, byte[]>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;

		public ExportRoomsCommandHandler(IDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<byte[]> Handle(ExportRoomsCommand request, CancellationToken cancellationToken)
		{
			byte[] roomsExcel = null;
			Room[] rooms = await this.databaseContext
				.Rooms
				.Include(r => r.RoomBeds)
				.Include(r => r.Building)
				.Include(r => r.Floor)
				.Include(r => r.Hotel)
				.Include(r => r.Area)
				.Include(r => r.Category)
				.OrderBy(r => r.OrdinalNumber)
				.ToArrayAsync();

			if (rooms != null)
			{
				using (ExcelEngine excelEngine = new ExcelEngine())
				{
					var application = excelEngine.Excel;
					application.DefaultVersion = ExcelVersion.Excel2016;

					var workbook = application.Workbooks.Create(1);
					var worksheet = workbook.Worksheets[0];

					var integerColumnStyle = workbook.Styles.Add("IntegerColumnStyle");
					integerColumnStyle.NumberFormat = "0";
					
					var textColumnStyle = workbook.Styles.Add("TextColumnStyle");
					textColumnStyle.NumberFormat = "text";

					worksheet.Range["A1"].Text = "Room Name";
					worksheet.SetDefaultColumnStyle(1, textColumnStyle);

					worksheet.Range["B1"].Text = "Room Type";
					worksheet.SetDefaultColumnStyle(2, textColumnStyle);

					worksheet.Range["C1"].Text = "Room Category";
					worksheet.SetDefaultColumnStyle(3, textColumnStyle);

					worksheet.Range["D1"].Text = "Beds";
					worksheet.SetDefaultColumnStyle(4, textColumnStyle);

					worksheet.Range["E1"].Text = "Order";
					worksheet.SetDefaultColumnStyle(5, integerColumnStyle);

					worksheet.Range["F1"].Text = "Floor SubSection";
					worksheet.SetDefaultColumnStyle(6, textColumnStyle);

					worksheet.Range["G1"].Text = "Floor Section";
					worksheet.SetDefaultColumnStyle(7, textColumnStyle);
					
					worksheet.Range["H1"].Text = "Floor";
					worksheet.SetDefaultColumnStyle(8, textColumnStyle);

					worksheet.Range["I1"].Text = "Building";
					worksheet.SetDefaultColumnStyle(9, textColumnStyle);
					
					worksheet.Range["J1"].Text = "Hotel";
					worksheet.SetDefaultColumnStyle(10, textColumnStyle);
					
					worksheet.Range["K1"].Text = "Area";
					worksheet.SetDefaultColumnStyle(11, textColumnStyle);

					var roomsForInsert = rooms.Select(u => this._GenerateRoomForExcel(u)).ToArray();
					worksheet.ImportData(roomsForInsert, 2, 1, false);

					worksheet.UsedRange.AutofitColumns();

					IListObject table = worksheet.ListObjects.Create("RoomsTable", worksheet["A1:K" + (roomsForInsert.Count() + 1).ToString()]);
					//Formatting table with a built-in style
					table.BuiltInTableStyle = TableBuiltInStyles.TableStyleLight14;

					using (var mstream = new MemoryStream())
					{
						workbook.SaveAs(mstream, ";");
						roomsExcel = mstream.ToArray();
					}
				}
			}

			return roomsExcel;
		}

		private ExcelRoom _GenerateRoomForExcel(Room room)
		{
			return new ExcelRoom
			{
				RoomName = room.Name,
				RoomType = room.TypeKey,
				RoomCategory = room.Category != null ? room.Category.Name : null,
				Order = room.OrdinalNumber.ToString(),
				Beds = room.RoomBeds == null || !room.RoomBeds.Any() ? null : string.Join(", ", room.RoomBeds.Select(rb => rb.Name)),
				FloorSubSection = room.FloorSubSectionName,
				FloorSection = room.FloorSectionName,
				Floor = room.Floor != null ? room.Floor.Name : null,
				Building = room.Building != null ? room.Building.Name : null,
				Area = room.Area != null ? room.Area.Name : null,
				Hotel = room.Hotel != null ? room.Hotel.Name : null
			};
		}
	}
}
