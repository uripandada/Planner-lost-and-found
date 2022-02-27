using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomManagement.Commands.AssignRoomsToFloor
{
	public class AssignRoomsToFloorCommand : IRequest<ProcessResponse<RoomAssignmentResult[]>>
	{
		public Guid FloorId { get; set; }
		public Guid BuildingId { get; set; }
		public Guid CategoryId { get; set; }
		public IEnumerable<Guid> RoomIds { get; set; }
		public string RoomTypeKey { get; set; }
	}

	public class RoomAssignmentResult
	{
		public Guid RoomId { get; set; }
		public bool IsSuccess { get; set; }
		public string Message { get; set; }
	}

	public class AssignRoomsToFloorCommandHandler : IRequestHandler<AssignRoomsToFloorCommand, ProcessResponse<RoomAssignmentResult[]>>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;

		public AssignRoomsToFloorCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse<RoomAssignmentResult[]>> Handle(AssignRoomsToFloorCommand request, CancellationToken cancellationToken)
		{
			if(request.RoomIds == null || !request.RoomIds.Any())
			{
				return new ProcessResponse<RoomAssignmentResult[]>
				{
					Data = new RoomAssignmentResult[0],
					HasError = false,
					IsSuccess = true,
					Message = "Nothing changed."
				};
			}

			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync())
			{
				var rooms = await this._databaseContext.Rooms.Where(r => request.RoomIds.Contains(r.Id)).ToArrayAsync();
				var assignmentResults = new List<RoomAssignmentResult>();
				var dataChanged = false;

				foreach(var room in rooms)
				{
					if(room.BuildingId.HasValue || room.FloorId.HasValue) 
					{
						assignmentResults.Add(new RoomAssignmentResult { IsSuccess = false, RoomId = room.Id, Message = $"Room {room.Name} (ID: {room.Id}) is already assigned to the building ID {room.BuildingId} and the floor ID {room.FloorId}." });
						continue;
					}

					room.BuildingId = request.BuildingId;
					room.FloorId = request.FloorId;
					room.CategoryId = request.CategoryId;
					room.TypeKey = request.RoomTypeKey;

					assignmentResults.Add(new RoomAssignmentResult { IsSuccess = true, RoomId = room.Id, Message = $"Room {room.Name} (ID: {room.Id}) successfully assigned to the floor ID {request.FloorId}." });
					dataChanged = true;
				}

				if (dataChanged)
				{
					await this._databaseContext.SaveChangesAsync(cancellationToken);
					await transaction.CommitAsync(cancellationToken);
				}
				else
				{
					await transaction.RollbackAsync(cancellationToken);
				}

				return new ProcessResponse<RoomAssignmentResult[]>
				{
					Data = assignmentResults.ToArray(),
					IsSuccess = true,
					HasError = false,
					Message = "Rooms assigned."
				};
			}
		}
	}
}
