using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Commands.UpdateFloor
{
	public class UpdateFloorResponse
	{
	}

	public class UpdateFloorCommand : IRequest<ProcessResponse<UpdateFloorResponse>>
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public int Number { get; set; }
		public int OrdinalNumber { get; set; }
		public Guid BuildingId { get; set; }
		public string HotelId { get; set; }
	}

	public class UpdateFloorCommandHandler : IRequestHandler<UpdateFloorCommand, ProcessResponse<UpdateFloorResponse>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public UpdateFloorCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<UpdateFloorResponse>> Handle(UpdateFloorCommand request, CancellationToken cancellationToken)
		{
			var response = new UpdateFloorResponse();

			using (var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				var floor = await this._databaseContext.Floors.Include(r => r.Rooms).Where(b => b.Id == request.Id).FirstOrDefaultAsync();

				if (floor == null)
				{
					return new ProcessResponse<UpdateFloorResponse>
					{
						HasError = true,
						IsSuccess = false,
						Message = "Unable to find floor to update."
					};
				}

				floor.ModifiedAt = DateTime.UtcNow;
				floor.ModifiedById = this._userId;
				floor.Name = request.Name;
				floor.OrdinalNumber = request.OrdinalNumber;
				floor.Number = request.Number;
				floor.BuildingId = request.BuildingId;

				await this._databaseContext.SaveChangesAsync(cancellationToken);
				await transaction.CommitAsync();
			}

			return new ProcessResponse<UpdateFloorResponse>
			{
				Data = response,
				HasError = false,
				IsSuccess = true,
				Message = "Floor updated."
			};
		}
	}
}
