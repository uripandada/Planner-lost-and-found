using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Application.Interfaces
{
	public enum FileTypes
	{
		IMAGE,
		DOCUMENT,
		
	}

	public class FileTypeData
	{
		public FileTypes FileType { get; set; }
		public string Extension { get; set; }
	}

	public interface IFileService
	{
		string GetAssetFileStoragePath(Guid assetId, string fileName = null);
		string GetTaskConfigurationFileStoragePath(Guid taskConfigurationId, string fileName = null);
		string GetAssetFileUrl(Guid assetId, string fileName);
		string GetTaskConfigurationFileUrl(Guid taskConfigurationId, string fileName);
		string GetFileStorageRootPath(string fileName = null);
		string GetTemporaryFileUploadPath(string fileName = null);
		string GetRootFileUrl(string fileName);
		string GetTemporaryFileUrl(string fileName);
		string GetLostAndFoundStoragePath(Guid lostAndFoundId, string fileName = null);
		string GetOnGuardStoragePath(Guid onGuardId, string fileName = null);
		FileTypeData DetermineFileType(string fileNameWithExtension);
		Task<ProcessFileResult> ProcessNewAssetFile(Guid assetId, string fileName);
		Task<ProcessFileResult> ProcessNewQrCodeFile(Guid assetId, string fileName);
		Task<SaveAvatarResult> SaveAvatar(Guid userId, byte[] file, string fileName);
	}

	public class ProcessFileResult
	{
		public string Extension { get; set; }
		public FileTypes FileType { get; set; }
		public string FilePath { get; set; }
	}

	public class SaveAvatarResult
	{
		public string FileUrl { get; set; }
	}

}
