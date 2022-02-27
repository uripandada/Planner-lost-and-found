using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Planner.WebUi.Controllers.External
{
	[ApiController]
	[Route("external-api/[controller]/[action]")]
	public class BaseExternalApiController : Controller
	{
		private IMediator _mediator;

		protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
	}
}
