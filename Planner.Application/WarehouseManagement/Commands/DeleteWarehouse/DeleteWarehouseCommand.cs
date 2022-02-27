using MediatR;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.WarehouseManagement.Commands.DeleteWarehouse
{
	public class DeleteWarehouseCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}
	public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public DeleteWarehouseCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
		{
			var warehouse = await this._databaseContext.Warehouses.FindAsync(request.Id);

			if (warehouse == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find warehouse to delete."
				};
			}

			try
			{
				this._databaseContext.Warehouses.Remove(warehouse);
				await this._databaseContext.SaveChangesAsync(cancellationToken);

				return new ProcessResponse
				{
					HasError = false,
					IsSuccess = true,
					Message = "Warehouse deleted."
				};
			}
			catch(Exception ex)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Used warehouse can't be deleted."
				};
			}			
		}
	}
}
