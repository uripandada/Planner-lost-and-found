using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.AssetManagement.Commands.InsertAssetGroup
{
	public class InsertAssetGroupCommand : IRequest<ProcessResponse<Guid>>
	{
		public Guid? ParentAssetGroupId { get; set; }
		public string Name { get; set; }
		public string TypeKey { get; set; }
	}

	public class InsertAssetGroupCommandHandler : IRequestHandler<InsertAssetGroupCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertAssetGroupCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertAssetGroupCommand request, CancellationToken cancellationToken)
		{
			var assetGroup = new Domain.Entities.AssetGroup
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				TypeKey = request.TypeKey,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Name = request.Name,
				ParentAssetGroupId = request.ParentAssetGroupId,
			};

			await this._databaseContext.AssetGroups.AddAsync(assetGroup, cancellationToken);
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
