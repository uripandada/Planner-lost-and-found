using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planner.Application.Admin.Interfaces;
using Planner.Application.Interfaces;
using Planner.Common;
using Planner.Common.Extensions;
using Planner.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planner.Application.MobileApi.RoomCategories.Queries.GetRoomCategoryDetailsForMobile
{
	public class MobileRoomCategoryDetails
	{
		public Guid Id { get; set; }
		public string Label { get; set; } = "NULL category"; // label: { type: String, required: true, validate: labelValidations },
		public int? Credits { get; set; } = 1; // credits: { type: Number, default: 1, required: false },
		public string HotelId { get; set; } = Guid.Empty.ToString(); // hotelId: { type: ObjectId, ref: 'Hotel', index: true },
		public bool IsPublic { get; set; } = false; // isPublic: { type: Boolean, default: false },
		public bool IsPrivate { get; set; } = false; // isPrivate: { type: Boolean, default: false },
		public bool IsOutside { get; set; } = false; // isOutside: { type: Boolean, default: false },
		public int CreditsStay { get; set; } = 0; // creditsStay: { isActive: false, value: 0 },
		public int CreditsDep { get; set; } = 0; // creditsDep: { isActive: false, value: 0 },
		public int CreditsCS { get; set; } = 0; // creditsCS: { isActive: false, value: 0 },
		public int CreditsLS { get; set; } = 0; // creditsLS: { isActive: false, value: 0 },
		public int CreditsOther { get; set; } = 0; // creditsOther: { isActive: false, value: 0 }
	}

	public class GetRoomCategoryDetailsForMobileQuery: IRequest<MobileRoomCategoryDetails>
	{
		public Guid Id { get; set; }
	}

	public class GetRoomCategoryDetailsForMobileQueryHandler : IRequestHandler<GetRoomCategoryDetailsForMobileQuery, MobileRoomCategoryDetails>, IAmWebApplicationHandler
	{
		private IDatabaseContext _databaseContext;
		private readonly Guid _userId;

		public GetRoomCategoryDetailsForMobileQueryHandler(IDatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
		{
			this._databaseContext = databaseContext;
			this._userId = contextAccessor.UserId();
		}

		public async Task<MobileRoomCategoryDetails> Handle(GetRoomCategoryDetailsForMobileQuery request, CancellationToken cancellationToken)
		{
			var c = await this._databaseContext.RoomCategorys.FindAsync(request.Id);
			if (c == null) throw new Exception("Unable to find room category details.");

			var defaultRoomCategory = new Shared.Models.RoomCategoryForMobile();

			return new MobileRoomCategoryDetails
			{
				Id = c.Id,
				Label = c.Name,
				IsPrivate = c.IsPrivate,
				IsPublic = c.IsPublic,
				HotelId = "", // Room categories are available to all hotels!
				Credits = defaultRoomCategory.Credits,
				CreditsCS = defaultRoomCategory.CreditsCS,
				CreditsDep = defaultRoomCategory.CreditsDep,
				CreditsLS = defaultRoomCategory.CreditsLS,
				CreditsOther = defaultRoomCategory.CreditsOther,
				CreditsStay = defaultRoomCategory.CreditsStay,
				IsOutside = defaultRoomCategory.IsOutside,
			};
		}
	}
}
