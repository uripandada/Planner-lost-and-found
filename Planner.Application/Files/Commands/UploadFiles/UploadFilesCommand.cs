using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Files.Commands
{
	public class UploadFilesDataRequest
	{
		public string SanitizedFileName { get; set; }
	}

	public class TemporaryUploadedFileDetails
	{
		public string FileName { get; set; }
		public string ImageUrl { get; set; }
		public bool HasError { get; set; }
		public string Message { get; set; }
		public bool AlreadyExists { get; set; }
	}

	public class UploadFilesCommand : IRequest<IEnumerable<TemporaryUploadedFileDetails>>
	{
		public List<IFormFile> Files { get; set; }
		public List<UploadFilesDataRequest> Data { get; set; }

		public class UploadFilesCommandHandler : IRequestHandler<UploadFilesCommand, IEnumerable<TemporaryUploadedFileDetails>>, IAmWebApplicationHandler
		{
			private IDatabaseContext _databaseContext;
			private IFileService _fileService;

			public UploadFilesCommandHandler(IDatabaseContext databaseContext, IFileService fileService)
			{
				this._databaseContext = databaseContext;
				this._fileService = fileService;
			}

			public async Task<IEnumerable<TemporaryUploadedFileDetails>> Handle(UploadFilesCommand request, CancellationToken cancellationToken)
			{
				var uploadedFiles = await this.SaveUploadedFiles(request, cancellationToken);

				return uploadedFiles.Select(f => f.FileDetails).ToList();
			}

			private async Task<IEnumerable<UploadFileResult>> SaveUploadedFiles(UploadFilesCommand request, CancellationToken cancellationToken)
			{
				var uploadResults = new List<UploadFileResult>();

				if (request.Files.Count == 0)
				{
					return uploadResults;
				}

				//var imageStorageDirectoryPath = this._getTemporaryImageStorageDirectoryPath();
				var imageStorageDirectoryPath = this._fileService.GetTemporaryFileUploadPath();
				if (!Directory.Exists(imageStorageDirectoryPath))
				{
					Directory.CreateDirectory(imageStorageDirectoryPath);
				}

				foreach (var file in request.Files)
				{
					// Uploaded file has 0 bytes so there is nothing to save.
					if (file == null || file.Length == 0)
					{
						uploadResults.Add(this._getUploadFileResponse(file, null, null, $"Uploaded file has 0 bytes.", true, false));
						continue;
					}

					var fileData = request.Data.First(f => f.SanitizedFileName == file.FileName);
					// The file data is missing from the request
					if (fileData == null)
					{
						uploadResults.Add(this._getUploadFileResponse(file, fileData, null, "Uploaded file has no file data.", true, false));
						continue;
					}

					var imageStorageFilePath = this._fileService.GetTemporaryFileUploadPath(file.FileName);
					var fileMode = FileMode.CreateNew;

					// There already exists a file with the same name at the upload directory
					if (System.IO.File.Exists(imageStorageFilePath))
					{
						uploadResults.Add(this._getUploadFileResponse(file, fileData, null, "A file with the same name already exists.", false, true));
						continue;
					}

					using (var stream = new FileStream(imageStorageFilePath, fileMode))
					{
						await file.CopyToAsync(stream);
					}

					uploadResults.Add(this._getUploadFileResponse(file, fileData, System.IO.File.ReadAllBytes(imageStorageFilePath), "File uploaded.", false, false));
				}

				return uploadResults;
			}

			private UploadFileResult _getUploadFileResponse(IFormFile file, UploadFilesDataRequest fileData, byte[] uploadedFile, string message, bool isError, bool alreadyExists)
			{
				var isFileNull = file == null;
				var isFileDataNull = fileData == null;

				return new UploadFileResult
				{
					OriginalFileName = isFileNull ? "" : file.FileName,
					UploadedFile = uploadedFile,
					FileDetails = new TemporaryUploadedFileDetails
					{
						ImageUrl = isFileNull || isFileDataNull ? "" : this._fileService.GetTemporaryFileUrl(file.FileName),
						FileName = isFileNull ? "" : file.FileName,
						HasError = isError,
						Message = message,
						AlreadyExists = alreadyExists
					}
				};
			}
		}
	}

	public class UploadFileResult
	{
		public TemporaryUploadedFileDetails FileDetails { get; set; }
		public string OriginalFileName { get; set; }
		public byte[] UploadedFile { get; set; }
	}
}
