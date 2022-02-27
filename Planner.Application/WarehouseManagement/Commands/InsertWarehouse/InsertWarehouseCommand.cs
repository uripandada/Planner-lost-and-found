using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.WarehouseManagement.Commands.InsertWarehouse
{
	public class SaveWarehouseRequest
	{
		public string Name { get; set; }
		public bool IsCentralWarehouse { get; set; }
		public string HotelId { get; set; }
		public Guid? FloorId { get; set; }
	}

	public class InsertWarehouseCommand : SaveWarehouseRequest, IRequest<ProcessResponse<Guid>>
	{
	}

	public class InsertWarehouseCommandHandler : IRequestHandler<InsertWarehouseCommand, ProcessResponse<Guid>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly Guid _userId;

		public InsertWarehouseCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._userId = httpContextAccessor.UserId();
		}

		public async Task<ProcessResponse<Guid>> Handle(InsertWarehouseCommand request, CancellationToken cancellationToken)
		{
			var warehouse = new Warehouse
			{
				Id = Guid.NewGuid(),
				Name = request.Name,
				HotelId = request.HotelId,
				FloorId = request.FloorId,
				IsCentral = request.IsCentralWarehouse,
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Hotel = null,
				CreatedBy = null,
				Floor = null,
				ModifiedBy = null,
			};

			await this._databaseContext.Warehouses.AddAsync(warehouse, cancellationToken);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<Guid>
			{
				Data = warehouse.Id,
				HasError = false,
				IsSuccess = true,
				Message = "Warehouse inserted"
			};
		}
	}
}
