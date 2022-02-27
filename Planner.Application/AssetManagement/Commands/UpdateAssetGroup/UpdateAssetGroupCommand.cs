using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Commands.UpdateAssetGroup
{
	public class UpdateAssetGroupCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string TypeKey { get; set; }
	}

	public class UpdateAssetGroupCommandHandler : IRequestHandler<UpdateAssetGroupCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateAssetGroupCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(UpdateAssetGroupCommand request, CancellationToken cancellationToken)
		{
			var assetGroup = await this._databaseContext.AssetGroups.FindAsync(request.Id);

			if(assetGroup == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find asset group to update.",
				};
			}

			assetGroup.ModifiedAt = DateTime.UtcNow;
			assetGroup.ModifiedById = this._userId;
			assetGroup.Name = request.Name;
			assetGroup.TypeKey = request.TypeKey;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = assetGroup.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Asset group created."
			};
		}
	}
}
