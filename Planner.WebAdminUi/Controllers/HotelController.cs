using Microsoft.AspNetCore.Mvc;
using Planner.Application.Admin.Hotel.Queries.GetHotelHierarchy;
using Planner.Application.Admin.Hotel.Queries.GetHotelRoomCategories;
using Planner.Application.Admin.Hotel.Queries.GetHotelRooms;
using Planner.Application.Admin.HotelGroup.Queries.GetHotelGroups;
using Planner.Application.Admin.HotelGroup.Queries.GetHotels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.Controllers
{
	public class HotelController : BaseController
	{
		[HttpPost]
		public async Task<HotelGridData[]> GetHotels(GetHotelsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<IEnumerable<HotelRoomCategoryData>> GetHotelRoomCategories(GetHotelRoomCategoriesQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<HotelHierarchyData> GetHotelHierarchy(GetHotelHierarchyQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpPost]
		public async Task<IEnumerable<HotelRoomData>> GetHotelRooms(GetHotelRoomsQuery query)
		{
			return await this.Mediator.Send(query);
		}
	}
}
