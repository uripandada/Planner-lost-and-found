using MediatR;
using Microsoft.AspNetCore.Http;
using Planner.Application.Interfaces;
using Planner.Common.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.RoomCategoryManagement.Queries.GetRoomCategoryDetails
{
	public class RoomCategoryDetailsViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public bool IsPrivate { get; set; }
		public bool IsDefaultForReservationSync { get; set; }
		public bool IsSystemDefaultForReservationSync { get; set; }
	}

	public class GetRoomCategoryDetailsQuery : IRequest<RoomCategoryDetailsViewModel>
	{
		public Guid Id { get; set; }
	}

	public class GetRoomCategoryDetailsQueryHandler : IRequestHandler<GetRoomCategoryDetailsQuery, RoomCategoryDetailsViewModel>, IAmWebApplicationHandler
	{
		private readonly IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetRoomCategoryDetailsQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<RoomCategoryDetailsViewModel> Handle(GetRoomCategoryDetailsQuery request, CancellationToken cancellationToken)
		{
			var category = await this._databaseContext.RoomCategorys.FindAsync(request.Id);

			return new RoomCategoryDetailsViewModel
			{
				IsSystemDefaultForReservationSync = category.IsSystemDefaultForReservationSync,
				IsDefaultForReservationSync = category.IsDefaultForReservationSync,
				IsPrivate = category.IsPrivate,
				Id = category.Id,
				Name = category.Name,
			};
		}
	}
}
