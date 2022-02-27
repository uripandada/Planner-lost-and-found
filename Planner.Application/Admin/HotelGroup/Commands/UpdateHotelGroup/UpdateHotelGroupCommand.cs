using MediatR;
using Planner.Application.Admin.HotelGroup.Commands.InsertHotelGroup;
using Planner.Application.Admin.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Commands.UpdateHotelGroup
{
	public class UpdateHotelGroupCommand : SaveHotelGroup, IRequest<ProcessResponse>
	{
		public Guid Id { get; set; }
	}

	public class UpdateHotelGroupCommandHandler : IRequestHandler<UpdateHotelGroupCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private IMasterDatabaseContext _masterDatabaseContext;

		public UpdateHotelGroupCommandHandler(IMasterDatabaseContext masterDatabaseContext)
		{
			this._masterDatabaseContext = masterDatabaseContext;
		}

		public async Task<ProcessResponse> Handle(UpdateHotelGroupCommand request, CancellationToken cancellationToken)
		{
			var tenant = await this._masterDatabaseContext.HotelGroupTenants.FindAsync(request.Id);

			if(tenant == null)
			{
				return new ProcessResponse
				{
					IsSuccess = false,
					HasError = true,
					Message = "Unable to find hotel group to update"
				};
			}

			tenant.Key = request.Key;
			tenant.Name = request.Name;

			await this._masterDatabaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				IsSuccess = true,
				HasError = false,
				Message = "Hotel group updated"
			};
		}
	}
}
