using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Files.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class FileController : BaseController
	{
		public async Task<IEnumerable<TemporaryUploadedFileDetails>> UploadFiles([FromForm] List<IFormFile> files, [FromForm(Name = "filesData")] List<UploadFilesDataRequest> filesData)
		{
			var command = new UploadFilesCommand
			{
				Data = filesData,
				Files = files
			};
			return await this.Mediator.Send(command);
		}

		/// <summary>
		/// This controller method is added to force NSwag to correctly generate the Typescript client.
		/// This call is not really used but must not be deleted until the NSwag client generator is fixed.
		/// Problematic NSwag version: 
		/// </summary>
		/// <param name="file"></param>
		/// <param name="fileData"></param>
		/// <returns></returns>
		public async Task<TemporaryUploadedFileDetails> UploadFile(IFormFile file, [FromForm(Name = "fileData")] UploadFilesDataRequest fileData)
		{
			var command = new UploadFilesCommand
			{
				Data = new List<UploadFilesDataRequest> { fileData },
				Files = new List<IFormFile> { file }
			};
			return (await this.Mediator.Send(command)).FirstOrDefault();
		}
	}
}
