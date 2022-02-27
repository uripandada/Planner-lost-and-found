using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Application.Admin.Authentication.Queries.Login;
using Planner.Common.Data;
using System.Threading.Tasks;

namespace Planner.WebAdminUi.Controllers
{
	public class AccountController : BaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ProcessResponse<MasterLoginModel>>> Login(MasterLoginQuery request)
        {
            return await this.Mediator.Send(request);
        }
    }	
}
