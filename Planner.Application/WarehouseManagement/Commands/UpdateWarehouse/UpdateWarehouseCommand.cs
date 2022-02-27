using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Application.WarehouseManagement.Commands.InsertWarehouse;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.WarehouseManagement.Commands.UpdateWarehouse
{
	public class UpdateWarehouseCommand: SaveWarehouseRequest, IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly Guid _userId;

		public UpdateWarehouseCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
		{
			this._databaseContext = databaseContext;
			this._httpContextAccessor = httpContextAccessor;
			this._userId = httpContextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
		{
			var warehouse = await this._databaseContext.Warehouses.FindAsync(request.Id);

			if(warehouse == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find warehouse to update."
				};
			}

			warehouse.Name = request.Name;
			warehouse.IsCentral = request.IsCentralWarehouse;
			warehouse.ModifiedAt = DateTime.UtcNow;
			warehouse.ModifiedById = this._userId;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Warehouse updated."
			};
		}
	}

}
