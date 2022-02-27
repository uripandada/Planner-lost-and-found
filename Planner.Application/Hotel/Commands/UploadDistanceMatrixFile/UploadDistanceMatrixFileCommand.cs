using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
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

namespace Planner.Application.Hotel.Commands.UploadDistanceMatrixFile
{
	public enum DistanceMatrixType
	{
		BUILDING,
		FLOOR,
	}

	public class UploadDistanceMatrixFileCommand : IRequest<ProcessResponse>
	{
		public DistanceMatrixType Type { get; set; } 
		public string HotelId { get; set; }
		public IFormFile File { get; set; }
	}

	public class UploadDistanceMatrixFileCommandHandler : IRequestHandler<UploadDistanceMatrixFileCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public UploadDistanceMatrixFileCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(UploadDistanceMatrixFileCommand request, CancellationToken cancellationToken)
		{
			var hotelSettings = await this._databaseContext.Settings.Where(s => s.HotelId == request.HotelId).FirstOrDefaultAsync();

			if (hotelSettings == null)
			{
				return new ProcessResponse
				{
					HasError = false,
					IsSuccess = false,
					Message = "Hotel settings don't exist."
				};
			}

			var fileTextContent = await this.ReadFormFileAsync(request.File);
			if(request.Type == DistanceMatrixType.BUILDING)
			{
				hotelSettings.BuildingsDistanceMatrix = fileTextContent;
			}
			else if(request.Type == DistanceMatrixType.FLOOR)
			{
				hotelSettings.LevelsDistanceMatrix = fileTextContent;
			}

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Distance matrix saved",
			};
		}

		private async Task<string> ReadFormFileAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return await Task.FromResult((string)null);
			}

			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				return await reader.ReadToEndAsync();
			}
		}
	}
}
