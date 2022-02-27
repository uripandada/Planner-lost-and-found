using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Commands.InsertFloor
{
	public class InsertFloorResponse
	{
		public Guid Id { get; set; }
	}

	public class InsertFloorCommand : IRequest<ProcessResponse<InsertFloorResponse>>
	{
		public string Name { get; set; }
		public int Number { get; set; }
		public int OrdinalNumber { get; set; }
		public Guid BuildingId { get; set; }
		public string HotelId { get; set; }
	}
	public class InsertFloorCommandHandler : IRequestHandler<InsertFloorCommand, ProcessResponse<InsertFloorResponse>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public InsertFloorCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse<InsertFloorResponse>> Handle(InsertFloorCommand request, CancellationToken cancellationToken)
		{
			var floor = new Floor
			{
				Id = Guid.NewGuid(),
				CreatedAt = DateTime.UtcNow,
				CreatedById = this._userId,
				HotelId = request.HotelId,
				ModifiedAt = DateTime.UtcNow,
				ModifiedById = this._userId,
				Name = request.Name,
				OrdinalNumber = request.OrdinalNumber,
				Number = request.Number,
				BuildingId = request.BuildingId
			};

			var response = new InsertFloorResponse
			{
				Id = floor.Id
			};

			await this._databaseContext.Floors.AddAsync(floor);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse<InsertFloorResponse>
			{
				Data = response,
				HasError = false,
				IsSuccess = true,
				Message = "Floor inserted."
			};
		}
	}
}
