using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Roles.Commands.Delete;
using Planner.Application.Roles.Commands.Insert;
using Planner.Application.Roles.Commands.Update;
using Planner.Application.Roles.Queries.GetRoleById;
using Planner.Application.Roles.Queries.GetRoles;
using Planner.Common.Data;
using Planner.Domain.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	public class RoleController : BaseController
    {
        [HttpGet]
        public async Task<ActionResult<ProcessResponse<IEnumerable<RoleListModel>>>> GetRolesList()
        {
            var result = await this.Mediator.Send(new GetRolesListQuery());
            return this.Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ProcessResponse<RoleModel>>> GetRoleById(string roleId)
        {
            var result = await this.Mediator.Send(new GetRoleByIdQuery { RoleId = roleId });
            return this.Ok(result);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.RoleManagement)]
        public async Task<ActionResult<ProcessResponse<Guid>>> Insert([FromBody] InsertRoleCommand request)
        {
            var result = await this.Mediator.Send(request);
            return this.Ok(result);
        }

        [HttpPost]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.RoleManagement)]
        public async Task<ActionResult<ProcessResponse>> Update([FromBody] UpdateRoleCommand request)
        {
            var result = await this.Mediator.Send(request);
            return this.Ok(result);
        }

        [HttpDelete]
        //[Authorize(Policy = ClaimsKeys.SettingsClaimKeys.RoleManagement)]
        public async Task<ActionResult<ProcessResponse>> Delete(string roleId)
        {
            var result = await this.Mediator.Send(new DeleteRoleCommand { RoleId = roleId });
            return this.Ok(result);
        }
    }
}
