using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Planner.WebUi.Controllers.Rcc
{
	[ApiController]
	[Route("rcc-api/[controller]/[action]")]
	public class BaseRccApiController : Controller
	{
		private IMediator _mediator;

		protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
	}
}
