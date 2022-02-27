using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Domain.Entities;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Export.Commands.ExportAssetActions
{
	public class ExcelAssetAction
	{
		public string Asset { get; set; }
		public string Action { get; set; }
		public int? Credits { get; set; }
		public decimal? Price { get; set; }
		public string Type { get; set; }
		public string Priority { get; set; }
	}

	public class ExportAssetActionsCommand : IRequest<byte[]>
	{
	}

	public class ExportAssetActionsCommandHandler : IRequestHandler<ExportAssetActionsCommand, byte[]>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;

		public ExportAssetActionsCommandHandler(IDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<byte[]> Handle(ExportAssetActionsCommand request, CancellationToken cancellationToken)
		{
			byte[] assetsExcel = null;

			var assetGroups = await this.databaseContext.AssetGroups.Include(ag => ag.AssetActions).ToArrayAsync();

			using (ExcelEngine excelEngine = new ExcelEngine())
			{
				var application = excelEngine.Excel;
				application.DefaultVersion = ExcelVersion.Excel2016;

				var workbook = application.Workbooks.Create(1);
				var worksheet = workbook.Worksheets[0];

				var assetActionsForInsert = new List<ExcelAssetAction>();

				foreach (var assetGroup in assetGroups)
				{
					foreach (var assetAction in assetGroup.AssetActions)
					{
						assetActionsForInsert.Add(new ExcelAssetAction
						{
							Asset = assetGroup.Name,
							Action = assetAction.Name,
							Credits = assetAction.Credits,
							Price = assetAction.Price,
							Type = assetAction.QuickOrTimedKey,
							Priority = assetAction.PriorityKey,
						});
					}
				}


				worksheet.Range["A1"].Text = "Asset";
				worksheet.Range["B1"].Text = "Action";
				worksheet.Range["C1"].Text = "Credits";
				worksheet.Range["D1"].Text = "Price";
				worksheet.Range["E1"].Text = "Type";
				worksheet.Range["F1"].Text = "Priority";


				worksheet.ImportData(assetActionsForInsert, 1, 1, true);

				worksheet.UsedRange.AutofitColumns();
				IListObject table = worksheet.ListObjects.Create("AssetsActionsTable", worksheet["A1:F" + (assetActionsForInsert.Count() + 1).ToString()]);
				//Formatting table with a built-in style
				table.BuiltInTableStyle = TableBuiltInStyles.TableStyleLight14;

				using (var mstream = new MemoryStream())
				{
					workbook.SaveAs(mstream, ";");
					assetsExcel = mstream.ToArray();
				}
			}

			return assetsExcel;
		}
	}
}
