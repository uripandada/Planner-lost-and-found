using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.ExternalApi.Rooms.Commands.ExternalDndOn
{
	public class ExternalDndOnCommand : IRequest<ProcessResponse>
	{
		/// <summary>
		/// You can choose to set either hotelGroupId or hotelGroupKey
		/// </summary>
		public Guid? HotelGroupId { get; set; }
		/// <summary>
		/// You can choose to set either hotelGroupId or hotelGroupKey
		/// </summary>
		public string HotelGroupKey { get; set; }
		public string HotelId { get; set; }
		/// <summary>
		/// You can choose to set either roomId or roomName
		/// </summary>
		public Guid? RoomId { get; set; }
		/// <summary>
		/// You can choose to set either roomId or roomName
		/// </summary>
		public string RoomName { get; set; }
		/// <summary>
		/// You can choose to set either roomBedId or roomBedName
		/// </summary>
		public Guid? RoomBedId { get; set; }
		/// <summary>
		/// You can choose to set either roomBedId or roomBedName
		/// </summary>
		public string RoomBedName { get; set; }
		/// <summary>
		/// A string description to identify who made the request
		/// </summary>
		public string RequestedBy { get; set; }

	}
	public class ExternalDndOnCommandHandler : IRequestHandler<ExternalDndOnCommand, ProcessResponse>
	{
		public async Task<ProcessResponse> Handle(ExternalDndOnCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
