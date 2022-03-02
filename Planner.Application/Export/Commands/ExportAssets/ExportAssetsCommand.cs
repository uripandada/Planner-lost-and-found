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

namespace Planner.Application.Export.Commands.ExportAssets
{
	public class ExcelAsset
	{
		public string AssetName { get; set; }
		public string AssetGroupName { get; set; }
		public string IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public string AssetImageName { get; set; }
		public string AssetQrCodeImageName { get; set; }
		public string AssetTags { get; set; }
	}

	public class ExportAssetsCommand : IRequest<byte[]>
	{
	}

	public class ExportAssetsCommandHandler : IRequestHandler<ExportAssetsCommand, byte[]>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext databaseContext;

		public ExportAssetsCommandHandler(IDatabaseContext databaseContext)
		{
			this.databaseContext = databaseContext;
		}

		public async Task<byte[]> Handle(ExportAssetsCommand request, CancellationToken cancellationToken)
		{
			byte[] assetsExcel = null;
			Asset[] assets = await this.databaseContext
				.Assets
				.Include(a => a.AssetGroup)
				.Include(a => a.AssetTags)
				.ToArrayAsync();

			if (assets != null)
			{
				using (ExcelEngine excelEngine = new ExcelEngine())
				{
					var application = excelEngine.Excel;
					application.DefaultVersion = ExcelVersion.Excel2016;

					var workbook = application.Workbooks.Create(1);
					var worksheet = workbook.Worksheets[0];

					worksheet.Range["A1"].Text = "Asset Name";
					worksheet.Range["B1"].Text = "Asset Group Name";
					worksheet.Range["C1"].Text = "Is Bulk";
					worksheet.Range["D1"].Text = "Serial Number";
					worksheet.Range["E1"].Text = "Asset Image Name";
					worksheet.Range["F1"].Text = "Asset Qr Code Image Name";
					worksheet.Range["G1"].Text = "Asset Tags";

					var assetsForInsert = assets.Select(u => this._GenerateAssetForExcel(u)).ToArray();
					worksheet.ImportData(assetsForInsert, 2, 1, false);
					worksheet.UsedRange.AutofitColumns();

					worksheet.UsedRange.AutofitColumns();
					IListObject table = worksheet.ListObjects.Create("AssetsTable", worksheet["A1:G" + (assetsForInsert.Count() + 1).ToString()]);
					//Formatting table with a built-in style
					table.BuiltInTableStyle = TableBuiltInStyles.TableStyleLight14;

					using (var mstream = new MemoryStream())
					{
						workbook.SaveAs(mstream, ";");
						assetsExcel = mstream.ToArray();
					}
				}
			}

			return assetsExcel;
		}

		private ExcelAsset _GenerateAssetForExcel(Asset asset)
		{
			return new ExcelAsset
			{
				AssetName = asset.Name,
				AssetGroupName = asset.AssetGroup != null ? asset.AssetGroup.Name : null,
				IsBulk = Convert.ToString(asset.IsBulk),
				SerialNumber = asset.SerialNumber,
				AssetImageName = null,
				AssetQrCodeImageName = null,
				AssetTags = asset.AssetTags != null ? _GetAssetTagsString(asset.AssetTags) : null
			};
		}

		private string _GetAssetTagsString(IEnumerable<AssetTag> assetTags)
		{
			return string.Join(", ", assetTags.Select(t => t.TagKey).ToArray());
		}
	}
}
