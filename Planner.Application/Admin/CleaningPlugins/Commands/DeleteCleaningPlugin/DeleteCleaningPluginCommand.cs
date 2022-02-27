using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.CleaningPlugins.Commands.DeleteCleaningPlugin
{
	public class DeleteCleaningPluginCommand : IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}
	public class DeleteCleaningPluginCommandHandler : IRequestHandler<DeleteCleaningPluginCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public DeleteCleaningPluginCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<ProcessResponse> Handle(DeleteCleaningPluginCommand request, CancellationToken cancellationToken)
		{
			var existingPlugin = await this._databaseContext.CleaningPlugins.Where(cp => cp.Id == request.Id).FirstOrDefaultAsync();

			if (existingPlugin == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find cleaning plugin to update."
				};
			}

			this._databaseContext.CleaningPlugins.Remove(existingPlugin);
			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Cleaning plugin deleted."
			};
		}
	}
}
