using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Planner.Application.MobileApi.Assets.Queries.GetListOfAssetsForMobile;
using System.Collections.Generic;
using Planner.Application.MobileApi.Assets.Queries.GetListOfAssetActionsForMobile;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	public class AssetController : BaseMobileApiController
	{
		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<IEnumerable<MobileAsset>> GetListOfAssets([FromBody] GetListOfAssetsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost]
		public async Task<IEnumerable<MobileAssetAction>> GetListOfAssetActions([FromBody] GetListOfAssetActionsForMobileQuery request)
		{
			return await this.Mediator.Send(request);
		}
	}
}
