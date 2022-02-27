using MediatR;
using Planner.Application.Admin.HotelGroup.Commands.InsertHotelGroupHotel;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.Admin.HotelGroup.Commands.UpdateHotelGroupHotel
{
	public class UpdateHotelGroupHotelCommand : SaveHotelGroupHotelData, IRequest<ProcessResponse>
	{
	}

	public class UpdateHotelGroupHotelCommandHandler : IRequestHandler<UpdateHotelGroupHotelCommand, ProcessResponse>, IAmAdminApplicationHandler
	{
		private IDatabaseContext _databaseContext;

		public UpdateHotelGroupHotelCommandHandler(IDatabaseContext databaseContext)
		{
			this._databaseContext = databaseContext;
		}

		public async Task<ProcessResponse> Handle(UpdateHotelGroupHotelCommand request, CancellationToken cancellationToken)
		{
			var ianaTimeZoneId = TimeZoneConverter.TZConvert.WindowsToIana(request.WindowsTimeZoneId);

			var hotel = await this._databaseContext.Hotels.FindAsync(request.Id);

			if(hotel == null)
			{
				return new ProcessResponse
				{
					HasError = true,
					IsSuccess = false,
					Message = "Unable to find hotel to update"
				};
			}

			hotel.IanaTimeZoneId = ianaTimeZoneId;
			hotel.WindowsTimeZoneId = request.WindowsTimeZoneId;
			hotel.ModifiedAt = DateTime.UtcNow;
			hotel.Name = request.Name;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new ProcessResponse
			{
				HasError = false,
				IsSuccess = true,
				Message = "Hotel updated"
			};
		}
	}
}
