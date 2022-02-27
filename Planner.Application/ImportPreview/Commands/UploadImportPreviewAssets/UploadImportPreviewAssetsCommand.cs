using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ImportPreview.Commands.UploadImportPreviewAssets
{
	public class ImportAssetsPreview
	{
		public string FileName { get; set; }
		public IEnumerable<ImportAssetPreview> Assets { get; set; }
		public bool HasError { get; set; }
		public string Message { get; set; }
	}

	public class ImportAssetPreview
	{
		public string AssetName { get; set; }
		public string AssetGroupName { get; set; }
		public bool IsBulk { get; set; }
		public string SerialNumber { get; set; }
		public string AssetImageName { get; set; }
		public string AssetQrCodeImageName { get; set; }
		public string AssetTags { get; set; }
		public bool HasError { get; set; }
		public string Message { get; set; }
		public bool AlreadyExists { get; set; }
	}

	public class UploadImportPreviewAssetsCommand : IRequest<ImportAssetsPreview>
	{
		public IFormFile File { get; set; }
	}

	public class UploadImportPreviewAssetsCommandHandler : IRequestHandler<UploadImportPreviewAssetsCommand, ImportAssetsPreview>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public UploadImportPreviewAssetsCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ImportAssetsPreview> Handle(UploadImportPreviewAssetsCommand request, CancellationToken cancellationToken)
		{
			ImportAssetsPreview importPreview = new ImportAssetsPreview();
			importPreview.FileName = request.File.FileName;
			importPreview.HasError = false;
			importPreview.Message = "Data extracted.";
			importPreview.Assets = new List<ImportAssetPreview>();

			try
			{
				using (ExcelEngine excelEngine = new ExcelEngine())
				{
					IApplication application = excelEngine.Excel;
					application.CSVSeparator = ";";

					application.DefaultVersion = ExcelVersion.Excel2016;

					using (var ms = new MemoryStream())
					{
						request.File.CopyTo(ms);

						ms.Position = 0;

						IWorkbook workbook = application.Workbooks.Open(ms, ExcelOpenType.Automatic);
						IWorksheet worksheet = workbook.Worksheets[0];

						if (worksheet.Rows.Length > 0)
						{
							importPreview.Assets = this._ExtractAssetsFromTheWorksheet(worksheet);
						}
					}
				}
			}
			catch (Exception)
			{
				throw;
			}


			await this._ValidateExtractedData(importPreview);

			return importPreview;
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

		private IEnumerable<ImportAssetPreview> _ExtractAssetsFromTheWorksheet(IWorksheet worksheet)
		{
			var assets = new List<ImportAssetPreview>();

			var columnsMap = this._GetColumnsFromWorksheet(worksheet.Rows[0]);

			foreach (IRange row in worksheet.Rows.Skip(1))
			{
				ImportAssetPreview asset = new ImportAssetPreview();
				asset.AssetName = Convert.ToString(row.Cells[columnsMap["Asset Name"]].Value);
				asset.AssetGroupName = Convert.ToString(row.Cells[columnsMap["Asset Group Name"]].Value);
				asset.IsBulk = Convert.ToBoolean(row.Cells[columnsMap["Is Bulk"]].Value);
				asset.SerialNumber = Convert.ToString(row.Cells[columnsMap["Serial Number"]].Value);
				asset.AssetImageName = Convert.ToString(row.Cells[columnsMap["Asset Image Name"]].Value);
				asset.AssetQrCodeImageName = Convert.ToString(row.Cells[columnsMap["Asset Qr Code Image Name"]].Value);
				asset.AssetTags = Convert.ToString(row.Cells[columnsMap["Asset Tags"]].Value);

				assets.Add(asset);
			}

			return assets;
		}

		private async Task _ValidateExtractedData(ImportAssetsPreview returnValue)
		{
			//var assets = await this._databaseContext.Assets.ToArrayAsync();
			//var assetGroups = await this._databaseContext.AssetGroups.ToArrayAsync();
			//var assetTags = await this._databaseContext.AssetTags.ToArrayAsync();

			foreach (ImportAssetPreview item in returnValue.Assets)
			{
				if (string.IsNullOrWhiteSpace(item.AssetName))
				{
					item.HasError = true;
					item.Message = $"AssetName is a required field.";
					continue;
				}

				//if (String.IsNullOrWhiteSpace(item.SerialNumber))
				//{
				//    var isNameAndGroupUnique = assets.Where(x => 
				//        x.Name.ToLower() == item.AssetName.ToLower() 
				//        && 
				//        (
				//            (x.AssetGroup?.Name != null && item.AssetGroupName != null && item.AssetGroupName != "" && x.AssetGroup?.Name.ToLower() == item.AssetGroupName.ToLower())
				//            ||
				//            (x.AssetGroup?.Name == null && (item.AssetGroupName == null || item.AssetGroupName == ""))
				//        )
				//    ).ToArray();

				//    if (isNameAndGroupUnique.Length > 0)
				//    {
				//        item.AlreadyExists = true;
				//        item.HasError = true;
				//        item.Message = $"AssetName and AssetGroupName combination must be unique.";
				//    }
				//}
				//else
				//{
				//	var isSerialNumberNameAndGroupUnique = assets.Where(x => x.SerialNumber?.ToLower() == item.SerialNumber.ToLower()).ToArray();

				//	if (isSerialNumberNameAndGroupUnique.Length > 0)
				//	{
				//		item.AlreadyExists = true;
				//		item.HasError = true;
				//		item.Message = $"Serial number, AssetName and AssetGroupName combination must be unique.";
				//	}
				//}
			}
		}
	}
}
