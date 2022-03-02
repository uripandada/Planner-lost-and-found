using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ImportPreview.Commands.UploadImportPreviewAssetActions
{
	public class ImportAssetActionsPreview
	{
		public string FileName { get; set; }
		public IEnumerable<ImportAssetActionPreview> AssetActions { get; set; }
		public bool HasError { get; set; }
		public string Message { get; set; }
	}

	public class ImportAssetActionPreview
	{
		public string Asset { get; set; }
		public string Action { get; set; }
		public int? Credits { get; set; }
		public decimal? Price { get; set; }
		public string Type { get; set; }
		public string Priority { get; set; }

		public bool HasError { get; set; }
		public string Message { get; set; }
	}

	public class UploadImportPreviewAssetActionsCommand : IRequest<ImportAssetActionsPreview>
	{
		public IFormFile File { get; set; }
	}

	public class UploadImportPreviewAssetActionsCommandHandler : IRequestHandler<UploadImportPreviewAssetActionsCommand, ImportAssetActionsPreview>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public UploadImportPreviewAssetActionsCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ImportAssetActionsPreview> Handle(UploadImportPreviewAssetActionsCommand request, CancellationToken cancellationToken)
		{
			ImportAssetActionsPreview importPreview = new ImportAssetActionsPreview();
			importPreview.FileName = request.File.FileName;
			importPreview.HasError = false;
			importPreview.Message = "Data extracted.";
			importPreview.AssetActions = new List<ImportAssetActionPreview>();

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

						IWorkbook workbook = application.Workbooks.Open(ms, ";", System.Text.Encoding.UTF8);
						IWorksheet worksheet = workbook.Worksheets[0];

						if (worksheet.Rows.Length > 0)
						{
							importPreview.AssetActions = this._ExtractAssetActionsFromTheWorksheet(worksheet);
						}
					}
				}
			}
			catch (Exception)
			{
				importPreview.HasError = true;
				importPreview.Message = "Invalid file format.";
				return importPreview;
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

		private IEnumerable<ImportAssetActionPreview> _ExtractAssetActionsFromTheWorksheet(IWorksheet worksheet)
		{
			var assets = new List<ImportAssetActionPreview>();

			var columnsMap = this._GetColumnsFromWorksheet(worksheet.Rows[0]);

			foreach (IRange row in worksheet.Rows.Skip(1))
			{
				var asset = new ImportAssetActionPreview();

				var creditsString = row.Cells[columnsMap["Credits"]].Value;
				var priceString = row.Cells[columnsMap["Price"]].Value;

				asset.Action = Convert.ToString(row.Cells[columnsMap["Action"]].Value);
				asset.Asset = Convert.ToString(row.Cells[columnsMap["Asset"]].Value);
				asset.Credits = creditsString.IsNotNull() ? Convert.ToInt32(creditsString) : null;
				asset.Price = priceString.IsNotNull() ? Convert.ToDecimal(priceString) : null;
				asset.Type = Convert.ToString(row.Cells[columnsMap["Type"]].Value);
				asset.Priority = Convert.ToString(row.Cells[columnsMap["Priority"]].Value);

				assets.Add(asset);
			}

			return assets;
		}

		private async Task _ValidateExtractedData(ImportAssetActionsPreview returnValue)
		{
			var assetGroups = await this._databaseContext.AssetGroups.ToArrayAsync();

			var assetGroupsMap = new Dictionary<string, AssetGroup>();
			foreach(var assetGroup in assetGroups)
			{
				var key = assetGroup.Name.Trim().ToLower();
				if (!assetGroupsMap.ContainsKey(key))
				{
					assetGroupsMap.Add(key, assetGroup);
				}
			}

			foreach (var item in returnValue.AssetActions)
			{
				var assetKey = item.Asset.Trim().ToLower();
				if (!assetGroupsMap.ContainsKey(assetKey))
				{
					item.HasError = true;
					item.Message = $"Asset doesn't exist.";
					continue;
				}

				if (string.IsNullOrWhiteSpace(item.Action))
				{
					item.HasError = true;
					item.Message = $"Action is required.";
					continue;
				}

				if (string.IsNullOrWhiteSpace(item.Asset))
				{
					item.HasError = true;
					item.Message = $"Asset is required.";
					continue;
				}

				if (string.IsNullOrWhiteSpace(item.Type))
				{
					item.HasError = true;
					item.Message = $"Type is required.";
					continue;
				}
				else
				{
					var typeKey = item.Type.Trim().ToUpper();
					if (typeKey != "QUICK" && typeKey != "TIMED")
					{
						item.HasError = true;
						item.Message = $"Type value can be either QUICK or TIMED.";
						continue;
					}
				}

				if (string.IsNullOrWhiteSpace(item.Priority))
				{
					item.HasError = true;
					item.Message = $"Priority is required.";
					continue;
				}
				else
				{
					var priorityKey = item.Priority.Trim().ToUpper();
					if (priorityKey != "LOW" && priorityKey != "HIGH" && priorityKey != "NORMAL")
					{
						item.HasError = true;
						item.Message = $"Priority value can be either LOW, NORMAL or HIGH.";
						continue;
					}
				}
			}
		}
	}
}
