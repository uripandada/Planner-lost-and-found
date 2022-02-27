using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Commands.DeleteRoom
{
	public class DeleteRoomCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public DeleteRoomCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
		{
			var room = await this._databaseContext.Rooms
				.Where(r => r.Id == request.Id)
				.FirstOrDefaultAsync();

			this._databaseContext.Rooms.Remove(room);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				IsSuccess = true,
				Message = "Room deleted."
			};
		}
	}
}
