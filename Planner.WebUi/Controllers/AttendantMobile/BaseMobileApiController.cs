using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Planner.WebUi.Controllers.AttendantMobile
{
	[ApiController]
	[Route("mobile-api/[controller]/[action]")]
	public class BaseMobileApiController : Controller
	{
		private IMediator _mediator;

		protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
	}

}
