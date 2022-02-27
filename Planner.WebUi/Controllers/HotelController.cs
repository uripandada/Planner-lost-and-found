using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Hotel.Commands.SaveHotelSettings;
using Planner.Application.Hotel.Commands.UploadDistanceMatrixFile;
using Planner.Application.Hotel.Queries.GetHotelSettings;
using Planner.Application.Hotel.Queries.GetList;
using Planner.Common.Data;
using Planner.Domain.Values;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class HotelController : BaseController
	{
		[HttpPost]
		public async Task<HotelItemData[]> GetList(GetListOfHotelsQuery query)
		{
			return await this.Mediator.Send(query);
		}

		[HttpGet]
		public async Task<ProcessResponse<HotelSettingsData>> GetHotelSettings(string hotelId)
		{
			return await this.Mediator.Send(new GetHotelSettingsQuery { HotelId = hotelId });
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.HotelSettings)]
		public async Task<ProcessResponse<string>> SaveHotelSettings([FromBody] SaveHotelSettingsCommand request)
		{
			return await this.Mediator.Send(request);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.HotelSettings)]
		public async Task<ProcessResponse> UploadDistanceMatrix(IFormFile file, string hotelId, DistanceMatrixType type)
		{
			return await this.Mediator.Send(new UploadDistanceMatrixFileCommand() { File = file, HotelId = hotelId, Type = type });
		}
	}
}
