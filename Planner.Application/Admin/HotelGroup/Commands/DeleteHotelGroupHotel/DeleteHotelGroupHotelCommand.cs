using MediatR;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Commands.DeleteHotelGroupHotel
{
	public class DeleteHotelGroupHotelCommand : IRequest<ProcessResponse>
	{
		public string Id { get; set; }
	}
	public class DeleteHotelGroupHotelCommandHandler : IRequestHandler<DeleteHotelGroupHotelCommand, ProcessResponse>
	{
		private IDatabaseContext _databaseContext;
		public DeleteHotelGroupHotelCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public Task<ProcessResponse> Handle(DeleteHotelGroupHotelCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
