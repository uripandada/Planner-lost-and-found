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

namespace Planner.Application.WarehouseManagement.Commands.DispatchAssetFromWarehouse
{
	public class DispatchAssetFromWarehouseCommand : IRequest<ProcessResponse>
	{
		public Guid AssetId { get; set; }
		public int Quantity { get; set; }
		public Guid WarehouseId { get; set; }
		public string Note { get; set; }
	}

	public class DispatchAssetFromWarehouseCommandHandler : IRequestHandler<DispatchAssetFromWarehouseCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly Guid _userId;

		public DispatchAssetFromWarehouseCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._userId = httpContextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(DispatchAssetFromWarehouseCommand request, CancellationToken cancellationToken)
		{
			var asset = await this._databaseContext.Assets.FindAsync(request.AssetId);

			if (!asset.IsBulk)
			{
				// if the asset is not bulk, it is important to check if the asset can be received - the total quantity must be 0
				var availabilities = await this._databaseContext.WarehouseAssetAvailabilities.Where(a => a.AssetId == request.AssetId).ToArrayAsync();
				var sumAvailableQuantities = availabilities.Sum(a => a.Quantity);
				var sumReservedQuantities = availabilities.Sum(a => a.ReservedQuantity);

				if (sumAvailableQuantities == 0)
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to dispatch an individual asset with 0 quantity.",
					};
				}

				if(sumAvailableQuantities > 1)
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to dispatch individual asset with more than 1 quantity. Data inconsistency error. Please contact support.",
					};
				}

				if(sumReservedQuantities > 0)
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to dispatch a reserved individual asset.",
					};
				}

				var usages = await this._databaseContext.RoomAssetUsages.Where(a => a.AssetId == request.AssetId).ToArrayAsync();
				var sumInUseQuantities = usages.Sum(a => a.Quantity);
				if (sumInUseQuantities > 0)
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to dispatch a used individual asset.",
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
				TypeKey = WarehouseDocumentType.DISPATCH_MANUAL.ToString(),
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
				// THE ASSET CAN'T BE DISPATCHED WITHOUT AVAILABILITY!

				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to dispatch an asset without availability.",
				};
			}
			else
			{
				//if (!asset.IsBulk && (availability.Quantity != 1))
				//{
				//	return new ProcessResponse
				//	{
				//		HasError = true,
				//		IsSuccess = false,
				//		Message = "You can't receive individual asset more than once.",
				//	};
				//}

				var availableQuantityBefore = availability.Quantity;
				var availableQuantityAfter = availability.Quantity - quantity;

				if(availableQuantityAfter < 0)
				{
					return new ProcessResponse
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to dispatch more assets than available.",
					};
				}

				warehouseDocument.AvailableQuantityBeforeChange = availableQuantityBefore;
				warehouseDocument.AvailableQuantityChange = (-1) * quantity;

				availability.Quantity = availableQuantityAfter;
			}

			await this._databaseContext.WarehouseDocuments.AddAsync(warehouseDocument, cancellationToken);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Asset dispatched."
			};
		}
	}
}
