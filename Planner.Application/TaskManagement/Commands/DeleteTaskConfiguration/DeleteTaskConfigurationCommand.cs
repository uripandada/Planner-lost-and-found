using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.TaskManagement.Commands.DeleteTaskConfiguration
{
	public class DeleteTaskConfigurationCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class DeleteTaskConfigurationCommandHandler : IRequestHandler<DeleteTaskConfigurationCommand, ProcessResponse>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public DeleteTaskConfigurationCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public Task<ProcessResponse> Handle(DeleteTaskConfigurationCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
