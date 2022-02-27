using MediatR;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.CleaningPlugins.Commands.UpdatePluginOrder
{
	public class UpdatePluginOrderItem
	{
		public Guid PluginId { get; set; }
		public int OrdinalNumber { get; set; }
	}
	public class UpdatePluginOrderCommand : IRequest<ProcessResponse>
	{
		public string HotelId { get; set; }
		public IEnumerable<UpdatePluginOrderItem> OrderedPluginIds { get; set; }
	}

	public class UpdatePluginOrderCommandHandler : IRequestHandler<UpdatePluginOrderCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public UpdatePluginOrderCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(UpdatePluginOrderCommand request, CancellationToken cancellationToken)
		{
			var plugins = await this._databaseContext.CleaningPlugins.Where(cp => cp.HotelId == request.HotelId).ToArrayAsync();
			var orderMap = request.OrderedPluginIds.ToDictionary(i => i.PluginId, i => i.OrdinalNumber);

			foreach(var plugin in plugins)
			{
				plugin.OrdinalNumber = orderMap[plugin.Id];
			}

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Plugin order updated."
			};
		}
	}
}
