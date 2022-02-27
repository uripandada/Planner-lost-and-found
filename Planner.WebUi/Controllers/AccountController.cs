using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Planner.Application.Authentication.Queries.Login;
using Planner.Common.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.WebUi.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class AccountController : Controller
	{
		private IMediator _mediator;

		protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());

		[HttpPost]
		public async Task<ActionResult<ProcessResponse>> Login(LoginQuery request)
		{

			var result = await this.Mediator.Send(request);
			if (result.IsSuccess)
			{
				this.HttpContext.Response.Cookies.Delete("hotel_group_key");
				this.HttpContext.Response.Cookies.Append("hotel_group_id", result.Data.HotelGroupId, result.Data.CookieOptions);

				return this.Ok(new ProcessResponse
				{
					IsSuccess = result.IsSuccess,
					HasError = result.HasError,
					Message = result.Message
				});
			}

			return this.Unauthorized();
		}
	}
}
