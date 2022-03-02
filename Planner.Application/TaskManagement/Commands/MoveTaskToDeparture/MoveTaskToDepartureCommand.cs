using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Application.TaskManagement.Commands.InsertTaskConfiguration;
using Planner.Common.Data;
using Planner.Common.Enums;
using Planner.Common.Extensions;
using Planner.Common.Helpers;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Planner.Application.TaskManagement.Commands.MoveTaskToDeparture
{
	public class MoveTaskToDepartureCommand: IRequest<ProcessResponseSimple>
	{
		public Guid TaskId { get; set; }
	}
	public class MoveTaskToDepartureCommandHandler: IRequestHandler<MoveTaskToDepartureCommand, ProcessResponseSimple>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly IFileService _fileService;
		private readonly ISystemTaskGenerator _systemTaskGenerator;
		private readonly ISystemEventsService _systemEventsService;
		private readonly Guid _userId;
		private readonly Guid _hotelGroupId;

		public MoveTaskToDepartureCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor, IFileService fileService, ISystemTaskGenerator systemTaskGenerator, ISystemEventsService systemEventsService)
		{
			this._databaseContext = databaseContext;
			this._fileService = fileService;
			this._systemTaskGenerator = systemTaskGenerator;
			this._userId = contextAccessor.UserId();
			this._hotelGroupId = contextAccessor.HotelGroupId();
			this._systemEventsService = systemEventsService;
		}

		public async Task<ProcessResponseSimple> Handle(MoveTaskToDepartureCommand request, CancellationToken cancellationToken)
		{
			var task = await this._databaseContext.SystemTasks
				.Include(st => st.ToRoom)
				.Where(t => t.Id == request.TaskId)
				.FirstOrDefaultAsync();

			if(task == null)
			{
				return new ProcessResponseSimple
				{
					 IsSuccess = false,
					 Message = "Unable to find task.",
				};
			}

			if (task.ToReservationId.IsNotNull())
			{
				// load room by reservation id
			}
			else if (task.ToRoomId.HasValue)
			{
				// load room by room id
			}

			throw new NotImplementedException();
		}
	}
}
