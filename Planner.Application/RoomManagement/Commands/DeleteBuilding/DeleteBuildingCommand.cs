using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Commands.DeleteBuilding
{
	public class DeleteBuildingCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class DeleteBuildingCommandHandler : IRequestHandler<DeleteBuildingCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public DeleteBuildingCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(DeleteBuildingCommand request, CancellationToken cancellationToken)
		{
			var building = await this._databaseContext.Buildings
				.Include(b => b.Rooms)
				.Include(b => b.Floors)
				.Where(b => b.Id == request.Id)
				.FirstOrDefaultAsync();

			if (building.Floors.Count() > 0)
			{
				return new ProcessResponse
				{
					HasError = true,
					Message = "Can't delete building with existing floors."
				};
			}
			if (building.Rooms.Count() > 0)
			{
				return new ProcessResponse
				{
					HasError = true,
					Message = "Can't delete building with existing rooms."
				};
			}

			this._databaseContext.Buildings.Remove(building);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				IsSuccess = true,
				Message = "Building deleted."
			};
		}
	}
}
