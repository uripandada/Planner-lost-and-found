using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Commands.DeleteFloor
{
	public class DeleteFloorCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class DeleteFloorCommandHandler : IRequestHandler<DeleteFloorCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public DeleteFloorCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(DeleteFloorCommand request, CancellationToken cancellationToken)
		{
			var floor = await this._databaseContext.Floors
				.Include(b => b.Rooms)
				.Where(b => b.Id == request.Id)
				.FirstOrDefaultAsync();

			if (floor.Rooms.Count() > 0)
			{
				return new ProcessResponse
				{
					HasError = true,
					Message = "Can't delete floor with existing rooms."
				};
			}

			this._databaseContext.Floors.Remove(floor);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				IsSuccess = true,
				Message = "Floor deleted."
			};
		}
	}
}
