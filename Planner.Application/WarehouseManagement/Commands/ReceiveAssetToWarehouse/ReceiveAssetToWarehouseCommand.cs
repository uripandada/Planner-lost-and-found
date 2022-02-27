using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.WarehouseManagement.Commands.ReceiveAssetToWarehouse
{
	public class ReceiveAssetToWarehouseCommand : IRequest<ProcessResponse>
	{
		public Guid AssetId { get; set; }
		public int Quantity { get; set; }
		public Guid WarehouseId { get; set; }
		public string Note { get; set; }
	}

	public class ReceiveAssetToWarehouseCommandHandler : IRequestHandler<ReceiveAssetToWarehouseCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly Guid _userId;

		public ReceiveAssetToWarehouseCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._userId = httpContextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(ReceiveAssetToWarehouseCommand request, CancellationToken cancellationToken)
		{
			var asset = await this._databaseContext.Assets.FindAsync(request.AssetId);

			if (!asset.IsBulk)
			{
				// if the asset is not bulk, it is important to check if the asset can be received - the total quantity must be 0
				var availabilities = await this._databaseContext.WarehouseAssetAvailabilities.Where(a => a.AssetId == request.AssetId).ToArrayAsync();
				if(availabilities.Sum(a => a.Quantity + a.ReservedQuantity) > 0)
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "You can't receive individual asset more than once.",
					};
				}

				var usages = await this._databaseContext.RoomAssetUsages.Where(a => a.AssetId == request.AssetId).ToArrayAsync();
				if(usages.Sum(a => a.Quantity) > 0)
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "You can't receive individual asset more than once.",
					};
				}
			}

			var availability = await this._databaseContext.WarehouseAssetAvailabilities.Where(a => a.WarehouseId == request.WarehouseId && a.AssetId == request.AssetId).FirstOrDefaultAsync();
			var warehouseDocument = new Domain.Entities.WarehouseDocument
			{
				AssetId = request.AssetId,
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Id = Guid.NewGuid(),
				TypeKey = WarehouseDocumentType.RECEIPT_MANUAL_INIT.ToString(),
				WarehouseId = request.WarehouseId,
				AvailableQuantityChange = 0,
				AvailableQuantityBeforeChange = 0,
				ReservedQuantityBeforeChange = 0,
				ReservedQuantityChange = 0,
				Note = request.Note,
			};

			var quantity = asset.IsBulk ? request.Quantity : 1;

			if (availability == null)
			{
				availability = new Domain.Entities.WarehouseAssetAvailability
				{
					AssetId = request.AssetId,
					Id = Guid.NewGuid(),
					Quantity = quantity,
					ReservedQuantity = 0,
					WarehouseId = request.WarehouseId,
				};
				warehouseDocument.AvailableQuantityChange = quantity;

				await this._databaseContext.WarehouseAssetAvailabilities.AddAsync(availability, cancellationToken);
			}
			else
			{
				if(!asset.IsBulk && (availability.Quantity != 0 || availability.ReservedQuantity != 0))
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "You can't receive individual asset more than once.",
					};
				}

				warehouseDocument.AvailableQuantityBeforeChange = availability.Quantity;
				warehouseDocument.AvailableQuantityChange = quantity;

				availability.Quantity += quantity;
			}

			await this._databaseContext.WarehouseDocuments.AddAsync(warehouseDocument, cancellationToken);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Asset received."
			};
		}
	}
}
