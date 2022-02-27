using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Infrastructure
{
	public class FileService : IFileService
	{
		private readonly Guid _userId;
		private readonly string _webRootPath;

		private readonly string FILE_STORAGE_ROOT_DIRECTORY = "uploaded-files";
		private readonly string FILE_STORAGE_TEMPORARY_DIRECTORY = "temporary-uploads";

		public FileService(IHttpContextAccessor contextAccessor, IHostingEnvironment hostingEnvironment)
		{
			this._userId = contextAccessor.UserId();
			this._webRootPath = hostingEnvironment.WebRootPath;
		}

		public string GetAssetFileStoragePath(Guid assetId, string fileName = null)
		{
			var path = this.GetFileStorageRootPath();

			path = Path.Combine(path, "assets", assetId.ToString());

			if (fileName.IsNotNull())
			{
				path = Path.Combine(path, fileName);
			}

			return path;
		}

		public string GetTaskConfigurationFileStoragePath(Guid taskConfigurationId, string fileName = null)
		{
			var path = this.GetFileStorageRootPath();

			path = Path.Combine(path, "task-configurations", taskConfigurationId.ToString());

			if (fileName.IsNotNull())
			{
				path = Path.Combine(path, fileName);
			}

			return path;
		}

		public string GetAvatarFileStoragePath(Guid userId, string fileName = null)
		{
			var path = this.GetFileStorageRootPath();

			path = Path.Combine(path, "avatars", userId.ToString());

			if (fileName.IsNotNull())
			{
				path = Path.Combine(path, fileName);
			}

			return path;
		}

		public string GetAssetQrCodeFileStoragePath(Guid assetId, string fileName = null)
		{
			var path = this.GetFileStorageRootPath();

			path = Path.Combine(this.GetAssetFileStoragePath(assetId, null), "qr-codes");

			if (fileName.IsNotNull())
			{
				path = Path.Combine(path, fileName);
			}

			return path;
		}

		public string GetAssetFileUrl(Guid assetId, string fileName)
		{
			return $"/{this.FILE_STORAGE_ROOT_DIRECTORY}/assets/{assetId.ToString()}/{fileName}";
		}

		public string GetAssetQrCodeFileUrl(Guid assetId, string fileName)
		{
			return $"/{this.FILE_STORAGE_ROOT_DIRECTORY}/assets/{assetId.ToString()}/qr-codes/{fileName}";
		}

		public string GetAvatarFileUrl(Guid userId, string fileName)
		{
			return $"/{this.FILE_STORAGE_ROOT_DIRECTORY}/avatars/{userId.ToString()}/{fileName}";
		}

		public string GetTaskConfigurationFileUrl(Guid taskConfigurationId, string fileName)
		{
			return $"/{this.FILE_STORAGE_ROOT_DIRECTORY}/task-configurations/{taskConfigurationId.ToString()}/{fileName}";
		}

		public string GetFileStorageRootPath(string fileName = null)
		{
			var path = Path.Combine(this._webRootPath, this.FILE_STORAGE_ROOT_DIRECTORY);
			if (fileName.IsNotNull())
			{
				path = Path.Combine(path, fileName);
			}

			return path;
		}

		public string GetTemporaryFileUploadPath(string fileName = null)
		{
			var path = Path.Combine(this._webRootPath, this.FILE_STORAGE_TEMPORARY_DIRECTORY, this._userId.ToString());
			if (fileName.IsNotNull())
			{
				path = Path.Combine(path, fileName);
			}

			return path;
		}

		public string GetRootFileUrl(string fileName)
		{
			return $"/{this.FILE_STORAGE_ROOT_DIRECTORY}/{this._userId}/{fileName}";
		}

		public string GetTemporaryFileUrl(string fileName)
		{
			return $"/{this.FILE_STORAGE_TEMPORARY_DIRECTORY}/{this._userId}/{fileName}";
		}

		public async Task<ProcessFileResult> ProcessNewAssetFile(Guid assetId, string fileName)
		{
			var fileTypeData = this.DetermineFileType(fileName);
			var sourcePath = this.GetTemporaryFileUploadPath(fileName);
			var destinationPath = this.GetAssetFileStoragePath(assetId);

			if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);

			destinationPath = this.GetAssetFileStoragePath(assetId, fileName);

			if (fileTypeData.FileType == FileTypes.IMAGE)
			{
				var maxWidth = 200;
				var maxHeight = 200;

				var width = maxWidth;
				var height = maxHeight;

				using (var image = Image.FromFile(sourcePath))
				{
					if (image.Width > image.Height)
					{
						height = (int)Math.Floor(image.Height * ((decimal)width / image.Width));
					}
					else
					{
						width = (int)Math.Floor(image.Width * ((decimal)height / image.Height));
					}

					using (var bitmap = ImageService.ResizeImage(image, width, height))
					{
						bitmap.Save(destinationPath);
					}
				}
			}
			else
			{
				File.Copy(sourcePath, destinationPath, true);
			}

			return new ProcessFileResult
			{
				Extension = fileTypeData.Extension,
				FileType = fileTypeData.FileType,
				FilePath = destinationPath
			};
		}

		public async Task<ProcessFileResult> ProcessNewQrCodeFile(Guid assetId, string fileName)
		{
			var fileTypeData = this.DetermineFileType(fileName);
			var sourcePath = this.GetTemporaryFileUploadPath(fileName);
			var destinationPath = this.GetAssetQrCodeFileStoragePath(assetId);

			if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);

			destinationPath = this.GetAssetFileStoragePath(assetId, fileName);

			if (fileTypeData.FileType == FileTypes.IMAGE)
			{
				var maxWidth = 400;
				var maxHeight = 400;

				var width = maxWidth;
				var height = maxHeight;

				using (var image = Image.FromFile(sourcePath))
				{
					if (image.Width > image.Height)
					{
						height = (int)Math.Floor(image.Height * ((decimal)width / image.Width));
					}
					else
					{
						width = (int)Math.Floor(image.Width * ((decimal)height / image.Height));
					}

					using (var bitmap = ImageService.ResizeImage(image, width, height))
					{
						bitmap.Save(destinationPath);
					}
				}
			}
			else
			{
				File.Copy(sourcePath, destinationPath, true);
			}

			return new ProcessFileResult
			{
				Extension = fileTypeData.Extension,
				FileType = fileTypeData.FileType,
				FilePath = destinationPath
			};
		}

		public FileTypeData DetermineFileType(string fileNameWithExtension)
		{
			var file = new FileInfo(fileNameWithExtension);
			var imageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".bmp", ".tiff", ".png" };
			var isImage = imageExtensions.Contains(file.Extension.ToLower());
			return new FileTypeData
			{
				FileType = isImage ? FileTypes.IMAGE : FileTypes.DOCUMENT,
				Extension = file.Extension
			};
		}


		public async Task<SaveAvatarResult> SaveAvatar(Guid userId, byte[] file, string fileName)
		{
			var directoryPath = this.GetAvatarFileStoragePath(userId);
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			var filePath = this.GetAvatarFileStoragePath(userId, fileName);
			await File.WriteAllBytesAsync(filePath, file);

			var url = this.GetAvatarFileUrl(userId, fileName);
			return new SaveAvatarResult
			{
				FileUrl = url
			};
		}

        public string GetLostAndFoundStoragePath(Guid lostAndFoundId, string fileName = null)
        {
			var path = this.GetFileStorageRootPath();

			path = Path.Combine(path, "lostAndFound", lostAndFoundId.ToString());

			if (fileName.IsNotNull())
			{
				path = Path.Combine(path, fileName);
			}

			return path;
		}

        public string GetOnGuardStoragePath(Guid onGuardId, string fileName = null)
        {
			var path = this.GetFileStorageRootPath();

			path = Path.Combine(path, "onGuard", onGuardId.ToString());

			if (fileName.IsNotNull())
			{
				path = Path.Combine(path, fileName);
			}

			return path;
		}
    }

	public class ImageService
	{
		/// <summary>
		/// Resize the image to the specified width and height.
		/// </summary>
		/// <param name="image">The image to resize.</param>
		/// <param name="width">The width to resize to.</param>
		/// <param name="height">The height to resize to.</param>
		/// <returns>The resized image.</returns>
		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
	}
}
