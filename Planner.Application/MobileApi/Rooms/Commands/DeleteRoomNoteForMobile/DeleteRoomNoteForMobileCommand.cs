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

namespace Planner.Application.MobileApi.Rooms.Commands.DeleteRoomNoteForMobile
{
	public class DeleteRoomNoteForMobileCommand: IRequest<SimpleProcessResponse>
	{
		public string HotelId { get; set; }
		public Guid RoomId { get; set; }
	}

	public class DeleteRoomNoteForMobileCommandHandler : IRequestHandler<DeleteRoomNoteForMobileCommand, SimpleProcessResponse>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public DeleteRoomNoteForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<SimpleProcessResponse> Handle(DeleteRoomNoteForMobileCommand request, CancellationToken cancellationToken)
		{
			using(var transaction = await this._databaseContext.Database.BeginTransactionAsync(cancellationToken))
			{
				var roomNotes = await this._databaseContext.RoomNotes.Where(rn => rn.RoomId == request.RoomId && rn.Room.HotelId == request.HotelId).ToArrayAsync();
				if (roomNotes.Any())
				{
					foreach(var roomNote in roomNotes)
					{
						roomNote.IsArchived = true;
					}

					await this._databaseContext.SaveChangesAsync(cancellationToken);
					await transaction.CommitAsync(cancellationToken);
				}
			}

			return new SimpleProcessResponse
			{
				Success = true
			};
		}
	}
}
