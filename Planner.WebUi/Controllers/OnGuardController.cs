using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.OnGuards.Commands.Insert;
using Planner.Application.OnGuards.Commands.Update;
using Planner.Application.OnGuards.Models;
using Planner.Application.OnGuards.Queries.GetById;
using Planner.Application.OnGuards.Queries.GetList;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class OnGuardController : BaseController
	{
		[HttpPost]
		public async Task<ActionResult<ProcessResponse<PageOf<OnGuardListItem>>>> GetList(GetOnGuardListQuery query)
		{
			var result = await this.Mediator.Send(query);
			return this.Ok(result);
		}

		[HttpGet]
		public async Task<ActionResult<ProcessResponse<OnGuardModel>>> GetById(string onGuardId)
		{
			var result = await this.Mediator.Send(new GetOnGuardByIdQuery { OnGuardId = onGuardId });
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.OnGuard)]
		public async Task<ActionResult<ProcessResponse<string>>> Insert(InsertOnGuardCommand request)
		{
			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}

		[HttpPost]
		//[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.OnGuard)]
		public async Task<ActionResult<ProcessResponse>> Update(UpdateOnGuardCommand request)
		{
			var result = await this.Mediator.Send(request);
			return this.Ok(result);
		}
	}
}
