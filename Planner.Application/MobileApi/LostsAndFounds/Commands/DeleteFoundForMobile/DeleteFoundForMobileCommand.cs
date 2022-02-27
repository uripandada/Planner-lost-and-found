using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Infrastructure;
using Planner.Application.Interfaces;
using Planner.Common.Data;
using Planner.Common.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.LostsAndFounds.Commands.DeleteFoundForMobile
{
	public class DeleteFoundForMobileCommand: IRequest<SimpleProcessResponse>
	{
		public string HotelId { get; set; }
		public Guid Id { get; set; }
	}

	public class DeleteFoundForMobileCommandHandler : IRequestHandler<DeleteFoundForMobileCommand, SimpleProcessResponse>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public DeleteFoundForMobileCommandHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<SimpleProcessResponse> Handle(DeleteFoundForMobileCommand request, CancellationToken cancellationToken)
		{
			var dateProvider = new HotelLocalDateProvider();
			var dateTime = await dateProvider.GetHotelCurrentLocalDate(this._databaseContext, request.HotelId, true);

			var foundItem = await this._databaseContext.LostAndFounds.Where(lf => lf.Id == request.Id).FirstOrDefaultAsync();

			if (foundItem == null)
			{
				return new SimpleProcessResponse
				{
					Success = false
				};
			}

			foundItem.IsDeleted = true;
			foundItem.ModifiedById = this._userId;
			foundItem.ModifiedAt = dateTime;
			foundItem.Status = Domain.Values.LostAndFoundStatus.Deleted;
			foundItem.RccStatus = Domain.Values.RccLostAndFoundStatus.DELETED;

			await this._databaseContext.SaveChangesAsync(cancellationToken);

			return new SimpleProcessResponse
			{
				Success = true
			};
		}
	}
}
