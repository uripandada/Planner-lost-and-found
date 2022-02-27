using MediatR;
using Planner.Application.Admin.Interfaces;
using Planner.Common.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Commands.DeleteHotelGroup
{
	public class DeleteHotelGroupCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}
	public class DeleteHotelGroupCommandHandler : IRequestHandler<DeleteHotelGroupCommand, ProcessResponse>
	{
		private IMasterDatabaseContext _databaseContext;
		public DeleteHotelGroupCommandHandler(IMasterDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public Task<ProcessResponse> Handle(DeleteHotelGroupCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
