using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.LostAndFounds.Commands.Insert;
using Planner.Application.LostAndFounds.Commands.Update;
using Planner.Application.LostAndFounds.Models;
using Planner.Application.LostAndFounds.Queries.GetById;
using Planner.Application.LostAndFounds.Queries.GetList;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
    public class LostAndFoundController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<ProcessResponse<PageOf<LostAndFoundListItem>>>> GetList(GetLostAndFoundListQuery query)
        {
            var result = await this.Mediator.Send(query);
            return this.Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ProcessResponse<LostAndFoundModel>>> GetById(string lostAndFoundId)
        {
            var result = await this.Mediator.Send(new GetLostAndFoundByIdQuery { LostAndFoundId = lostAndFoundId });
            return this.Ok(result);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.LostAndFound)]
        public async Task<ActionResult<ProcessResponse<string>>> Insert(InsertLostAndFoundCommand request)
        {
            var result = await this.Mediator.Send(request);
            return this.Ok(result);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.ManagementClaimKeys.LostAndFound)]
        public async Task<ActionResult<ProcessResponse>> Update(UpdateLostAndFoundCommand request)
        {
            var result = await this.Mediator.Send(request);
            return this.Ok(result);
        }

        


        
    }
}
